/// <summary>
/// تست‌های Entity دسته‌بندی (Category)
/// پوشش‌دهنده: ایجاد، تغییر نام، تغییر والد
/// خطاهای تست‌شده: EmptyCategoryNameException | CategoryCircularReferenceException
/// </summary>
namespace Ahmad.OnlineShop.Domain.Products.Tests;

public class CategoryTests
{
    // ── Constructor ──────────────────────────────────────────────────────────

    /// <summary>سازنده باید نام و شناسه والد را ست کند</summary>
    [Fact]
    public void Constructor_Should_Set_Name_And_ParentId()
    {
        var category = new Category(1, "Electronics", null);

        Assert.Equal(1,             category.Id);
        Assert.Equal("Electronics", category.Name);
        Assert.Null(category.ParentId);
    }

    /// <summary>سازنده با والد باید ParentId را ست کند</summary>
    [Fact]
    public void Constructor_With_ParentId_Should_Set_ParentId()
    {
        var category = new Category(2, "Phones", 1);

        Assert.Equal(1, category.ParentId);
    }

    /// <summary>خطا: نام خالی → EmptyCategoryNameException</summary>
    [Fact]
    public void Constructor_When_EmptyName_Should_Throw_EmptyCategoryNameException()
    {
        Assert.Throws<EmptyCategoryNameException>(() => new Category(1, "", null));
    }

    /// <summary>خطا: نام فضای خالی → EmptyCategoryNameException</summary>
    [Fact]
    public void Constructor_When_WhitespaceName_Should_Throw_EmptyCategoryNameException()
    {
        Assert.Throws<EmptyCategoryNameException>(() => new Category(1, "   ", null));
    }

    // ── Rename ───────────────────────────────────────────────────────────────

    /// <summary>تغییر نام باید نام را آپدیت کند</summary>
    [Fact]
    public void Rename_Should_Update_Name()
    {
        var category = new Category(1, "Old Name", null);
        category.Rename("New Name");

        Assert.Equal("New Name", category.Name);
    }

    /// <summary>خطا: نام جدید خالی → EmptyCategoryNameException</summary>
    [Fact]
    public void Rename_When_EmptyName_Should_Throw_EmptyCategoryNameException()
    {
        var category = new Category(1, "Electronics", null);

        Assert.Throws<EmptyCategoryNameException>(() => category.Rename(""));
    }

    /// <summary>خطا: نام جدید فضای خالی → EmptyCategoryNameException</summary>
    [Fact]
    public void Rename_When_WhitespaceName_Should_Throw_EmptyCategoryNameException()
    {
        var category = new Category(1, "Electronics", null);

        Assert.Throws<EmptyCategoryNameException>(() => category.Rename("  "));
    }

    // ── ChangeParent ─────────────────────────────────────────────────────────

    /// <summary>تغییر والد باید ParentId را آپدیت کند</summary>
    [Fact]
    public void ChangeParent_Should_Update_ParentId()
    {
        var category = new Category(1, "Electronics", null);
        category.ChangeParent(5);

        Assert.Equal(5, category.ParentId);
    }

    /// <summary>تغییر والد به null باید ParentId را پاک کند</summary>
    [Fact]
    public void ChangeParent_To_Null_Should_Clear_ParentId()
    {
        var category = new Category(1, "Electronics", 5);
        category.ChangeParent(null);

        Assert.Null(category.ParentId);
    }

    /// <summary>خطا: دسته‌بندی نمی‌تواند والد خودش باشد → CategoryCircularReferenceException</summary>
    [Fact]
    public void ChangeParent_To_Self_Should_Throw_CategoryCircularReferenceException()
    {
        var category = new Category(1, "Electronics", null);

        Assert.Throws<CategoryCircularReferenceException>(() => category.ChangeParent(1));
    }
}
