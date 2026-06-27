using Ahmad.OnlineShop.Application.Query.User;

namespace Ahmad.OnlineShop.Application.Query.UserQueryHandlers
{
    public static class UserMapper
    {
        public static GetProfileQueryResponse Map(Ahmad.OnlineShop.Domain.User.User user)
            => new GetProfileQueryResponse(
                user.Id,
                user.UserName,
                string.Empty,
                user.Email,
                user.PhoneNumber
            );

        //public static GetAccountsQueryResponse ToGetAccountsResponse(this User user)
        //    => new(
        //        user.Id,
        //        user.Email,
        //        user.PhoneNumber
        //    );
    }
}
