namespace Ahmad.OnlineShop.Rest.EndPoints.Invoice;

public static class InvoiceConstants
{
    public static class Routes
    {
        public const string BaseRoute      = "api/v{version:apiVersion}/invoices";
        public const string GetById        = "/{id}";
        public const string GetByOrder     = "/order/{orderId}";
        public const string GetMyInvoices  = "/my";
    }

    public static class Names
    {
        public const string GetById       = "GetInvoiceById";
        public const string GetByOrder    = "GetInvoiceByOrder";
        public const string GetMyInvoices = "GetMyInvoices";
    }

    public static class Docs
    {
        public static class GetById
        {
            public const string Summary = "دریافت فاکتور با شناسه";
        }
        public static class GetByOrder
        {
            public const string Summary = "دریافت فاکتور سفارش";
        }
        public static class GetMyInvoices
        {
            public const string Summary = "فاکتورهای من";
        }
    }
}
