using Base.Application.Dtos.Auth;
using Base.Domain.Entities.userInf;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Application.Contracts
{
    public interface ITokenInfoService
    {
        Task<TokenDto> GenerateToken(User user);
        long GetCurrentUserId();
        TokenValidationParameters GetValidationParameters();
    }
}
