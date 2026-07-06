using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Users.Args;

public sealed record CompleteRegistrationArg(
string Name,
string Family,
string? NationalCode,
string? Email
);
