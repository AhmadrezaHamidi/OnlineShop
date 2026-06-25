using Ahmad.OnlineShop.Domain.Products;
using AhmadBase.Persistence.NHiLoHelper;
using AhmadBase.Doamin;
using Microsoft.EntityFrameworkCore;


namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IHiLoIdGenerator _hiLoGenerator;

    public ProductRepository(ApplicationDbContext context, IHiLoIdGenerator hiLoGenerator)
    {
        _context = context;
        _hiLoGenerator = hiLoGenerator;
    }

    public async Task<Product?> Get(long id, CancellationToken token)
        => await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, token);

    public async Task<(List<Product> Items, int TotalCount)> GetListAsync(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        long? categoryId = null,
        CancellationToken token = default)
    {
        var query = _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) ||
                                   (p.Description != null && p.Description.Contains(search)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync(token);

        var items = await query
            .OrderByDescending(p => p.CreationTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(token);

        return (items, totalCount);
    }

    public async Task Add(Product product, CancellationToken token)
        => await _context.Products.AddAsync(product, token);

    public Task Update(Product product, CancellationToken token)
    {
        _context.Products.Update(product);
        return _context.SaveChangesAsync(token);
    }

    public long GetNextId()
        => _hiLoGenerator.GetNextId<Product>();
}
