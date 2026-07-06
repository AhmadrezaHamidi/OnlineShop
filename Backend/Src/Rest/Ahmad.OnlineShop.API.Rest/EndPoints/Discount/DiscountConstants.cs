namespace Ahmad.OnlineShop.Rest.EndPoints.Discount;

public static class DiscountConstants
{
    public static class Routes
    {
        public const string Base         = "api/v{version:apiVersion}/Discounts";
        public const string ById         = "{id:long}";
        public const string ByCode       = "code/{code}";
        public const string Activate     = "{id:long}/activate";
        public const string Deactivate   = "{id:long}/deactivate";
        public const string Apply        = "apply";

        public const string PackagesBase    = "api/v{version:apiVersion}/Packages";
        public const string PackageById     = "{id:long}";
        public const string PackageItems    = "{id:long}/items";
        public const string PackageActivate = "{id:long}/activate";
        public const string PackageDeactivate = "{id:long}/deactivate";
    }

    public static class Tags
    {
        public const string Discount = "Discounts";
        public const string Package  = "Packages";
    }
}
