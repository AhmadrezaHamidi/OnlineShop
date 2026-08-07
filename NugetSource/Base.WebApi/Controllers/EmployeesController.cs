using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Base.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateEmployee(CreateEmployeeDto command)
        //{
        //    //var commandCreated = new CreateEmployeeCommand(command?.Name, command.EmployeeNumber);
        //    try
        //    {
        //        //var result = await _mediator.Send();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    //return CreatedAtAction(nameof(GetEmployee), new { id = result?.Entity }, result);
        //    return Ok();
        //}


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var query = new GetEmployeeQuery { Id = id };
            //var result = await _mediator.Send(query);
            return Ok();

            //return Ok(result);
        }
    }
}
