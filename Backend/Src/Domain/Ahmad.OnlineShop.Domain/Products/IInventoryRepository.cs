using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Products;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductId(long productId, CancellationToken token);

    Task Add(Inventory inventory, CancellationToken token);

    Task Update(Inventory inventory, CancellationToken token);
}
