using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Domain.Products.Enums
{

    public enum ProductStatus
    {
        Active,
        Inactive,
        Archived
    }

    public enum ImageType
    {
        Primary,    // تصویر اصلی (thumbnail)
        Gallery,    // گالری
        Banner      // بنر
    }
}
