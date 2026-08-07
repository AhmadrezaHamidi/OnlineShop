# Ahmad OnlineShop

فروشگاه آنلاین طراحی‌شده با معماری **DDD + Clean Architecture + CQRS** روی پلتفرم **.NET 8**.

---

## فهرست مطالب

- [معماری پروژه](#معماری-پروژه)
- [Design Patterns پیاده‌سازی‌شده](#design-patterns)
- [ماژول‌های اصلی](#ماژول‌های-اصلی)
- [API Endpoints](#api-endpoints)
- [راه‌اندازی با Docker](#راه‌اندازی-با-docker)
- [متغیرهای محیطی](#متغیرهای-محیطی)

---

## معماری پروژه

```
Src/
├── Domain/
│   └── Ahmad.OnlineShop.Domain        ← Aggregates، Entities، Domain Events، Repository Interfaces
├── Application/
│   ├── Ahmad.OnlineShop.Application   ← Command/Query Handlers
│   ├── ...Contract                    ← Commands، Queries (DTOs)
│   ├── ...Application.Query           ← Query Handlers (Read Side)
│   └── ...Event.Handler               ← Domain Event Handlers
├── Infrastructure/
│   ├── Ahmad.OnlineShop.Persistence.EF  ← EF Core، Repositories، Migrations
│   ├── Ahmad.OnlineShop.Read.Dapper     ← Dapper Read Side
│   └── Ahmad.OnlineShop.API.Config      ← Autofac Module، Middleware
├── Rest/
│   └── Ahmad.OnlineShop.API.Rest        ← Minimal API Endpoints (IEndpoint)
└── Host/
    └── Ahmad.OnlineShop.ServiceHost     ← Entry Point
```

### لایه‌بندی

| لایه | مسئولیت |
|------|---------|
| **Domain** | منطق تجاری، Aggregate Rootها، Domain Events |
| **Application** | Orchestration، Handlers، Validation |
| **Infrastructure** | EF Core، Dapper، MinIO، ZarinPal، SMS |
| **REST** | Minimal API، Swagger، Versioning |

---

## Design Patterns

### ۱. Strategy Pattern — محاسبه تخفیف

هر نوع تخفیف یک strategy جداگانه دارد. `Discount.Apply()` بدون if/else کار می‌کند.

```
IDiscountCalculationStrategy
├── PercentageDiscountStrategy   ← تخفیف درصدی
└── FixedAmountDiscountStrategy  ← تخفیف مبلغ ثابت

DiscountStrategyFactory.Create(DiscountType) → استراتژی مناسب را می‌سازد
```

**مسیر:** `Src/Domain/Ahmad.OnlineShop.Domain/Discount/Strategies/`

### ۲. Specification Pattern — اعتبارسنجی تخفیف

قوانین اعتبارسنجی به صورت ترکیب‌پذیر و جداگانه تعریف شده‌اند:

```
IDiscountSpecification
├── IsActiveSpecification         ← کد فعال باشد
├── NotExpiredSpecification       ← منقضی نشده باشد
├── UsageLimitSpecification       ← سقف استفاده پر نشده باشد
└── MinOrderAmountSpecification   ← حداقل مبلغ سفارش رعایت شده باشد

DiscountSpecificationBuilder.BuildFullValidation()
    → همه قوانین را با AND ترکیب می‌کند
```

**مسیر:** `Src/Domain/Ahmad.OnlineShop.Domain/Discount/Specifications/`

### ۳. Observer Pattern — صدور خودکار فاکتور

وقتی سفارش تأیید می‌شود (`OrderConfirmedEvent`)، event handler به صورت خودکار فاکتور صادر می‌کند:

```
OrderConfirmed → OrderConfirmedEventHandler
                    ├── ConfirmStock (کسر قطعی موجودی)
                    └── CreateInvoiceCommand → InvoiceHandlers → Invoice.Create()
```

**مسیر:** `Src/Application/Ahmad.OnlineShop.Event.Handler/Order/OrderConfirmedEventHandler.cs`

### ۴. Factory Pattern — ساخت استراتژی تخفیف

```csharp
DiscountStrategyFactory.Create(DiscountType.Percentage)
    → new PercentageDiscountStrategy()
```

### ۵. Circuit Breaker Pattern — پرداخت آنلاین با Fallback

درگاه ZarinPal با **Polly v8 Circuit Breaker** محافظت می‌شود. در صورت خرابی درگاه، سفارش به **Cash On Delivery** Fallback می‌شود:

```
POST /api/v1/Order/{id}/online-payment
    → اگر ZarinPal در دسترس: لینک پرداخت بازگشت داده می‌شود
    → اگر Circuit باز باشد (BrokenCircuitException): ثبت به عنوان COD
```

**مسیر:** `Src/Application/Ahmad.OnlineShop.Application/Order/OrderHandlers.cs`

### ۶. Repository Pattern + Unit of Work

هر Aggregate Root یک Repository interface در Domain دارد و یک EF پیاده‌سازی در Infrastructure. `ApplicationDbContext` نقش Unit of Work را بازی می‌کند و Domain Events را پس از SaveChanges dispatch می‌کند.

---

## ماژول‌های اصلی

### محصولات و دسته‌بندی
- مدیریت محصول با موجودی جداگانه (Inventory Aggregate)
- آپلود تصویر روی **MinIO** (S3-compatible)
- دسته‌بندی سلسله‌مراتبی

### سفارش‌ها

```
Draft → Placed → Confirmed → Shipped → Delivered
                           ↘ Cancelled
```

- اضافه/حذف آیتم در حالت Draft
- اعمال کد تخفیف یا پکیج روی سفارش
- ثبت چند پرداخت برای یک سفارش

### تخفیف‌ها و پکیج‌ها

| نوع | توضیح |
|-----|-------|
| **Discount Code** | کد تخفیف درصدی یا مبلغ ثابت با محدودیت استفاده و تاریخ انقضا |
| **Product Package** | بسته‌بندی چند محصول با تخفیف گروهی |

### فاکتور (رسید)

فاکتور به صورت **خودکار** پس از تأیید پرداخت صادر می‌شود:

```
شماره فاکتور: INV-{سال ماه}-{id:D6}   مثال: INV-202607-000001

فیلدها: جمع کل، مبلغ تخفیف کد، تخفیف پکیج، مبلغ نهایی، روش پرداخت، کد تراکنش
```

### پرداخت — ZarinPal با Fallback

```
POST /api/v1/Order/{id}/online-payment
    → اگر ZarinPal در دسترس: بازگشت URL پرداخت
    → اگر Circuit باز باشد: سفارش COD ثبت می‌شود
```

### احراز هویت
- **OTP Login**: ورود با کد یک‌بارمصرف SMS
- **JWT + Refresh Token**
- نقش‌ها: Customer / Seller / Admin

### BNPL — خرید اقساطی
- ثبت قرارداد، اقساط، و محاسبه بدهی
- سقف اعتبار (CreditLimit) برای هر کاربر

---

## API Endpoints

### احراز هویت

| Method | Route | توضیح |
|--------|-------|-------|
| POST | `/api/v1/auth/request-otp` | ارسال OTP |
| POST | `/api/v1/auth/verify-otp` | تأیید OTP و دریافت Token |
| POST | `/api/v1/auth/refresh` | تجدید Token |
| POST | `/api/v1/auth/logout` | خروج |

### محصولات

| Method | Route | توضیح |
|--------|-------|-------|
| GET | `/api/v1/product` | لیست محصولات |
| GET | `/api/v1/product/{id}` | جزئیات محصول |
| POST | `/api/v1/product` | ایجاد محصول |
| PUT | `/api/v1/product/{id}` | ویرایش محصول |
| POST | `/api/v1/product/{id}/image` | آپلود تصویر |
| GET | `/api/v1/inventory/{productId}` | موجودی محصول |

### سفارش‌ها

| Method | Route | توضیح |
|--------|-------|-------|
| GET | `/api/v1/Order` | لیست سفارش‌ها |
| GET | `/api/v1/Order/{id}` | جزئیات سفارش |
| POST | `/api/v1/Order` | ایجاد سفارش |
| POST | `/api/v1/Order/{id}/Items` | افزودن آیتم |
| DELETE | `/api/v1/Order/{id}/Items/{itemId}` | حذف آیتم |
| POST | `/api/v1/Order/{id}/Place` | نهایی‌سازی |
| POST | `/api/v1/Order/{id}/Confirm` | تأیید |
| POST | `/api/v1/Order/{id}/Ship` | ارسال |
| POST | `/api/v1/Order/{id}/Deliver` | تحویل |
| POST | `/api/v1/Order/{id}/Cancel` | لغو |
| POST | `/api/v1/Order/{id}/discount` | **اعمال کد تخفیف** |
| POST | `/api/v1/Order/{id}/package` | **اعمال پکیج** |
| POST | `/api/v1/Order/{id}/online-payment` | پرداخت آنلاین (ZarinPal) |
| POST | `/api/v1/Order/{id}/online-payment/verify` | تأیید پرداخت |

### تخفیف‌ها و پکیج‌ها

| Method | Route | توضیح |
|--------|-------|-------|
| GET | `/api/v1/discount` | لیست کدهای تخفیف |
| GET | `/api/v1/discount/{id}` | جزئیات کد تخفیف |
| POST | `/api/v1/discount` | ایجاد کد تخفیف |
| PUT | `/api/v1/discount/{id}` | ویرایش کد تخفیف |
| GET | `/api/v1/package` | لیست پکیج‌ها |
| GET | `/api/v1/package/{id}` | جزئیات پکیج |
| POST | `/api/v1/package` | ایجاد پکیج |

### فاکتورها

| Method | Route | توضیح |
|--------|-------|-------|
| GET | `/api/v1/invoices/{id}` | دریافت فاکتور با ID |
| GET | `/api/v1/invoices/order/{orderId}` | فاکتور سفارش |
| GET | `/api/v1/invoices/my?userId={id}` | فاکتورهای من |

---

## راه‌اندازی با Docker

```bash
# همه سرویس‌ها را بالا بیاور
docker compose up -d

# بازسازی سریع API (بدون نیاز به pull از MCR)
dotnet publish Src/Host/Ahmad.OnlineShop.ServiceHost -c Release -o publish_temp
docker build -f Dockerfile.local -t onlineshop-api:latest .
docker stop onlineshop-api && docker rm onlineshop-api
docker run -d --name onlineshop-api --network onlineshop-network -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=onlineshop-db;..." \
  -e Jwt__Secret="your-secret" \
  -e JWTOptions__Secret="your-secret" \
  onlineshop-api:latest
```

### سرویس‌های Docker Compose

| سرویس | پورت | توضیح |
|--------|------|-------|
| `onlineshop-api` | 8080 | بک‌اند .NET 8 |
| `onlineshop-db` | 1433 | SQL Server |
| `onlineshop-minio` | 9000 / 9001 | ذخیره تصاویر (Console) |
| `onlineshop-seq` | 5341 / 8081 | لاگ‌های Structured |
| `onlineshop-frontend` | 3000 | React Frontend |

---

## متغیرهای محیطی

| کلید | توضیح |
|------|-------|
| `ConnectionStrings__DefaultConnection` | رشته اتصال SQL Server (Write) |
| `ConnectionStrings__CommandConnection` | رشته اتصال Identity DB |
| `ConnectionStrings__QueryConnection` | رشته اتصال Dapper Read |
| `Jwt__Secret` | کلید JWT (سیستم قدیمی) |
| `JWTOptions__Secret` | کلید JWT (AhmadBase framework) |
| `JWTOptions__ValidIssuer` | Issuer برای JWT |
| `JWTOptions__ValidAudience` | Audience برای JWT |
| `ZarinPal__MerchantId` | شناسه مرچنت زرین‌پال |
| `Minio__Endpoint` | آدرس MinIO (مثلاً `minio:9000`) |
| `Minio__AccessKey` | کلید دسترسی MinIO |
| `Minio__SecretKey` | رمز MinIO |
| `Minio__BucketName` | نام Bucket تصاویر |
| `UseFakeServices` | `true` = SMS فیک، OTP در Log |

---

## تکنولوژی‌ها

| بخش | تکنولوژی |
|-----|----------|
| Framework | .NET 8، ASP.NET Core Minimal API |
| ORM | EF Core 8 (Write Side)، Dapper (Read Side) |
| DI Container | Autofac |
| Validation | FluentValidation |
| Resilience | Polly v8 (Circuit Breaker) |
| Logging | Serilog → Console + File + Seq |
| File Storage | MinIO (S3-compatible) |
| Payment Gateway | ZarinPal |
| Auth | JWT + OTP SMS |
| ID Generation | HiLo (AhmadBase) |
| Containerization | Docker + Docker Compose |
| Frontend | React (پوشه `-FRONT`) |
