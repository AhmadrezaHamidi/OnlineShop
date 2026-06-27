using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ahmad.OnlineShop.Rest.EndPoints.Product;

public static class ProductConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Product";

        // Product
        public const string GetProducts = "/";
        public const string GetProduct = "/{id}";
        public const string CreateProduct = "/";
        public const string UpdateProduct = "/{id}";
        public const string ChangePrice = "/{id}/Price";
        public const string ActivateProduct = "/{id}/Activate";
        public const string DeactivateProduct = "/{id}/Deactivate";
        public const string ArchiveProduct = "/{id}/Archive";

        // Inventory
        public const string GetInventory = "/{id}/Inventory";
        public const string ReplenishStock = "/{id}/Inventory/Replenish";
        public const string ReserveStock = "/{id}/Inventory/Reserve";
        public const string ReleaseStock = "/{id}/Inventory/Release";
        public const string ConfirmStock = "/{id}/Inventory/Confirm";

        // Image
        public const string GetImages       = "/{id}/Images";
        public const string AddImage        = "/{id}/Images";
        public const string RemoveImage     = "/{id}/Images/{imageId}";
        public const string SetPrimaryImage = "/{id}/Images/{imageId}/Primary";
        public const string ReorderImage    = "/{id}/Images/{imageId}/Reorder";

        // Category
        public const string GetCategories  = "/Categories";
        public const string CreateCategory = "/Categories";
        public const string UpdateCategory = "/Categories/{id}";
    }

    public static class Names
    {
        public const string GetProducts = "GetProducts";
        public const string GetProduct = "GetProduct";
        public const string CreateProduct = "CreateProduct";
        public const string UpdateProduct = "UpdateProduct";
        public const string ChangePrice = "ChangeProductPrice";
        public const string ActivateProduct = "ActivateProduct";
        public const string DeactivateProduct = "DeactivateProduct";
        public const string ArchiveProduct = "ArchiveProduct";

        public const string GetInventory = "GetInventory";
        public const string ReplenishStock = "ReplenishStock";
        public const string ReserveStock = "ReserveStock";
        public const string ReleaseStock = "ReleaseStock";
        public const string ConfirmStock = "ConfirmStock";

        public const string GetImages       = "GetProductImages";
        public const string AddImage        = "AddProductImage";
        public const string RemoveImage     = "RemoveProductImage";
        public const string SetPrimaryImage = "SetPrimaryImage";
        public const string ReorderImage    = "ReorderImage";

        public const string GetCategories  = "GetCategories";
        public const string CreateCategory = "CreateCategory";
        public const string UpdateCategory = "UpdateCategory";
    }

    public static class Docs
    {
        public static class GetProducts
        {
            public const string Summary = "دریافت لیست محصولات";
            public const string Description = "این سرویس لیست محصولات را با امکان فیلتر و صفحه‌بندی برمی‌گرداند";
        }

        public static class GetProduct
        {
            public const string Summary = "دریافت جزئیات محصول";
            public const string Description = "این سرویس اطلاعات کامل یک محصول خاص را بر اساس شناسه برمی‌گرداند";
        }

        public static class CreateProduct
        {
            public const string Summary = "ایجاد محصول جدید";
            public const string Description = "این سرویس برای ثبت محصول جدید در سیستم استفاده می‌شود";
        }

        public static class UpdateProduct
        {
            public const string Summary = "به‌روزرسانی محصول";
            public const string Description = "این سرویس اطلاعات یک محصول موجود را ویرایش می‌کند";
        }

        public static class ChangePrice
        {
            public const string Summary = "تغییر قیمت محصول";
            public const string Description = "این سرویس برای تغییر قیمت یک محصول خاص استفاده می‌شود";
        }

        public static class ActivateProduct
        {
            public const string Summary = "فعال‌سازی محصول";
            public const string Description = "این سرویس محصول را به حالت فعال تغییر می‌دهد";
        }

        public static class DeactivateProduct
        {
            public const string Summary = "غیرفعال‌سازی محصول";
            public const string Description = "این سرویس محصول را به حالت غیرفعال تغییر می‌دهد";
        }

        public static class ArchiveProduct
        {
            public const string Summary = "بایگانی محصول";
            public const string Description = "این سرویس محصول را به حالت بایگانی منتقل می‌کند";
        }

        // Inventory
        public static class GetInventory
        {
            public const string Summary = "دریافت موجودی محصول";
            public const string Description = "این سرویس وضعیت موجودی فعلی یک محصول را برمی‌گرداند";
        }

        public static class ReplenishStock
        {
            public const string Summary = "افزایش موجودی (تامین)";
            public const string Description = "این سرویس برای افزایش موجودی انبار یک محصول استفاده می‌شود";
        }

        public static class ReserveStock
        {
            public const string Summary = "رزرو موجودی";
            public const string Description = "این سرویس موجودی را برای سفارش یا عملیات خاص رزرو می‌کند";
        }

        public static class ReleaseStock
        {
            public const string Summary = "آزادسازی موجودی";
            public const string Description = "این سرویس موجودی رزرو شده را آزاد می‌کند";
        }

        public static class ConfirmStock
        {
            public const string Summary = "تایید موجودی";
            public const string Description = "این سرویس موجودی رزرو شده را به صورت قطعی کسر می‌کند";
        }

        // Image
        public static class AddImage
        {
            public const string Summary = "افزودن تصویر به محصول";
            public const string Description = "این سرویس تصویر جدید به گالری محصول اضافه می‌کند";
        }

        public static class RemoveImage
        {
            public const string Summary = "حذف تصویر محصول";
            public const string Description = "این سرویس یک تصویر خاص از محصول را حذف می‌کند";
        }

        public static class SetPrimaryImage
        {
            public const string Summary = "تنظیم تصویر اصلی";
            public const string Description = "این سرویس یک تصویر را به عنوان تصویر اصلی محصول تنظیم می‌کند";
        }

        public static class GetImages
        {
            public const string Summary     = "دریافت تصاویر محصول";
            public const string Description = "این سرویس تمام تصاویر یک محصول را برمی‌گرداند";
        }

        public static class ReorderImage
        {
            public const string Summary     = "تغییر ترتیب تصاویر";
            public const string Description = "این سرویس ترتیب نمایش تصاویر محصول را تغییر می‌دهد";
        }

        public static class GetCategories
        {
            public const string Summary     = "لیست دسته‌بندی‌ها";
            public const string Description = "این سرویس تمام دسته‌بندی‌های موجود را برمی‌گرداند";
        }

        public static class CreateCategory
        {
            public const string Summary     = "ایجاد دسته‌بندی";
            public const string Description = "این سرویس یک دسته‌بندی جدید ایجاد می‌کند";
        }

        public static class UpdateCategory
        {
            public const string Summary     = "ویرایش دسته‌بندی";
            public const string Description = "این سرویس اطلاعات یک دسته‌بندی را ویرایش می‌کند";
        }
    }
}
