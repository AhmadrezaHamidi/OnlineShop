using Base.Application.Common.Response;
using Base.Application.Common;
using Base.Domain.Entities.userInf;
using Base.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Base.Application.Extensions;

namespace Base.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int UserId => int.Parse(User.Identity.FindFirstValue("UserId"));


        protected string UserKey => User.FindFirstValue(ClaimTypes.UserData);


        protected IActionResult OperationResult(dynamic result)
        {
            if (result is null)
                return new ServerErrorResult("مشکلی به وجود آمده است");

            if (!((object)result).IsAssignableFromBaseTypeGeneric(typeof(OperationResult<>)))
            {
                throw new InvalidCastException("Given Type is not an OperationResult<T>");
            }


            if (result.IsSuccess) return result.Result is bool ? Ok() : Ok(result.Result);

            if (result.IsNotFound)
                return NotFound(result.ErrorMessage);

            ModelState.AddModelError("GeneralError", result.ErrorMessage);
            return BadRequest(ModelState);

        }

    }
}
