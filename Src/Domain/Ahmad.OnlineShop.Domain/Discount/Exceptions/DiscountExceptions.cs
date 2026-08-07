using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Exceptions;

public sealed class DiscountNotFoundException : BusinessException
{
    public DiscountNotFoundException() : base(Resources.Message.DiscountNotFoundException) { }
}

public sealed class InvalidDiscountCodeException : BusinessException
{
    public InvalidDiscountCodeException() : base(Resources.Message.InvalidDiscountCodeException) { }
}

public sealed class InvalidDiscountValueException : BusinessException
{
    public InvalidDiscountValueException() : base(Resources.Message.InvalidDiscountValueException) { }
}

public sealed class DiscountCodeAlreadyExistsException : BusinessException
{
    public DiscountCodeAlreadyExistsException() : base(Resources.Message.DiscountCodeAlreadyExistsException) { }
}

public sealed class DiscountExpiredException : BusinessException
{
    public DiscountExpiredException() : base(Resources.Message.DiscountExpiredException) { }
}

public sealed class DiscountMaxUsageReachedException : BusinessException
{
    public DiscountMaxUsageReachedException() : base(Resources.Message.DiscountMaxUsageReachedException) { }
}

public sealed class DiscountMinOrderAmountNotMetException : BusinessException
{
    public DiscountMinOrderAmountNotMetException() : base(Resources.Message.DiscountMinOrderAmountNotMetException) { }
}

public sealed class DiscountNotActiveException : BusinessException
{
    public DiscountNotActiveException() : base(Resources.Message.DiscountNotActiveException) { }
}

public sealed class DiscountAlreadyActiveException : BusinessException
{
    public DiscountAlreadyActiveException() : base(Resources.Message.DiscountAlreadyActiveException) { }
}

// ── Package ───────────────────────────────────────────────────────────────────

public sealed class ProductPackageNotFoundException : BusinessException
{
    public ProductPackageNotFoundException() : base(Resources.Message.ProductPackageNotFoundException) { }
}

public sealed class InvalidPackageTitleException : BusinessException
{
    public InvalidPackageTitleException() : base(Resources.Message.InvalidPackageTitleException) { }
}

public sealed class InvalidPackageDiscountException : BusinessException
{
    public InvalidPackageDiscountException() : base(Resources.Message.InvalidPackageDiscountException) { }
}

public sealed class InvalidPackageDateRangeException : BusinessException
{
    public InvalidPackageDateRangeException() : base(Resources.Message.InvalidPackageDateRangeException) { }
}

public sealed class PackageItemAlreadyExistsException : BusinessException
{
    public PackageItemAlreadyExistsException() : base(Resources.Message.PackageItemAlreadyExistsException) { }
}

public sealed class PackageItemNotFoundException : BusinessException
{
    public PackageItemNotFoundException() : base(Resources.Message.PackageItemNotFoundException) { }
}

public sealed class PackageNotActiveException : BusinessException
{
    public PackageNotActiveException() : base(Resources.Message.PackageNotActiveException) { }
}

public sealed class PackageAlreadyActiveException : BusinessException
{
    public PackageAlreadyActiveException() : base(Resources.Message.PackageAlreadyActiveException) { }
}

public sealed class PackageExpiredException : BusinessException
{
    public PackageExpiredException() : base(Resources.Message.PackageExpiredException) { }
}
