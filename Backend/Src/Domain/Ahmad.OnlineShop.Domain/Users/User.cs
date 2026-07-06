using AhmadBase.Doamin;
using Microsoft.AspNetCore.Identity;
using Ahmad.OnlineShop.Domain.Users.Args;
using Ahmad.OnlineShop.Domain.Users.Exceptions;

namespace Ahmad.OnlineShop.Domain.User;

public class User : IdentityUser<long>, IAggregateRoot
{
    public IReadOnlyCollection<IEvent> DomainEvents => Array.Empty<IEvent>();
    public void ClearDomainEvents() { }

    public string? Name        { get; private set; }
    public string? Family      { get; private set; }
    public string? DisplayName { get; private set; }
    public string? NationalCode { get; private set; }

    public bool IsDeleted { get; private set; }
    public bool IsEnabled { get; private set; } = true;

    private readonly List<UserRole> _roles    = [];
    private readonly List<Session>  _sessions = [];

    public IReadOnlyCollection<UserRole> Roles    => _roles.AsReadOnly();
    public IReadOnlyCollection<Session>  Sessions => _sessions.AsReadOnly();

    private User() { }

    private User(CreateUserArg arg)
    {
        Id          = arg.Id;
        PhoneNumber = FormatPhoneNumber(arg.PhoneNumber);
        IsEnabled   = true;
    }

    /// <summary>
    /// ایجاد کاربر جدید — چک تکراری بودن در Application Handler انجام می‌شود
    /// </summary>
    public static User Create(CreateUserArg arg)
    {
        GuardPhone(arg.PhoneNumber);
        return new User(arg);
    }

    public void CompleteRegistration(CompleteRegistrationArg arg)
    {
        GuardName(arg.Name, arg.Family);
        GuardNationalCode(arg.NationalCode);

        Name        = arg.Name;
        Family      = arg.Family;
        DisplayName = $"{arg.Name} {arg.Family}";
        Email       = arg.Email;
        NationalCode = arg.NationalCode;
    }

    public void Modify(ModifyUserArg arg)
    {
        GuardName(arg.Name, arg.Family);

        Name        = arg.Name;
        Family      = arg.Family;
        DisplayName = $"{arg.Name} {arg.Family}";
        Email       = arg.Email;
    }

    public Guid Login(string hashedPassword)
    {
        VerifyPassword(hashedPassword);

        var sessionId = Guid.NewGuid();
        _sessions.Add(Session.New(Id, sessionId));
        return sessionId;
    }

    public void Logout(Guid sessionId)
    {
        var session = _sessions.FirstOrDefault(x => x.SessionId == sessionId)
            ?? throw new InvalidOperationException("Session not found");

        session.Deactivate();
    }

    public void ChangePassword(string newHash) => PasswordHash = newHash;

    public void AssignRole(long roleId) => _roles.Add(UserRole.New(roleId, Id));

    public void Disable() => IsEnabled = false;
    public void Enable()  => IsEnabled = true;

    // ─── Guards ───────────────────────────────────────────────────────────────

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
        if (string.IsNullOrWhiteSpace(phone)) return null;

        return phone switch
        {
            _ when phone.StartsWith("0098") => "0" + phone[4..],
            _ when phone.StartsWith("+98")  => "0" + phone[3..],
            _ when phone.StartsWith("98")   => "0" + phone[2..],
            _                               => phone
        };
    }
}
