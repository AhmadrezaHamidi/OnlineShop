namespace Ahmad.OnlineShop.Domain.BackOffice.Enums;

public enum AdminRole
{
    SuperAdmin,
    ProductManager,
    OrderManager,
    FinanceManager,
    SupportAgent
}

public enum AdminStatus
{
    Active,
    Inactive,
    Suspended
}

public enum ReportType
{
    Sales,
    Inventory,
    UserActivity,
    BnplOverdue,
    Financial
}

public enum ReportStatus
{
    Pending,
    Generating,
    Completed,
    Failed
}
