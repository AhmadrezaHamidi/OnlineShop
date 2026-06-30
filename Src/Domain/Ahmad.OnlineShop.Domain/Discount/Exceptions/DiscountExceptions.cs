using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Discount.Exceptions;

public sealed class DiscountNotFoundException : BusinessException
{
    public DiscountNotFoundException() : base("کد تخفیف پیدا نشد.") { }
}

public sealed class InvalidDiscountCodeException : BusinessException
{
    public InvalidDiscountCodeException() : base("کد تخفیف نامعتبر است.") { }
}

public sealed class InvalidDiscountValueException : BusinessException
{
    public InvalidDiscountValueException() : base("مقدار تخفیف باید بزرگ‌تر از صفر باشد.") { }
}

public sealed class DiscountCodeAlreadyExistsException : BusinessException
{
    public DiscountCodeAlreadyExistsException() : base("این کد تخفیف قبلاً ثبت شده است.") { }
}

public sealed class DiscountExpiredException : BusinessException
{
    public DiscountExpiredException() : base("مدت اعتبار این کد تخفیف به پایان رسیده است.") { }
}

public sealed class DiscountMaxUsageReachedException : BusinessException
{
    public DiscountMaxUsageReachedException() : base("این کد تخفیف به حداکثر تعداد استفاده رسیده است.") { }
}

public sealed class DiscountMinOrderAmountNotMetException : BusinessException
{
    public DiscountMinOrderAmountNotMetException() : base("مبلغ سفارش از حداقل مبلغ لازم برای این تخفیف کمتر است.") { }
}

public sealed class DiscountNotActiveException : BusinessException
{
    public DiscountNotActiveException() : base("این کد تخفیف فعال نیست.") { }
}

public sealed class DiscountAlreadyActiveException : BusinessException
{
    public DiscountAlreadyActiveException() : base("این تخفیف در حال حاضر فعال است.") { }
}

// ── Package ───────────────────────────────────────────────────────────────────

public sealed class ProductPackageNotFoundException : BusinessException
{
    public ProductPackageNotFoundException() : base("پکیج محصول پیدا نشد.") { }
}

public sealed class InvalidPackageTitleException : BusinessException
{
    public InvalidPackageTitleException() : base("عنوان پکیج نمی‌تواند خالی باشد.") { }
}

public sealed class InvalidPackageDiscountException : BusinessException
{
    public InvalidPackageDiscountException() : base("درصد تخفیف پکیج باید بین ۱ تا ۱۰۰ باشد.") { }
}

public sealed class InvalidPackageDateRangeException : BusinessException
{
    public InvalidPackageDateRangeException() : base("تاریخ شروع باید قبل از تاریخ پایان باشد.") { }
}

public sealed class PackageItemAlreadyExistsException : BusinessException
{
    public PackageItemAlreadyExistsException() : base("این محصول از قبل در پکیج موجود است.") { }
}

public sealed class PackageItemNotFoundException : BusinessException
{
    public PackageItemNotFoundException() : base("آیتم مورد نظر در این پکیج یافت نشد.") { }
}

public sealed class PackageNotActiveException : BusinessException
{
    public PackageNotActiveException() : base("این پکیج فعال نیست.") { }
}

public sealed class PackageAlreadyActiveException : BusinessException
{
    public PackageAlreadyActiveException() : base("این پکیج در حال حاضر فعال است.") { }
}

public sealed class PackageExpiredException : BusinessException
{
    public PackageExpiredException() : base("مدت اعتبار این پکیج به پایان رسیده است.") { }
}
