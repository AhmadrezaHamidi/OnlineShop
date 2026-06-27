using AhmadBase.Doamin;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Aggregates;

public sealed class User : AggregateRoot<long>
{
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<long> _roleIds = [];
    public IReadOnlyCollection<long> RoleIds => _roleIds.AsReadOnly();

    private User() { }

    private User(long id, string fullName, string email, string passwordHash, string? phoneNumber)
        : base(id)
    {
        FullName = fullName;
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public static User Register(long id, string fullName, string email, string passwordHash, string? phoneNumber = null)
        => new(id, fullName, email, passwordHash, phoneNumber);

    public void ChangePassword(string newHash)
        => PasswordHash = newHash;

    public void UpdateProfile(string fullName, string? phoneNumber)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new UserAlreadyExistsException();
        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
    }

    public void AssignRole(long roleId)
    {
        if (!_roleIds.Contains(roleId))
            _roleIds.Add(roleId);
    }

    public void RemoveRole(long roleId)
        => _roleIds.Remove(roleId);
}
