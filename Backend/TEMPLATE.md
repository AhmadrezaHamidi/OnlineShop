# Ahmad.OnlineShop — Project Template

این پروژه به عنوان **template** برای پروژه‌های بعدی آماده شده است.

## ساختار لایه‌ها

```
Src/
├── Domain/                        ← Entities, Value Objects, Domain Events
│   └── Ahmad.OnlineShop.Domain
│   └── Ahmad.OnlineShop.Event
├── Application/                   ← CQRS Commands/Queries, Handlers, Contracts
│   ├── Ahmad.OnlineShop.Application
│   ├── Ahmad.OnlineShop.Application.Contract
│   ├── Ahmad.OnlineShop.Application.Query
│   ├── Ahmad.OnlineShop.Application.Query.Contract
│   ├── Ahmad.OnlineShop.Application.Validators   ← FluentValidation
│   └── Ahmad.OnlineShop.Event.Handler
├── Infrastructure/                ← EF Core, Repos, External Services
├── Rest/                          ← Controllers, DTOs, Swagger config
└── Host/                          ← Program.cs, DI, Middleware
```

## تکنولوژی‌های اصلی

- **.NET 8** — ASP.NET Core Web API
- **EF Core 8** — SQL Server / LocalDB
- **MediatR** — CQRS pattern
- **FluentValidation** — command validation
- **JWT Bearer** — authentication
- **OTP (Bale SMS)** — بدون پسورد، با کد یکبار مصرف
- **IBaleService** — `SendMessageAsync(chatId, text)` → `https://tapi.bale.ai`

## برای پروژه‌ی بعدی

1. این repository را fork/clone کن
2. نام‌های `Ahmad.OnlineShop` را به نام پروژه‌ی جدید تغییر بده
3. Domain Entities را جایگزین/گسترش بده
4. ConnectionString را در `appsettings.json` تنظیم کن
5. Migration جدید بزن: `dotnet ef migrations add Init`

## نکات مهم

- `.env` و secrets هرگز commit نشوند
- همیشه روی branch کار کن، نه main
- برای SMS از **Bale** استفاده کن (نه Telegram)
