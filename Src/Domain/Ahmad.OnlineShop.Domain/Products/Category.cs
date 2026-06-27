using Ahmad.OnlineShop.Domain.Products.Exceptions;
using AhmadBase.Doamin;

namespace Ahmad.OnlineShop.Domain.Products;

public sealed class Category : TEntity<long>
{
    public string Name     { get; private set; } = string.Empty;
    public long?  ParentId { get; private set; }

    private Category() { }

    public Category(long id, string name, long? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EmptyCategoryNameException();

        Id       = id;
        Name     = name;
        ParentId = parentId;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EmptyCategoryNameException();

        Name = name;
    }

    public void ChangeParent(long? parentId)
    {
        if (parentId == Id)
            throw new CategoryCircularReferenceException();

        ParentId = parentId;
    }
}
