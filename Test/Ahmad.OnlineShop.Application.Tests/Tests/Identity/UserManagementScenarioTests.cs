/// <summary>
/// سناریوهای مدیریت کاربر (توسط ادمین)
/// ─────────────────────────────────────────────────────────────────────────────
/// - آپدیت پروفایل (نام)
/// - فعال / غیرفعال / تعلیق کاربر
/// - اختصاص / حذف نقش
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;
using Ahmad.OnlineShop.Application.Tests.Fakes.Identity;
using Identity.Application.Commands;
using Identity.Application.Handlers;
using IdentityUser = Identity.Domain.Aggregates.User;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;

namespace Ahmad.OnlineShop.Application.Tests.Identity;

public class UserManagementScenarioTests
{
    private const string Phone = "09121234567";

    private readonly FakeIdentityUserRepository _userRepo    = new();
    private readonly FakeOtpRepository          _otpRepo     = new();
    private readonly FakeRefreshTokenRepository _refreshRepo = new();
    private readonly FakeIdentityRoleRepository _roleRepo    = new();
    private readonly FakeSmsService             _sms         = new();
    private readonly FakeJwtService             _jwt         = new();
    private readonly IdentityHandlers           _sut;
    private readonly CancellationToken          _ct = CancellationToken.None;

    public UserManagementScenarioTests()
    {
        _sut = new IdentityHandlers(_userRepo, _refreshRepo, _roleRepo, _otpRepo, _sms, _jwt, FakeIdentityDb.Create());
    }

    // ── آپدیت پروفایل ─────────────────────────────────────────────────────────

    /// <summary>آپدیت نام کامل کاربر باید FullName را تغییر دهد</summary>
    [Fact]
    public async Task UpdateProfile_Should_Set_FullName()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);

        await _sut.Handle(new UpdateProfileCommand(1, "Ahmad Hamidi"), _ct);

        Assert.Equal("Ahmad Hamidi", user.FullName);
    }

    /// <summary>خطا: کاربر پیدا نشد → UserNotFoundException</summary>
    [Fact]
    public async Task UpdateProfile_When_UserNotFound_Should_Throw_UserNotFoundException()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _sut.Handle(new UpdateProfileCommand(99, "Ahmad"), _ct));
    }

    // ── وضعیت کاربر ──────────────────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی کاربر Active باید وضعیت را Inactive کند</summary>
    [Fact]
    public async Task Deactivate_Should_Change_Status_To_Inactive()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);

        await _sut.Handle(new DeactivateUserCommand(1), _ct);

        Assert.Equal(UserStatus.Inactive, user.Status);
    }

    /// <summary>تعلیق کاربر باید وضعیت را Suspended کند</summary>
    [Fact]
    public async Task Suspend_Should_Change_Status_To_Suspended()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);

        await _sut.Handle(new SuspendUserCommand(1), _ct);

        Assert.Equal(UserStatus.Suspended, user.Status);
    }

    /// <summary>فعال‌سازی کاربر Inactive باید وضعیت را Active کند</summary>
    [Fact]
    public async Task Activate_After_Deactivate_Should_Change_Status_To_Active()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        user.Deactivate();
        _userRepo.Seed(user);

        await _sut.Handle(new ActivateUserCommand(1), _ct);

        Assert.Equal(UserStatus.Active, user.Status);
    }

    // ── اختصاص نقش ───────────────────────────────────────────────────────────

    /// <summary>اختصاص نقش فروشنده به مشتری</summary>
    [Fact]
    public async Task AssignSellerRole_Should_Add_RoleId_To_User()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);
        _roleRepo.Seed(new Role(10, "Seller"));

        await _sut.Handle(new AssignRoleCommand(1, 10), _ct);

        Assert.Contains(10L, user.RoleIds);
    }

    /// <summary>اختصاص نقش تکراری نباید آن را دوبار اضافه کند</summary>
    [Fact]
    public async Task AssignRole_Twice_Should_Not_Duplicate()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);
        _roleRepo.Seed(new Role(10, "Seller"));

        await _sut.Handle(new AssignRoleCommand(1, 10), _ct);
        await _sut.Handle(new AssignRoleCommand(1, 10), _ct);

        Assert.Single(user.RoleIds);
    }

    /// <summary>حذف نقش باید RoleId را پاک کند</summary>
    [Fact]
    public async Task RemoveRole_Should_Remove_RoleId_From_User()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        user.AssignRole(10);
        _userRepo.Seed(user);
        _roleRepo.Seed(new Role(10, "Seller"));

        await _sut.Handle(new RemoveRoleCommand(1, 10), _ct);

        Assert.Empty(user.RoleIds);
    }

    /// <summary>خطا: نقش پیدا نشد → RoleNotFoundException</summary>
    [Fact]
    public async Task AssignRole_When_RoleNotFound_Should_Throw_RoleNotFoundException()
    {
        var user = FakeIdentityUserRepository.ExistingCustomer(1, Phone);
        _userRepo.Seed(user);

        await Assert.ThrowsAsync<RoleNotFoundException>(
            () => _sut.Handle(new AssignRoleCommand(1, 99), _ct));
    }

    // ── سناریوی کامل: ورود → پروفایل → نقش ─────────────────────────────────

    /// <summary>
    /// سناریوی کامل: کاربر وارد می‌شود → نام خود را ثبت می‌کند → نقش فروشنده می‌گیرد
    /// </summary>
    [Fact]
    public async Task FullScenario_NewSeller_Login_UpdateProfile_AssignRole()
    {
        // گام ۱: ورود با OTP
        _otpRepo.SeedValidOtp(Phone, "123456");
        var loginResult = await _sut.Handle(new VerifyOtpCommand(Phone, "123456"), _ct);
        var userId = loginResult.UserId;

        // گام ۲: ثبت نام
        await _sut.Handle(new UpdateProfileCommand(userId, "Ali Rezaei"), _ct);

        // گام ۳: اختصاص نقش فروشنده
        _roleRepo.Seed(new Role(10, "Seller"));
        await _sut.Handle(new AssignRoleCommand(userId, 10), _ct);

        // بررسی
        var user = await _userRepo.GetByPhoneAsync(Phone);
        Assert.NotNull(user);
        Assert.Equal("Ali Rezaei", user!.FullName);
        Assert.Contains(10L, user.RoleIds);
    }
}
