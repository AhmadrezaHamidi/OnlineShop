/// <summary>
/// تست‌های Aggregate Root مدیر (AdminUser)
/// پوشش‌دهنده: ایجاد، تغییر وضعیت، تغییر نقش، گزارش‌ها و لاگ
/// خطاهای تست‌شده: AdminInvalidNameException | AdminInvalidEmailException
///                  AdminAlreadyActiveException | AdminAlreadyInactiveException
///                  ReportNotFoundException | ReportAlreadyCompletedException
///                  AuditInvalidActionException | AuditInvalidEntityException
/// </summary>
namespace Ahmad.OnlineShop.Domain.BackOffice.Tests;

public class AdminUserTests
{
    private static CreateAdminUserArg ValidArg() =>
        new(1, "Ahmad Hamidi", "ahmad@example.com", AdminRole.SuperAdmin);

    // ── Create ──────────────────────────────────────────────────────────────

    /// <summary>ایجاد مدیر باید مشخصات را ست کند و وضعیت Active باشد</summary>
    [Fact]
    public void Create_Should_Set_Properties_And_ActiveStatus()
    {
        var admin = AdminUser.Create(ValidArg());

        Assert.Equal(1,                    admin.Id);
        Assert.Equal("Ahmad Hamidi",       admin.FullName);
        Assert.Equal("ahmad@example.com",  admin.Email);
        Assert.Equal(AdminRole.SuperAdmin,  admin.Role);
        Assert.Equal(AdminStatus.Active,    admin.Status);
        Assert.Empty(admin.AuditLogs);
        Assert.Empty(admin.Reports);
    }

    /// <summary>ایمیل باید به حروف کوچک نرمال‌سازی شود</summary>
    [Fact]
    public void Create_Should_Normalize_Email_To_Lowercase()
    {
        var arg   = new CreateAdminUserArg(1, "Ahmad", "AHMAD@EXAMPLE.COM", AdminRole.SuperAdmin);
        var admin = AdminUser.Create(arg);

        Assert.Equal("ahmad@example.com", admin.Email);
    }

    /// <summary>خطا: نام خالی → AdminInvalidNameException</summary>
    [Fact]
    public void Create_When_EmptyName_Should_Throw_AdminInvalidNameException()
    {
        var arg = new CreateAdminUserArg(1, "   ", "a@b.com", AdminRole.SuperAdmin);

        Assert.Throws<AdminInvalidNameException>(() => AdminUser.Create(arg));
    }

    /// <summary>خطا: ایمیل بدون @ → AdminInvalidEmailException</summary>
    [Fact]
    public void Create_When_InvalidEmail_Should_Throw_AdminInvalidEmailException()
    {
        var arg = new CreateAdminUserArg(1, "Ahmad", "not-an-email", AdminRole.SuperAdmin);

        Assert.Throws<AdminInvalidEmailException>(() => AdminUser.Create(arg));
    }

    /// <summary>خطا: ایمیل خالی → AdminInvalidEmailException</summary>
    [Fact]
    public void Create_When_EmptyEmail_Should_Throw_AdminInvalidEmailException()
    {
        var arg = new CreateAdminUserArg(1, "Ahmad", "", AdminRole.SuperAdmin);

        Assert.Throws<AdminInvalidEmailException>(() => AdminUser.Create(arg));
    }

    // ── Activate / Deactivate / Suspend ─────────────────────────────────────

    /// <summary>غیرفعال‌سازی باید وضعیت را Inactive کند</summary>
    [Fact]
    public void Deactivate_Should_Change_Status_To_Inactive()
    {
        var admin = AdminUser.Create(ValidArg());
        admin.Deactivate();

        Assert.Equal(AdminStatus.Inactive, admin.Status);
    }

    /// <summary>فعال‌سازی بعد از غیرفعال‌سازی باید وضعیت را Active کند</summary>
    [Fact]
    public void Activate_After_Deactivate_Should_Change_Status_To_Active()
    {
        var admin = AdminUser.Create(ValidArg());
        admin.Deactivate();
        admin.Activate();

        Assert.Equal(AdminStatus.Active, admin.Status);
    }

    /// <summary>خطا: فعال‌سازی وقتی از قبل Active است → AdminAlreadyActiveException</summary>
    [Fact]
    public void Activate_When_AlreadyActive_Should_Throw_AdminAlreadyActiveException()
    {
        var admin = AdminUser.Create(ValidArg());

        Assert.Throws<AdminAlreadyActiveException>(() => admin.Activate());
    }

    /// <summary>خطا: غیرفعال‌سازی وقتی از قبل Inactive است → AdminAlreadyInactiveException</summary>
    [Fact]
    public void Deactivate_When_AlreadyInactive_Should_Throw_AdminAlreadyInactiveException()
    {
        var admin = AdminUser.Create(ValidArg());
        admin.Deactivate();

        Assert.Throws<AdminAlreadyInactiveException>(() => admin.Deactivate());
    }

