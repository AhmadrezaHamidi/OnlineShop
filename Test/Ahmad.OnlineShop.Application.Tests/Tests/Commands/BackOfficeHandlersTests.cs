/// <summary>
/// تست‌های Application Handler مدیران (BackOfficeHandlers)
/// پوشش‌دهنده: ایجاد مدیر، تغییر نقش، فعال/غیرفعال/تعلیق، گزارش‌ها
/// تکنولوژی: Fake Repository
/// خطاهای تست‌شده: AdminEmailAlreadyExistsException | AdminNotFoundException
/// </summary>
using Ahmad.OnlineShop.Application.Tests.Fakes;

namespace Ahmad.OnlineShop.Application.BackOffice.Tests;

public class BackOfficeHandlersTests
{
    private readonly FakeAdminUserRepository _repo = new();
    private readonly BackOfficeHandlers      _sut;
    private readonly CancellationToken       _ct = CancellationToken.None;

    public BackOfficeHandlersTests()
    {
        _sut = new BackOfficeHandlers(_repo);
    }

    private static AdminUser MakeAdmin() =>
        AdminUser.Create(new CreateAdminUserArg(1, "Ahmad", "a@b.com", AdminRole.SuperAdmin));

    // ─── CreateAdminUserCommand ───────────────────────────────────────────────

    /// <summary>ایجاد مدیر جدید باید آن را در Repository ذخیره کند</summary>
    [Fact]
    public async Task Create_When_EmailNotExists_Should_AddAdmin()
    {
        _repo.FoundByEmail = null;

        var result = await _sut.Handle(
            new CreateAdminUserCommand("Ahmad", "a@b.com", AdminRole.SuperAdmin), _ct);

        Assert.NotNull(_repo.Added);
        Assert.Equal("Ahmad", _repo.Added!.FullName);
    }

    /// <summary>خطا: ایمیل تکراری → AdminEmailAlreadyExistsException</summary>
    [Fact]
    public async Task Create_When_EmailExists_Should_Throw_AdminEmailAlreadyExistsException()
    {
        _repo.FoundByEmail = MakeAdmin();

        await Assert.ThrowsAsync<AdminEmailAlreadyExistsException>(
            () => _sut.Handle(new CreateAdminUserCommand("Ahmad", "a@b.com", AdminRole.SuperAdmin), _ct));

        Assert.Null(_repo.Added);
    }

    // ─── ChangeAdminRoleCommand ───────────────────────────────────────────────

    /// <summary>تغییر نقش مدیر باید Repository را آپدیت کند</summary>
    [Fact]
    public async Task ChangeRole_When_AdminExists_Should_UpdateRole()
    {
        var admin = MakeAdmin();
        _repo.Seed(admin);

        await _sut.Handle(new ChangeAdminRoleCommand(1, AdminRole.OrderManager), _ct);

        Assert.Equal(AdminRole.OrderManager, admin.Role);
        Assert.NotNull(_repo.Updated);
    }

    /// <summary>خطا: مدیر پیدا نشد → AdminNotFoundException</summary>
    [Fact]
    public async Task ChangeRole_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new ChangeAdminRoleCommand(99, AdminRole.OrderManager), _ct));
    }

    // ─── ActivateAdminCommand ─────────────────────────────────────────────────

    /// <summary>فعال‌سازی مدیر غیرفعال باید وضعیت را Active کند</summary>
    [Fact]
    public async Task Activate_When_AdminInactive_Should_Activate()
    {
        var admin = MakeAdmin();
        admin.Deactivate();
        _repo.Seed(admin);

        await _sut.Handle(new ActivateAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Active, admin.Status);
    }

    /// <summary>خطا: مدیر پیدا نشد → AdminNotFoundException</summary>
    [Fact]
    public async Task Activate_When_AdminNotFound_Should_Throw_AdminNotFoundException()
    {
        await Assert.ThrowsAsync<AdminNotFoundException>(
            () => _sut.Handle(new ActivateAdminCommand(99), _ct));
    }

    // ─── DeactivateAdminCommand ───────────────────────────────────────────────

    /// <summary>غیرفعال‌سازی مدیر Active باید وضعیت را Inactive کند</summary>
    [Fact]
    public async Task Deactivate_When_AdminActive_Should_Deactivate()
    {
        var admin = MakeAdmin();
        _repo.Seed(admin);

        await _sut.Handle(new DeactivateAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Inactive, admin.Status);
    }

    // ─── SuspendAdminCommand ──────────────────────────────────────────────────

    /// <summary>تعلیق مدیر باید وضعیت را Suspended کند</summary>
    [Fact]
    public async Task Suspend_When_AdminExists_Should_Suspend()
    {
        var admin = MakeAdmin();
        _repo.Seed(admin);

        await _sut.Handle(new SuspendAdminCommand(1), _ct);

        Assert.Equal(AdminStatus.Suspended, admin.Status);
    }

    // ─── RequestReportCommand ─────────────────────────────────────────────────

    /// <summary>درخواست گزارش باید گزارش Pending به مدیر اضافه کند</summary>
    [Fact]
    public async Task RequestReport_When_AdminExists_Should_AddPendingReport()
    {
        var admin = MakeAdmin();
        _repo.Seed(admin);

        await _sut.Handle(new RequestReportCommand(1, 10, ReportType.Sales), _ct);

        Assert.Single(admin.Reports);
        Assert.Equal(ReportStatus.Pending, admin.Reports.First().Status);
    }

    // ─── CompleteReportCommand ────────────────────────────────────────────────

    /// <summary>تکمیل گزارش باید وضعیت گزارش را Completed کند</summary>
    [Fact]
    public async Task CompleteReport_When_ReportExists_Should_MarkCompleted()
    {
        var admin  = MakeAdmin();
        var report = admin.RequestReport(new CreateReportArg(10, admin.Id, ReportType.Sales));
        _repo.Seed(admin);

        await _sut.Handle(new CompleteReportCommand(1, report.Id, "/reports/file.pdf"), _ct);

        Assert.Equal(ReportStatus.Completed, report.Status);
    }

    // ─── FailReportCommand ────────────────────────────────────────────────────

    /// <summary>شکست گزارش باید وضعیت گزارش را Failed کند</summary>
    [Fact]
    public async Task FailReport_When_ReportExists_Should_MarkFailed()
    {
        var admin  = MakeAdmin();
        var report = admin.RequestReport(new CreateReportArg(10, admin.Id, ReportType.Sales));
        _repo.Seed(admin);

        await _sut.Handle(new FailReportCommand(1, report.Id, "خطای سرور"), _ct);

        Assert.Equal(ReportStatus.Failed, report.Status);
        Assert.Equal("خطای سرور",        report.FailReason);
    }
}
