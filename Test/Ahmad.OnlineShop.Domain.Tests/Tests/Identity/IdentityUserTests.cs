/// <summary>
/// تست‌های Aggregate Root کاربر هویت (Identity User)
/// پوشش‌دهنده: ثبت‌نام، تغییر رمز، آپدیت پروفایل، وضعیت، نقش‌ها
/// خطاهای تست‌شده: UserAlreadyExistsException
/// </summary>
using Identity.Domain.Enums;
using IdentityUser = Identity.Domain.Aggregates.User;

namespace Ahmad.OnlineShop.Domain.Identity.Tests;

public class IdentityUserTests
{
    private static IdentityUser RegisterUser(
        long    id    = 1,
        string  name  = "Ahmad Hamidi",
        string  email = "ahmad@example.com",
        string  hash  = "hashed_pass",
        string? phone = "09123456789")
        => IdentityUser.Register(id, name, email, hash, phone);

    // ── Register ─────────────────────────────────────────────────────────────

    /// <summary>ثبت‌نام باید مشخصات را ست کند و وضعیت Active باشد</summary>
    [Fact]
    public void Register_Should_Set_Properties_And_ActiveStatus()
    {
        var user = RegisterUser();

        Assert.Equal(1,               user.Id);
        Assert.Equal("Ahmad Hamidi",  user.FullName);
        Assert.Equal("ahmad@example.com", user.Email);
        Assert.Equal("hashed_pass",   user.PasswordHash);
        Assert.Equal("09123456789",   user.PhoneNumber);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Empty(user.RoleIds);
    }

    /// <summary>ایمیل باید به حروف کوچک نرمال‌سازی شود</summary>
    [Fact]
    public void Register_Should_Normalize_Email_To_Lowercase()
    {
        var user = RegisterUser(email: "AHMAD@EXAMPLE.COM");

        Assert.Equal("ahmad@example.com", user.Email);
    }

    /// <summary>ثبت‌نام بدون شماره موبایل باید موفق باشد</summary>
    [Fact]
    public void Register_Without_Phone_Should_Succeed()
    {
        var user = RegisterUser(phone: null);

        Assert.Null(user.PhoneNumber);
    }

    // ── ChangePassword ───────────────────────────────────────────────────────

    /// <summary>تغییر رمز باید PasswordHash را آپدیت کند</summary>
    [Fact]
    public void ChangePassword_Should_Update_PasswordHash()
    {
        var user = RegisterUser();
        user.ChangePassword("new_hash");

        Assert.Equal("new_hash", user.PasswordHash);
    }

    // ── UpdateProfile ─────────────────────────────────────────────────────────

    /// <summary>آپدیت پروفایل باید FullName و PhoneNumber را تغییر دهد</summary>
    [Fact]
    public void UpdateProfile_Should_Update_FullName_And_Phone()
    {
        var user = RegisterUser();
        user.UpdateProfile("Ali Rezaei", "09987654321");

        Assert.Equal("Ali Rezaei",  user.FullName);
        Assert.Equal("09987654321", user.PhoneNumber);
    }

    /// <summary>آپدیت پروفایل با موبایل null باید PhoneNumber را پاک کند</summary>
    [Fact]
    public void UpdateProfile_With_Null_Phone_Should_Clear_Phone()
    {
        var user = RegisterUser();
        user.UpdateProfile("Ali Rezaei", null);

        Assert.Null(user.PhoneNumber);
    }

    // ── Activate / Deactivate / Suspend ──────────────────────────────────────

    /// <summary>غیرفعال‌سازی باید وضعیت را Inactive کند</summary>
    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        var user = RegisterUser();
        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
    }

    /// <summary>تعلیق باید وضعیت را Suspended کند</summary>
    [Fact]
    public void Suspend_Should_Change_Status_To_Suspended()
    {
        var user = RegisterUser();
        user.Suspend();

        Assert.Equal(UserStatus.Suspended, user.Status);
    }

    /// <summary>خطا: فعال‌سازی کاربر از قبل Active → UserAlreadyExistsException</summary>
    [Fact]
    public void Activate_When_AlreadyActive_Should_Throw_UserAlreadyExistsException()
    {
        var user = RegisterUser();

        Assert.Throws<UserAlreadyExistsException>(() => user.Activate());
    }

    /// <summary>فعال‌سازی بعد از غیرفعال‌سازی باید وضعیت را Active کند</summary>
    [Fact]
    public void Activate_After_Deactivate_Should_Change_Status_To_Active()
    {
        var user = RegisterUser();
        user.Deactivate();
        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
    }

    // ── AssignRole / RemoveRole ───────────────────────────────────────────────

    /// <summary>اختصاص نقش باید RoleId را اضافه کند</summary>
    [Fact]
    public void AssignRole_Should_Add_RoleId()
    {
        var user = RegisterUser();
        user.AssignRole(10);

        Assert.Contains(10, user.RoleIds);
        Assert.Single(user.RoleIds);
    }

    /// <summary>اختصاص نقش تکراری نباید آن را دوبار اضافه کند</summary>
    [Fact]
    public void AssignRole_Same_Role_Twice_Should_Not_Duplicate()
    {
        var user = RegisterUser();
        user.AssignRole(10);
        user.AssignRole(10);

        Assert.Single(user.RoleIds);
    }

    /// <summary>اختصاص چند نقش باید همه را اضافه کند</summary>
    [Fact]
    public void AssignRole_Multiple_Roles_Should_Add_All()
    {
        var user = RegisterUser();
        user.AssignRole(10);
        user.AssignRole(20);

        Assert.Equal(2, user.RoleIds.Count);
    }

    /// <summary>حذف نقش باید RoleId را پاک کند</summary>
    [Fact]
    public void RemoveRole_Should_Remove_RoleId()
    {
        var user = RegisterUser();
        user.AssignRole(10);
        user.RemoveRole(10);

        Assert.Empty(user.RoleIds);
    }

    /// <summary>حذف نقشی که اختصاص داده نشده نباید خطا بدهد</summary>
    [Fact]
    public void RemoveRole_When_NotAssigned_Should_DoNothing()
    {
        var user = RegisterUser();
        user.RemoveRole(999);

        Assert.Empty(user.RoleIds);
    }
}
