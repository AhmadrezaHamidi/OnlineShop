using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Commands;

public record ChangeAdminRoleCommand(
    [property: JsonIgnore] long AdminId,
    AdminRole NewRole
) : ICommand<long>;
