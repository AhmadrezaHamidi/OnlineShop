// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// نقشه راه: این فایل در نسخه AhmadBase v5.1.0 حذف خواهد شد.
// وقتی NuGet آپدیت شد:
//   1. این فایل را حذف کنید
//   2. PackageReference AhmadBase.Application.Query را به 5.1.0 ارتقا دهید
//   3. using AhmadBase.Application.Query; را در GlobalUsings.cs اضافه کنید
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

namespace AhmadBase.Application.Query;

/// <summary>
/// نتیجه صفحه‌بندی‌شده — جایگزین QueryPagedResult، BackOfficePagedResult و IdentityPagedResult.
/// در نسخه NuGet 5.1.0 این class به پکیج AhmadBase.Application.Query منتقل خواهد شد.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int              TotalCount,
    int              Page,
    int              PageSize)
{
    public int  TotalPages      => PageSize > 0
                                    ? (int)Math.Ceiling((double)TotalCount / PageSize)
                                    : 0;
    public bool HasNextPage     => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public static PagedResult<T> Empty(int page = 1, int pageSize = 20)
        => new(Array.Empty<T>(), 0, page, pageSize);

    public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector)
        => new(Items.Select(selector).ToList(), TotalCount, Page, PageSize);
}
