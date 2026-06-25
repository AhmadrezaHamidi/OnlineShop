using Ahmad.OnlineShop.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.OnlineShop.Persistence.EF.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByProductId(long productId, CancellationToken token)
        => await _context.Set<Inventory>()
            .FirstOrDefaultAsync(i => i.ProductId == productId, token);

    public async Task Add(Inventory inventory, CancellationToken token)
        => await _context.Set<Inventory>().AddAsync(inventory, token);

    public Task Update(Inventory inventory, CancellationToken token)
    {
        _context.Set<Inventory>().Update(inventory);
        return _context.SaveChangesAsync(token);
    }
}
