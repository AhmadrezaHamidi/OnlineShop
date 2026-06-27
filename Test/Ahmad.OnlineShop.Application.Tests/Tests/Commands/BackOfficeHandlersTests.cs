/// <summary>
/// تست‌های Application Handler مدیران (BackOfficeHandlers)
/// پوشش‌دهنده: ایجاد مدیر، تغییر نقش، فعال/غیرفعال/تعلیق، گزارش‌ها
/// تکنولوژی Mock: NSubstitute — IAdminUserRepository
/// خطاهای تست‌شده: AdminEmailAlreadyExistsException | AdminNotFoundException | ReportNotFoundException
/// </summary>
using Ahmad.OnlineShop.Domain.BackOffice.Args;

namespace Ahmad.OnlineShop.Application.BackOffice.Tests;

public class BackOfficeHandlersTests
{
    private readonly IAdminUserRepository _repo = Substitute.For<IAdminUserRepository>();
    private readonly BackOfficeHandlers   _sut;
    private readonly CancellationToken    _ct = CancellationToken.None;

    public BackOfficeHandlersTests()
    {
        _sut = new BackOfficeHandlers(_repo);
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static AdminUser MakeAdmin() =>
        AdminUser.Create(new CreateAdminUserArg(1, "Ahmad", "a@b.com", AdminRole.SuperAdmin));

    // ─── CreateAdminUserCommand ───────────────────────────────────────────────

    /// <summary>ایجاد مدیر جدید باید آن را در Repository ذخیره کند و Id را برگرداند</summary>
    [Fact]
    public async Task Create_When_EmailNotExists_Should_AddAdmin_And_ReturnId()
    {
        _repo.GetByEmailAsync("a@b.com", _ct).ReturnsNull();
        _repo.GetNextId().Returns(1L);

        var result = await _sut.Handle(
            new CreateAdminUserCommand("Ahmad", "a@b.com", AdminRole.SuperAdmin), _ct);

        Assert.Equal(1, result);
        await _repo.Received(1).AddAsync(Arg.Any<AdminUser>(), _ct);
    }

    /// <summary>خطا: ایمیل تکراری → AdminEmailAlreadyExistsException</summary>
    [Fact]
    public async Task Create_When_EmailExists_Should_Throw_AdminEmailAlreadyExistsException()
    {
        _repo.GetByEmailAsync("a@b.com", _ct).Returns(MakeAdmin());

        await Assert.ThrowsAsync<AdminEmailAlreadyExistsException>(
            () => _sut.Handle(new CreateAdminUserCommand("Ahmad", "a@b.com", AdminRole.SuperAdmin), _ct));

        await _repo.DidNotReceive().AddAsync(Arg.Any<AdminUser>(), _ct);
    }

    // ─── ChangeAdminRoleCommand ───────────────────────────────────────────────

    /// <summary>تغییر نقش مدیر باید Repository را آپدیت کند</summary>
    [Fact]
    public async Task ChangeRole_When_AdminExists_Should_UpdateRepo_And_ReturnId()
    {
        var admin = MakeAdmin();
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        var result = await _sut.Handle(
            new ChangeAdminRoleCommand(1, AdminRole.OrderManager), _ct);

        Assert.Equal(1, result);
        Assert.Equal(AdminRole.OrderManager, admin.Role);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    /// <summary>خطا: مدیر پیدا نشد → AdminNotFoundException</summary>
    [Fact]
    public async Task ChangeRole_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new ChangeAdminRoleCommand(99, AdminRole.OrderManager), _ct));
    }

    // ─── ActivateAdminCommand ─────────────────────────────────────────────────

    /// <summary>خطا: مدیر پیدا نشد برای فعال‌سازی → AdminNotFoundException</summary>
    [Fact]
    public async Task Activate_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new ActivateAdminCommand(99), _ct));
    }

    /// <summary>فعال‌سازی مدیر غیرفعال باید موفق باشد</summary>
    [Fact]
    public async Task Activate_When_AdminInactive_Should_Activate_And_Update()
    {
        var admin = MakeAdmin();
        admin.Deactivate();
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        await _sut.Handle(new ActivateAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Active, admin.Status);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    // ─── DeactivateAdminCommand ───────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی مدیر باید وضعیت را Inactive کند</summary>
    [Fact]
    public async Task Deactivate_When_AdminActive_Should_Deactivate_And_Update()
    {
        var admin = MakeAdmin();
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        await _sut.Handle(new DeactivateAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Inactive, admin.Status);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    /// <summary>خطا: مدیر پیدا نشد برای غیرفعال‌سازی → AdminNotFoundException</summary>
    [Fact]
    public async Task Deactivate_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new DeactivateAdminCommand(99), _ct));
    }

    // ─── SuspendAdminCommand ──────────────────────────────────────────────────

    /// <summary>تعلیق مدیر باید وضعیت را Suspended کند</summary>
    [Fact]
    public async Task Suspend_When_AdminExists_Should_Suspend_And_Update()
    {
        var admin = MakeAdmin();
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        await _sut.Handle(new SuspendAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Suspended, admin.Status);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    // ─── RequestReportCommand ─────────────────────────────────────────────────

    /// <summary>درخواست گزارش باید گزارش با وضعیت Pending به مدیر اضافه کند</summary>
    [Fact]
    public async Task RequestReport_When_AdminExists_Should_AddReport_And_ReturnReportId()
    {
        var admin = MakeAdmin();
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        var result = await _sut.Handle(
            new RequestReportCommand(1, 10, ReportType.Sales), _ct);

        Assert.Single(admin.Reports);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    /// <summary>خطا: مدیر پیدا نشد برای گزارش → AdminNotFoundException</summary>
    [Fact]
    public async Task RequestReport_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        _repo.GetByIdAsync(99, _ct).ReturnsNull();

        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new RequestReportCommand(99, 10, ReportType.Sales), _ct));
    }

    // ─── CompleteReportCommand ────────────────────────────────────────────────

    /// <summary>تکمیل گزارش باید وضعیت گزارش را Completed کند</summary>
    [Fact]
    public async Task CompleteReport_When_ReportExists_Should_MarkCompleted()
    {
        var admin  = MakeAdmin();
        var report = admin.RequestReport(new CreateReportArg(10, admin.Id, ReportType.Sales));
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        await _sut.Handle(new CompleteReportCommand(1, report.Id, "/reports/file.pdf"), _ct);

        Assert.Equal(ReportStatus.Completed, report.Status);
        await _repo.Received(1).UpdateAsync(admin, _ct);
    }

    // ─── FailReportCommand ────────────────────────────────────────────────────

    /// <summary>شکست گزارش باید وضعیت گزارش را Failed کند</summary>
    [Fact]
    public async Task FailReport_When_ReportExists_Should_MarkFailed()
    {
        var admin  = MakeAdmin();
        var report = admin.RequestReport(new CreateReportArg(10, admin.Id, ReportType.Sales));
        _repo.GetByIdAsync(1, _ct).Returns(admin);

        await _sut.Handle(new FailReportCommand(1, report.Id, "خطای سرور"), _ct);

        Assert.Equal(ReportStatus.Failed, report.Status);
        Assert.Equal("خطای سرور",        report.FailReason);
    }
}
