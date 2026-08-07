using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Base.Domain.Entities.userInf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace Base.Application.Services.Auth.AuthManeger
{
    public class AppSignInManager : SignInManager<User>
    {
        public AppSignInManager(UserManager<User> userManager
            , IHttpContextAccessor contextAccessor
            , IUserClaimsPrincipalFactory<User> claimsFactory
            , IOptions<IdentityOptions> optionsAccessor
            , ILogger<SignInManager<User>> logger
            , IAuthenticationSchemeProvider schemes
            , IUserConfirmation<User> confirmation) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }
    }
    public class AppRoleManager : RoleManager<Role>
    {
        public AppRoleManager(IRoleStore<Role> store
            , IEnumerable<IRoleValidator<Role>> roleValidators
            , ILookupNormalizer keyNormalizer
            , IdentityErrorDescriber errors
            , ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
