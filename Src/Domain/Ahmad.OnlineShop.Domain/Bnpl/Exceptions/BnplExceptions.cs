using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Exceptions;

// Credit
public sealed class CreditLimitNotFoundException : BusinessException
{
    public CreditLimitNotFoundException() : base("سقف اعتبار کاربر یافت نشد.") { }
}

public sealed class InsufficientCreditException : BusinessException
{
    public InsufficientCreditException() : base("سقف اعتبار کافی نیست.") { }
}

public sealed class InvalidCreditLimitAmountException : BusinessException
{
    public InvalidCreditLimitAmountException() : base("مقدار اعتبار باید بزرگتر از صفر باشد.") { }
}

public sealed class NegativeCreditException : BusinessException
{
    public NegativeCreditException() : base("اعتبار استفاده‌شده نمی‌تواند منفی شود.") { }
}

// Contract
public sealed class BnplContractNotFoundException : BusinessException
{
    public BnplContractNotFoundException() : base("قرارداد BNPL یافت نشد.") { }
}

public sealed class BnplContractAlreadyCancelledException : BusinessException
{
    public BnplContractAlreadyCancelledException() : base("قرارداد قبلاً لغو شده است.") { }
}

public sealed class BnplContractAlreadyCompletedException : BusinessException
{
    public BnplContractAlreadyCompletedException() : base("قرارداد قبلاً تکمیل شده است.") { }
}

public sealed class BnplContractNotActiveException : BusinessException
{
    public BnplContractNotActiveException() : base("قرارداد در وضعیت فعال نیست.") { }
}

public sealed class InvalidInstallmentCountException : BusinessException
{
    public InvalidInstallmentCountException() : base("تعداد اقساط باید بین ۱ تا ۴۸ باشد.") { }
}

public sealed class InvalidTotalAmountException : BusinessException
{
    public InvalidTotalAmountException() : base("مبلغ کل قرارداد باید بزرگتر از صفر باشد.") { }
}

// Installment
public sealed class InstallmentNotFoundException : BusinessException
{
    public InstallmentNotFoundException() : base("قسط مورد نظر یافت نشد.") { }
}

public sealed class InstallmentAlreadyPaidException : BusinessException
{
    public InstallmentAlreadyPaidException() : base("این قسط قبلاً پرداخت شده است.") { }
}

public sealed class InstallmentInvalidAmountException : BusinessException
{
    public InstallmentInvalidAmountException() : base("مبلغ قسط باید بزرگتر از صفر باشد.") { }
}
