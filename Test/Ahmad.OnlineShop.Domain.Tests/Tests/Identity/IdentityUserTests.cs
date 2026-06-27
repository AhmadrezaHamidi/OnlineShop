/// <summary>
/// تست‌های Aggregate Root کاربر Identity (OTP-based)
/// ─────────────────────────────────────────────────────────────────────
/// سیستم جدید: ورود با OTP — نه پسورد
/// User فقط شماره موبایل دارد — بدون Email یا PasswordHash
///
/// پوشش‌دهنده: ایجاد کاربر، نوع کاربر، آپدیت پروفایل، وضعیت، نقش‌ها
/// خطاهای تست‌شده: UserAlreadyExistsException
/// </summary>
using Identity.Domain.Enums;
using IdentityUser = Identity.Domain.Aggregates.User;

namespace Ahmad.OnlineShop.Domain.Identity.Tests;

public class IdentityUserTests
{
    // ── Factory ──────────────────────────────────────────────────────────────

    private static IdentityUser CreateCustomer(
        long   id    = 1,
        string phone = "09121234567")
        => IdentityUser.Create(id, phone, UserType.Customer);

    private static IdentityUser CreateSeller(
        long   id    = 2,
        string phone = "09129876543")
        => IdentityUser.Create(id, phone, UserType.Seller);

    // ═══════════════════════════════════════════════════════════════════════
    // بخش اول: ایجاد کاربر
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>ایجاد مشتری باید مشخصات را ست کند و وضعیت Active باشد</summary>
    [Fact]
    public void Create_Customer_Should_Set_PhoneNumber_And_ActiveStatus()
    {
        var user = CreateCustomer(id: 1, phone: "09121234567");

        Assert.Equal(1,               user.Id);
        Assert.Equal("09121234567",   user.PhoneNumber);
        Assert.Equal(UserType.Customer, user.UserType);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Empty(user.RoleIds);
    }

    /// <summary>ایجاد فروشنده باید UserType.Seller داشته باشد</summary>
    [Fact]
    public void Create_Seller_Should_Set_UserType_Seller()
    {
        var seller = CreateSeller(id: 2, phone: "09129876543");

        Assert.Equal(UserType.Seller, seller.UserType);
        Assert.Equal(UserStatus.Active, seller.Status);
    }

    /// <summary>بدون UserType مشخص، نوع پیش‌فرض Customer است</summary>
    [Fact]
    public void Create_Without_UserType_Should_Default_To_Customer()
    {
        var user = IdentityUser.Create(1, "09121234567");

        Assert.Equal(UserType.Customer, user.UserType);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش دوم: آپدیت پروفایل
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>آپدیت پروفایل باید FullName را ست کند</summary>
    [Fact]
    public void UpdateProfile_Should_Set_FullName()
    {
        var user = CreateCustomer();

        user.UpdateProfile("Ahmad Hamidi");

        Assert.Equal("Ahmad Hamidi", user.FullName);
    }

    /// <summary>پس از ایجاد FullName خالی است (OTP — نامی وجود ندارد)</summary>
    [Fact]
    public void Create_Should_Have_Null_FullName_Initially()
    {
        var user = CreateCustomer();

        Assert.Null(user.FullName);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش سوم: وضعیت کاربر
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>غیرفعال‌سازی باید وضعیت را Inactive کند</summary>
    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        var user = CreateCustomer();

        user.Deactivate();

        Assert.Equal(UserStatus.Inactive, user.Status);
    }

    /// <summary>تعلیق باید وضعیت را Suspended کند</summary>
    [Fact]
    public void Suspend_Should_Change_Status_To_Suspended()
    {
        var user = CreateCustomer();

        user.Suspend();

        Assert.Equal(UserStatus.Suspended, user.Status);
    }

    /// <summary>فعال‌سازی بعد از غیرفعال‌سازی باید وضعیت را Active کند</summary>
    [Fact]
    public void Activate_After_Deactivate_Should_Change_Status_To_Active()
    {
        var user = CreateCustomer();
        user.Deactivate();

        user.Activate();

        Assert.Equal(UserStatus.Active, user.Status);
    }

    /// <summary>خطا: Activate کاربر Active → UserAlreadyExistsException</summary>
    [Fact]
    public void Activate_When_AlreadyActive_Should_Throw_UserAlreadyExistsException()
    {
        var user = CreateCustomer(); // status = Active

        Assert.Throws<UserAlreadyExistsException>(() => user.Activate());
    }

    // ═══════════════════════════════════════════════════════════════════════
    // بخش چهارم: مدیریت نقش‌ها
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>اختصاص نقش باید RoleId را اضافه کند</summary>
    [Fact]
    public void AssignRole_Should_Add_RoleId()
    {
        var user = CreateCustomer();

        user.AssignRole(10);

        Assert.Contains(10L, user.RoleIds);
        Assert.Single(user.RoleIds);
    }

    /// <summary>اختصاص نقش تکراری نباید آن را دوبار اضافه کند</summary>
    [Fact]
    public void AssignRole_Same_Role_Twice_Should_Not_Duplicate()
    {
        var user = CreateCustomer();

        user.AssignRole(10);
        user.AssignRole(10);

        Assert.Single(user.RoleIds);
    }

    /// <summary>اختصاص چند نقش باید همه را اضافه کند</summary>
    [Fact]
    public void AssignRole_Multiple_Roles_Should_Add_All()
    {
        var user = CreateCustomer();

        user.AssignRole(10);
        user.AssignRole(20);
        user.AssignRole(30);

        Assert.Equal(3, user.RoleIds.Count);
    }

    /// <summary>حذف نقش باید RoleId را پاک کند</summary>
    [Fact]
    public void RemoveRole_Should_Remove_RoleId()
    {
        var user = CreateCustomer();
        user.AssignRole(10);

        user.RemoveRole(10);

        Assert.Empty(user.RoleIds);
    }

    /// <summary>حذف نقشی که اختصاص داده نشده نباید خطا بدهد</summary>
    [Fact]
    public void RemoveRole_When_NotAssigned_Should_DoNothing()
    {
        var user = CreateCustomer();

        user.RemoveRole(999);

        Assert.Empty(user.RoleIds);
    }
}
