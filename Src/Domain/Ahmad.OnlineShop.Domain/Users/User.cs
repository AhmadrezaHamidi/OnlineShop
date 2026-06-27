
using AhmadBase.Doamin;
using Microsoft.AspNetCore.Identity;
using Ahmad.OnlineShop.Domain.Users;
using Ahmad.OnlineShop.Domain.Users.Args;
using Ahmad.OnlineShop.Domain.Users.Exceptions;

namespace Ahmad.OnlineShop.Domain.User;

public class User : IdentityUser<long>, IAggregateRoot
{
    public IReadOnlyCollection<IEvent> DomainEvents => Array.Empty<IEvent>();
    public void ClearDomainEvents() { }
    public string? Name { get; private set; }
    public string? Family { get; private set; }
    public string? DisplayName { get; private set; }
    public string? NationalCode { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsEnabled { get; private set; } = true;

    private readonly List<UserRole> _roles = [];
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    private readonly List<Session> _sessions = [];
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    private User() { }

    private User(CreateUserArg arg)
    {
        Id = arg.Id;
        PhoneNumber = FormatPhoneNumber(arg.PhoneNumber);
        IsEnabled = true;
    }

    public static async Task<User> Create(
        CreateUserArg arg,
        IUserService service,
        CancellationToken token)
    {
        GuardPhone(arg.PhoneNumber);

        if (await service.UserExists(arg.Id, arg.PhoneNumber, token))
            throw new ExistingUserException();

        return new User(arg);
    }

    public void CompleteRegistration(CompleteRegistrationArg arg)
    {
        GuardName(arg.Name, arg.Family);
        GuardNationalCode(arg.NationalCode);

        Name = arg.Name;
        Family = arg.Family;
        DisplayName = $"{arg.Name} {arg.Family}";
        Email = arg.Email;
        NationalCode = arg.NationalCode;
    }

    public void Modify(ModifyUserArg arg)
    {
        GuardName(arg.Name, arg.Family);

        Name = arg.Name;
        Family = arg.Family;
        DisplayName = $"{arg.Name} {arg.Family}";
        Email = arg.Email;
    }

    public Guid Login(string hashedPassword)
    {
        VerifyPassword(hashedPassword);

        var sessionId = Guid.NewGuid();
        var session = Session.New(Id, sessionId);

        _sessions.Add(session);

        return sessionId;
    }

    public void Logout(Guid sessionId)
    {
        var session = _sessions.FirstOrDefault(x => x.SessionId == sessionId);

        if (session is null)
            throw new InvalidOperationException("Session not found");

        session.Deactivate();
    }

    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
    }

    public void AssignRole(long roleId)
    {
        var role = UserRole.New(roleId, Id);
        _roles.Add(role);
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    private void VerifyPassword(string hash)
    {
        if (PasswordHash != hash)
            throw new IncorrectPasswordException();
    }   

    private static void GuardPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new EmptyPhoneNumberException();

        if (phone.Length is < 11 or > 14)
            throw new InvalidPhoneNumberException();
    }

    private static void GuardName(string? name, string? family)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(family))
            throw new EmptyNameException();
    }

    private static void GuardNationalCode(string? nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode) || nationalCode.Length != 10)
            throw new InvalidNationalCodeException();
    }

    private static string? FormatPhoneNumber(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;

        if (phone.StartsWith("0098"))
            phone = "0" + phone[4..];
        else if (phone.StartsWith("+98"))
            phone = "0" + phone[3..];
        else if (phone.StartsWith("98"))
            phone = "0" + phone[2..];

        return phone;
    }
}
