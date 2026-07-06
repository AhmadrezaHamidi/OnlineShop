using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Bnpl.Exceptions;

// Credit
public sealed class CreditLimitNotFoundException : BusinessException
{
    public CreditLimitNotFoundException() : base(Resources.Message.CreditLimitNotFoundException) { }
}

public sealed class InsufficientCreditException : BusinessException
{
    public InsufficientCreditException() : base(Resources.Message.InsufficientCreditException) { }
}

public sealed class InvalidCreditLimitAmountException : BusinessException
{
    public InvalidCreditLimitAmountException() : base(Resources.Message.InvalidCreditLimitAmountException) { }
}

public sealed class NegativeCreditException : BusinessException
{
    public NegativeCreditException() : base(Resources.Message.NegativeCreditException) { }
}

// Contract
public sealed class BnplContractNotFoundException : BusinessException
{
    public BnplContractNotFoundException() : base(Resources.Message.BnplContractNotFoundException) { }
}

public sealed class BnplContractAlreadyCancelledException : BusinessException
{
    public BnplContractAlreadyCancelledException() : base(Resources.Message.BnplContractAlreadyCancelledException) { }
}

public sealed class BnplContractAlreadyCompletedException : BusinessException
{
    public BnplContractAlreadyCompletedException() : base(Resources.Message.BnplContractAlreadyCompletedException) { }
}

public sealed class BnplContractNotActiveException : BusinessException
{
    public BnplContractNotActiveException() : base(Resources.Message.BnplContractNotActiveException) { }
}

public sealed class InvalidInstallmentCountException : BusinessException
{
    public InvalidInstallmentCountException() : base(Resources.Message.InvalidInstallmentCountException) { }
}

public sealed class InvalidTotalAmountException : BusinessException
{
    public InvalidTotalAmountException() : base(Resources.Message.InvalidTotalAmountException) { }
}

// Installment
public sealed class InstallmentNotFoundException : BusinessException
{
    public InstallmentNotFoundException() : base(Resources.Message.InstallmentNotFoundException) { }
}

public sealed class InstallmentAlreadyPaidException : BusinessException
{
    public InstallmentAlreadyPaidException() : base(Resources.Message.InstallmentAlreadyPaidException) { }
}

public sealed class InstallmentInvalidAmountException : BusinessException
{
    public InstallmentInvalidAmountException() : base(Resources.Message.InstallmentInvalidAmountException) { }
}
