using Base.Application.Profiles;
using Base.Domain.Entities.userInf;

namespace Base.Application.Dtos.Auth
{
    public class UserListDto : ICreateMapper<User>
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }


    }
}
