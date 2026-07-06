using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Products;

public interface IProductImageRepository
{
    Task<ProductImage?> GetById(Guid imageId, CancellationToken token);

    Task<List<ProductImage>> GetByProductId(long productId, CancellationToken token);

    Task Add(ProductImage image, CancellationToken token);

    Task Update(ProductImage image, CancellationToken token);

    Task Remove(Guid imageId, CancellationToken token);
}