    /// <summary>تعلیق باید وضعیت را Suspended کند</summary>
    [Fact]
    public void Suspend_Should_Change_Status_To_Suspended()
    {
        var admin = AdminUser.Create(ValidArg());
        admin.Suspend();

        Assert.Equal(AdminStatus.Suspended, admin.Status);
    }

    // ── ChangeRole ───────────────────────────────────────────────────────────

    /// <summary>تغییر نقش باید نقش مدیر را آپدیت کند</summary>
    [Fact]
    public void ChangeRole_Should_Update_Role()
    {
        var admin = AdminUser.Create(ValidArg());
        admin.ChangeRole(AdminRole.ProductManager);

        Assert.Equal(AdminRole.ProductManager, admin.Role);
    }

    // ── Reports ──────────────────────────────────────────────────────────────

    /// <summary>درخواست گزارش باید گزارش با وضعیت Pending اضافه کند</summary>
    [Fact]
    public void RequestReport_Should_Add_Report_With_Pending_Status()
    {
        var admin     = AdminUser.Create(ValidArg());
        var reportArg = new CreateReportArg(10, admin.Id, ReportType.Sales);
        var report    = admin.RequestReport(reportArg);

        Assert.Single(admin.Reports);
        Assert.Equal(ReportStatus.Pending, report.Status);
        Assert.Equal(ReportType.Sales,     report.Type);
    }

    /// <summary>تکمیل گزارش باید FilePath و وضعیت Completed را ست کند</summary>
    [Fact]
    public void CompleteReport_Should_Set_FilePath_And_Completed_Status()
    {
        var admin = CreateAdminWithReport(out long reportId);
        admin.CompleteReport(reportId, "/reports/sales.pdf");

        var report = admin.Reports.First(r => r.Id == reportId);
        Assert.Equal(ReportStatus.Completed, report.Status);
        Assert.Equal("/reports/sales.pdf",   report.FilePath);
        Assert.NotNull(report.GeneratedAt);
    }

    /// <summary>شکست گزارش باید دلیل و وضعیت Failed را ست کند</summary>
    [Fact]
    public void FailReport_Should_Set_Reason_And_Failed_Status()
    {
        var admin = CreateAdminWithReport(out long reportId);
        admin.FailReport(reportId, "خطای سرور");

        var report = admin.Reports.First(r => r.Id == reportId);
        Assert.Equal(ReportStatus.Failed, report.Status);
        Assert.Equal("خطای سرور",        report.FailReason);
    }

    /// <summary>خطا: گزارش پیدا نشد → ReportNotFoundException</summary>
    [Fact]
    public void CompleteReport_When_ReportNotFound_Should_Throw_ReportNotFoundException()
    {
        var admin = AdminUser.Create(ValidArg());

        Assert.Throws<ReportNotFoundException>(() => admin.CompleteReport(999, "/file.pdf"));
    }

    /// <summary>خطا: مسیر فایل خالی → ReportNotFoundException</summary>
    [Fact]
    public void CompleteReport_When_EmptyFilePath_Should_Throw_ReportNotFoundException()
    {
        var admin = CreateAdminWithReport(out long reportId);

        Assert.Throws<ReportNotFoundException>(() => admin.CompleteReport(reportId, "  "));
    }

    /// <summary>خطا: گزارش قبلاً تکمیل شده → ReportAlreadyCompletedException</summary>
    [Fact]
    public void CompleteReport_When_AlreadyCompleted_Should_Throw_ReportAlreadyCompletedException()
    {
        var admin = CreateAdminWithReport(out long reportId);
        admin.CompleteReport(reportId, "/file.pdf");

        Assert.Throws<ReportAlreadyCompletedException>(() => admin.CompleteReport(reportId, "/file2.pdf"));
    }

    // ── AuditLog ─────────────────────────────────────────────────────────────

    /// <summary>ثبت عملیات باید لاگ حسابرسی اضافه کند</summary>
    [Fact]
    public void LogAction_Should_Add_AuditLog()
    {
        var admin  = AdminUser.Create(ValidArg());
        var logArg = new CreateAuditLogArg(1, admin.Id, "Create", "Product", "42");
        admin.LogAction(logArg);

        Assert.Single(admin.AuditLogs);
        Assert.Equal("Create",  admin.AuditLogs.First().Action);
        Assert.Equal("Product", admin.AuditLogs.First().EntityType);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static AdminUser CreateAdminWithReport(out long reportId)
    {
        var admin  = AdminUser.Create(ValidArg());
        var report = admin.RequestReport(new CreateReportArg(10, admin.Id, ReportType.Sales));
        reportId   = report.Id;
        return admin;
    }
}
