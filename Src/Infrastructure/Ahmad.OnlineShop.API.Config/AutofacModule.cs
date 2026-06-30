using System.Data;
using AhmadBase.Config.Containers;
using AhmadBase.External.Service;
using AhmadBase.External.Service.Payment;
using AhmadBase.External.Service.Sms;
using AhmadBase.External.Service.Storage;
using SmsContract = AhmadBase.External.Service.ISmsService;
using Autofac;
using BackOffice.Domain.Repositories;
using Ahmad.OnlineShop.Domain.Repositories;
using Ahmad.OnlineShop.Domain.Products;
using OldIUserRepository = Ahmad.OnlineShop.Domain.Users.IUserRepository;
using OldUserRepository  = Ahmad.OnlineShop.Persistence.EF.Repositories.UserRepository;
using Ahmad.OnlineShop.Persistence.EF.Repositories;
using Ahmad.OnlineShop.Persistence.EF;
using Ahmad.OnlineShop.Persistence.EF.Services;
using Microsoft.EntityFrameworkCore;
using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Application.Query.Contracts;
using BackOffice.Application.Query.Contracts;
using Ahmad.OnlineShop.Read.Dapper.Identity;
using Ahmad.OnlineShop.Read.Dapper.BackOffice;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ahmad.OnlineShop.Config;

public class AutofacModule : Autofac.Module
{
    private readonly IConfiguration _configuration;
    private readonly bool            _isDevelopment;

    public AutofacModule(IConfiguration configuration)
    {
        _configuration = configuration;
        _isDevelopment  = bool.TryParse(configuration["UseFakeServices"], out var b) && b;

        if (_isDevelopment)
            OtpCodeGenerator.ConfigureForDevelopment();
    }

