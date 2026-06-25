using Ahmad.OnlineShop.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImage?> GetById(Guid imageId, CancellationToken token)
        => await _context.ProductImages
            .FirstOrDefaultAsync(i => i.Id == imageId, token);

    public async Task<List<ProductImage>> GetByProductId(long productId, CancellationToken token)
        => await _context.ProductImages
            .Where(i => i.ProductId == productId)
            .OrderBy(i => i.SortOrder)
            .ToListAsync(token);

    public async Task Add(ProductImage image, CancellationToken token)
        => await _context.ProductImages.AddAsync(image, token);

    public Task Update(ProductImage image, CancellationToken token)
    {
        _context.ProductImages.Update(image);
        return _context.SaveChangesAsync(token);
    }

    public async Task Remove(Guid imageId, CancellationToken token)
    {
        var image = await GetById(imageId, token);
        if (image != null)
        {
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync(token);
        }
    }
}
