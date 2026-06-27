using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.BackOffice.Exceptions;

public sealed class AdminNotFoundException : BusinessException
{
    public AdminNotFoundException()
        : base(Resources.Message.AdminNotFoundException) { }
}

public sealed class AdminEmailAlreadyExistsException : BusinessException
{
    public AdminEmailAlreadyExistsException()
        : base(Resources.Message.AdminEmailAlreadyExistsException) { }
}

public sealed class AdminInvalidEmailException : BusinessException
{
    public AdminInvalidEmailException()
        : base(Resources.Message.AdminInvalidEmailException) { }
}

public sealed class AdminInvalidNameException : BusinessException
{
    public AdminInvalidNameException()
        : base(Resources.Message.AdminInvalidNameException) { }
}

public sealed class AdminAlreadyActiveException : BusinessException
{
    public AdminAlreadyActiveException()
        : base(Resources.Message.AdminAlreadyActiveException) { }
}

public sealed class AdminAlreadyInactiveException : BusinessException
{
    public AdminAlreadyInactiveException()
        : base(Resources.Message.AdminAlreadyInactiveException) { }
}

public sealed class ReportNotFoundException : BusinessException
{
    public ReportNotFoundException()
        : base(Resources.Message.ReportNotFoundException) { }
}

public sealed class ReportAlreadyCompletedException : BusinessException
{
    public ReportAlreadyCompletedException()
        : base(Resources.Message.ReportAlreadyCompletedException) { }
}

public sealed class AuditInvalidActionException : BusinessException
{
    public AuditInvalidActionException()
        : base(Resources.Message.AuditInvalidActionException) { }
}

public sealed class AuditInvalidEntityException : BusinessException
{
    public AuditInvalidEntityException()
        : base(Resources.Message.AuditInvalidEntityException) { }
}