    protected override void Load(ContainerBuilder builder)
    {
        // ── ۱. Framework Auto-Registration ────────────────────────────────────
        Container.Setup(builder, _configuration)
            .RegisterDatabaseModels()    // ApplicationDbContext + HiLo + DapperQuery
            .RegisterMainServices()      // IQueryService, Options, CacheService
            .RegisterHandlers()          // CommandBus, QueryBus, EventHandlers
            .RegisterAuxiliaryServices();

        // IdentityAppDbContext — context جداگانه برای Identity system جدید
        var identityConnStr = _configuration.GetConnectionString("CommandConnection") ?? string.Empty;
        builder.Register(c =>
        {
            var opts = new DbContextOptionsBuilder<IdentityAppDbContext>()
                .UseSqlServer(identityConnStr)
                .Options;
            return new IdentityAppDbContext(opts);
        }).AsSelf().InstancePerLifetimeScope();

        // ── ۲. Domain Repositories (EF Write Side) ────────────────────────────
        builder.RegisterType<CategoryRepository>()     .As<ICategoryRepository>()     .InstancePerLifetimeScope();
        builder.RegisterType<BnplContractRepository>() .As<IBnplContractRepository>() .InstancePerLifetimeScope();
        builder.RegisterType<CreditLimitRepository>()  .As<ICreditLimitRepository>()  .InstancePerLifetimeScope();
        builder.RegisterType<AdminUserRepository>()    .As<IAdminUserRepository>()    .InstancePerLifetimeScope();
        builder.RegisterType<ProductRepository>()      .As<IProductRepository>()      .InstancePerLifetimeScope();
        builder.RegisterType<OrderRepository>()        .As<IOrderRepository>()        .InstancePerLifetimeScope();
        builder.RegisterType<InventoryRepository>()    .As<IInventoryRepository>()    .InstancePerLifetimeScope();
        builder.RegisterType<ProductImageRepository>() .As<IProductImageRepository>() .InstancePerLifetimeScope();
        builder.RegisterType<OldUserRepository>()      .As<OldIUserRepository>()      .InstancePerLifetimeScope();

        // ── ۳. Identity Repositories ──────────────────────────────────────────
        builder.RegisterType<IdentityUserRepository>()        .As<IUserRepository>()         .InstancePerLifetimeScope();
        builder.RegisterType<IdentityOtpRepository>()         .As<IOtpRepository>()          .InstancePerLifetimeScope();
        builder.RegisterType<IdentityRefreshTokenRepository>().As<IRefreshTokenRepository>() .InstancePerLifetimeScope();
        builder.RegisterType<IdentityRoleRepository>()        .As<IRoleRepository>()         .InstancePerLifetimeScope();

        // ── ۴. IDbConnection (Dapper Read) ────────────────────────────────────
        var connStr = _configuration.GetConnectionString("QueryConnection") ?? string.Empty;
        builder.Register<IDbConnection>(_ => new SqlConnection(connStr))
               .As<IDbConnection>()
               .InstancePerLifetimeScope();

        // Dapper Read Repositories — framework auto-scan اینا رو پیدا نمی‌کنه چون IQueryService implement نمی‌کنن
        builder.RegisterType<UserReadRepository>()     .As<IUserReadRepository>()      .InstancePerLifetimeScope();
        builder.RegisterType<RoleReadRepository>()     .As<IRoleReadRepository>()      .InstancePerLifetimeScope();
        builder.RegisterType<AdminUserReadRepository>().As<IAdminUserReadRepository>() .InstancePerLifetimeScope();

        // ── ۵. SMS ────────────────────────────────────────────────────────────
        builder.Register(c =>
        {
            var opts = c.Resolve<IConfiguration>().GetSection(SmsOptions.Section).Get<SmsOptions>() ?? new SmsOptions();
            return Options.Create(opts);
        }).As<IOptions<SmsOptions>>().SingleInstance();

        if (_isDevelopment)
            builder.RegisterType<FakeSmsSender>().As<SmsContract>().InstancePerLifetimeScope();
        else
            builder.RegisterType<SmsSender>()    .As<SmsContract>().InstancePerLifetimeScope();

        // ── ۶. JWT ────────────────────────────────────────────────────────────
        builder.RegisterType<JwtService>().As<IJwtService>().InstancePerLifetimeScope();

        // JWTOptions قدیمی (سیستم Account) — کلاس مجزا از IOption، در assembly Option اسکن نمی‌شود
        builder.Register(c =>
        {
            var cfg = c.Resolve<IConfiguration>();
            return new Ahmad.OnlineShop.Application.Contract.Opetions.JWTOptions
            {
                Secret               = cfg["Jwt:Secret"] ?? string.Empty,
                ValidIssuer          = cfg["Jwt:ValidIssuer"] ?? string.Empty,
                ValidAudience        = cfg["Jwt:ValidAudience"] ?? string.Empty,
                TokenExpireMinutes   = cfg.GetValue<int>("Jwt:TokenExpireMinutes", 60),
                RefreshExpireMinutes = cfg.GetValue<int>("Jwt:RefreshExpireMinutes", 10080),
            };
        }).AsSelf().SingleInstance();

        // ── ۷. Payment (ZarinPal) ─────────────────────────────────────────────
        builder.Register(c =>
        {
            var opts = c.Resolve<IConfiguration>().GetSection(PaymentGatewayOptions.Section).Get<PaymentGatewayOptions>() ?? new PaymentGatewayOptions();
            return Options.Create(opts);
        }).As<IOptions<PaymentGatewayOptions>>().SingleInstance();

        builder.RegisterType<ZarinPalService>().As<IPaymentService>().InstancePerLifetimeScope();

        // ── ۸. File Storage (MinIO) ───────────────────────────────────────────
        builder.Register(c =>
        {
            var opts = c.Resolve<IConfiguration>().GetSection(FileStorageOptions.Section).Get<FileStorageOptions>() ?? new FileStorageOptions();
            return Options.Create(opts);
        }).As<IOptions<FileStorageOptions>>().SingleInstance();

        builder.RegisterType<MinioFileStorageService>().As<IFileStorageService>().InstancePerLifetimeScope();

        // IDateTimeProvider — در نسخه بعدی AhmadBase.Doamin اضافه می‌شود
    }
}
