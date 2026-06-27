using Ahmad.OnlineShop.Domain.Products;
using AhmadBase.Persistence.NHiLoHelper;
using Ahmad.OnlineShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Ahmad.OnlineShop.Domain.Products.Enums;


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

    public async Task AddAsync(Product product, CancellationToken token)
    {
        await _context.Products.AddAsync(product, token);
        await _context.SaveChangesAsync(token);
    }

    public Task UpdateAsync(Product product, CancellationToken token)
    {
        _context.Products.Update(product);
        return _context.SaveChangesAsync(token);
    }

    public long GetNextId()
        => _hiLoGenerator.GetNextId<Product>();


    public async Task<(List<Product> Items, int Total)> GetListAsync(int page, int pageSize,
        string? search, long? categoryId, ProductStatus? status, CancellationToken token = default)
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
}
