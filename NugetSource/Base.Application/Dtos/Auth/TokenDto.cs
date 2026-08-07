using Base.Application.Profiles;
using Base.Domain.Entities.userInf;
using Duende.IdentityServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Application.Dtos.Auth
{
    public class TokenDto : ICreateMapper<User>
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public int UserId { get; set; }

        public string Email { get; set; }
        public string Roles { get; set; }

    }
}
