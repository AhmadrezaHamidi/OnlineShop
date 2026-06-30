using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ahmad.OnlineShop.Config.Extensions.Swagger;

/// <summary>
/// توضیحات Flow های کامل API برای Swagger UI
/// این extension توضیحات جامع در بالای Swagger اضافه می‌کند
/// </summary>
public static class SwaggerFlowExtensions
{
    private const string ApiDescription = """
        # 🛒 Ahmad OnlineShop API

        ## 🔑 احراز هویت (OTP)
        تمام endpoint ها (به جز Auth) نیاز به Bearer JWT دارند.

        **Flow ورود:**
        1. `POST /api/v1/Identity/Auth/Login` — شماره موبایل را ارسال کنید
        2. کد OTP به موبایل ارسال می‌شود
        3. `POST /api/v1/Identity/Auth/Login/verify` — کد را تأیید کنید → **JWT دریافت می‌کنید**
        4. در header: `Authorization: Bearer {token}`

        ---

        ## 👤 نقش‌ها
        | نقش | دسترسی |
        |-----|--------|
        | **Customer** | سفارش‌گذاری، پرداخت، BNPL |
        | **Seller** | مدیریت محصولات خودش، موجودی، تصاویر |
        | **Admin** | همه سفارشات، گزارشات، مدیریت کاربران |

        ---

        ## 🛒 Flow سفارش
        ```
        POST /orders              ← ایجاد سفارش
        POST /orders/{id}/items   ← افزودن آیتم (موجودی Reserve می‌شود)
        POST /orders/{id}/place   ← ثبت نهایی
        POST /orders/{id}/payments ← ثبت پرداخت
        PATCH /payments/{pid}/complete ← تأیید پرداخت → Confirmed
        PATCH /orders/{id}/ship   ← ارسال
        PATCH /orders/{id}/deliver ← تحویل
        ```

        ## 💳 روش‌های پرداخت
        - **ZarinPal**: Redirect به درگاه → Callback → Verify
        - **CashOnDelivery**: ارسال → تحویل → تأیید دستی توسط ادمین

        ## 💰 BNPL (اقساطی)
        ```
        GET  /bnpl/users/{id}/credit     ← بررسی سقف اعتبار
        POST /bnpl/contracts             ← ایجاد قرارداد اقساطی
        POST /bnpl/contracts/{id}/installments/{iid}/pay ← پرداخت قسط
        ```

        ---
        📄 [مستندات کامل Flow ها](../Docs/Flows/access-and-flows.html)
        """;

    public static SwaggerGenOptions AddFlowDescriptions(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "Ahmad OnlineShop API",
            Version     = "v1",
            Description = ApiDescription,
            Contact     = new OpenApiContact
            {
                Name  = "Ahmad OnlineShop Team",
                Email = "vestaabnergame@gmail.com"
            }
        });

        // JWT Bearer Auth برای همه endpoint ها
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name         = "Authorization",
            Type         = SecuritySchemeType.Http,
            Scheme       = "Bearer",
            BearerFormat = "JWT",
            In           = ParameterLocation.Header,
            Description  = "JWT Bearer token — مثال: `Bearer eyJhbGci...`"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return options;
    }
}
