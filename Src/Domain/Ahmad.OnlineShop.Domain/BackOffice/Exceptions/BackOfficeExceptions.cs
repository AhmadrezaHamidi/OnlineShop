using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Exceptions;



public sealed class AdminNotFoundException : BusinessException
{
    public AdminNotFoundException()
        : base("کاربر ادمین یافت نشد.") { }
}

public sealed class AdminInvalidEmailException : BusinessException
{
    public AdminInvalidEmailException()
        : base("ایمیل ادمین نامعتبر است.") { }
}

public sealed class AdminInvalidNameException : BusinessException
{
    public AdminInvalidNameException()
        : base("نام کامل ادمین الزامی است.") { }
}

public sealed class AdminAlreadyActiveException : BusinessException
{
    public AdminAlreadyActiveException()
        : base("ادمین قبلاً فعال است.") { }
}

public sealed class AdminAlreadyInactiveException : BusinessException
{
    public AdminAlreadyInactiveException()
        : base("ادمین قبلاً غیرفعال است.") { }
}

public sealed class ReportNotFoundException : BusinessException
{
    public ReportNotFoundException()
        : base("گزارش مورد نظر یافت نشد.") { }
}

public sealed class ReportAlreadyCompletedException : BusinessException
{
    public ReportAlreadyCompletedException()
        : base("گزارش قبلاً تکمیل شده است.") { }
}

public sealed class AuditInvalidActionException : BusinessException
{
    public AuditInvalidActionException()
        : base("عملیات لاگ审计 الزامی است.") { }
}

public sealed class AuditInvalidEntityException : BusinessException
{
    public AuditInvalidEntityException()
        : base("نوع موجودیت لاگ审计 الزامی است.") { }
}