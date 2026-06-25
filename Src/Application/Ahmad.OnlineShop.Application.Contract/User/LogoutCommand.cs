using AhmadBase.Application;
using System.Text.Json.Serialization;


namespace Ahmad.OnlineShop.Application.Contract.User;

public sealed class LogoutCommand : ICommand<long>
{
    [JsonIgnore] public long UserId { get; set; }
}

