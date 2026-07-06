using AhmadBase.Application.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Application.Query.User;

public sealed record GetProfileQuery(long UserId) : IQuery<GetProfileQueryResponse>;

public sealed record GetProfileQueryResponse(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);