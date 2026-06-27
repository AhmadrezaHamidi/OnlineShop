using AhmadBase.Doamin;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;

namespace Identity.Domain.Aggregates;

/// <summary>
/// Aggregate Root کاربر سیستم (مشتری یا فروشنده)
/// ادمین‌ها به صورت دستی در دیتابیس ثبت می‌شوند و از این aggregate استفاده نمی‌کنند
/// </summary>
public sealed class User : AggregateRoot<long>
{
    public string   PhoneNumber { get; private set; } = string.Empty;
    public string?  FullName    { get; private set; }
    public UserType UserType    { get; private set; }
    public UserStatus Status    { get; private set; }
    public DateTime CreatedAt   { get; private set; }

    private readonly List<long> _roleIds = [];
    public IReadOnlyCollection<long> RoleIds => _roleIds.AsReadOnly();

    private User() { }

    private User(long id, string phoneNumber, UserType userType) : base(id)
    {
        PhoneNumber = phoneNumber;
        UserType    = userType;
        Status      = UserStatus.Active;
        CreatedAt   = DateTime.UtcNow;
    }

    /// <summary>ایجاد کاربر جدید (اولین ورود با OTP)</summary>
    public static User Create(long id, string phoneNumber, UserType userType = UserType.Customer)
        => new(id, phoneNumber, userType);

    /// <summary>ثبت/بروزرسانی اطلاعات پروفایل</summary>
    public void UpdateProfile(string fullName)
        => FullName = fullName;

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new UserAlreadyExistsException();
        Status = UserStatus.Active;
    }

    public void Deactivate()  => Status = UserStatus.Inactive;
    public void Suspend()     => Status = UserStatus.Suspended;

    public void AssignRole(long roleId)
    {
        if (!_roleIds.Contains(roleId))
            _roleIds.Add(roleId);
    }

    public void RemoveRole(long roleId) => _roleIds.Remove(roleId);
}
