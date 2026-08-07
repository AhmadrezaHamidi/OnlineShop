using Microsoft.AspNetCore.Identity;

namespace Base.Domain.Entities.userInf
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RefreshToken { get; set; } = "";


        //#region Navigation Properties

        //public ICollection<Ticket> Tickets { get; set; }
        //public ICollection<UserTicket> UserTickets { get; set; }

        //#endregion

    }
}
