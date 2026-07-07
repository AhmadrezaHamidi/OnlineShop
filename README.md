# 🛍️ Ahmad OnlineShop

پلتفرم فروشگاه آنلاین با معماری DDD + CQRS — شامل Backend (.NET 8)، پنل مدیریت React، و زیرساخت کامل Docker.

---

## 📸 تصاویر پنل مدیریت

### صفحه ورود
![Login Page](docs/screenshots/login.png)

### داشبورد اصلی
![Dashboard](docs/screenshots/dashboard.png)

### مدیریت محصولات
![Products](docs/screenshots/products.png)

### مدیریت سفارش‌ها
![Orders](docs/screenshots/orders.png)

---

## 🚀 راه‌اندازی سریع

```bash
cp .env.example .env
docker compose up -d
```

| سرویس | آدرس |
|-------|------|
| 🖥️ پنل مدیریت | http://localhost:5510 |
| ⚙️ API | http://localhost:8080/swagger |
| 🗄️ MinIO Console | http://localhost:9001 |
| 📋 Seq (لاگ‌ها) | http://localhost:5341 |

---

## 🏗️ ساختار پروژه

```
OnlineShop/
├── Backend/                     # .NET 8 Web API
│   ├── Src/
│   │   ├── Domain/              # DDD Entities, Value Objects
│   │   ├── Application/         # CQRS Commands & Queries
│   │   ├── Infrastructure/      # EF Core, Dapper, Redis
│   │   └── Host/                # ASP.NET Core entry point
│   └── Dockerfile
├── Frontend/                    # React 18 + TypeScript + Vite
│   └── src/
│       ├── pages/admin/         # داشبورد، سفارش‌ها، محصولات، ...
│       ├── pages/auth/          # ورود OTP
│       ├── api/                 # Axios client + endpoints
│       └── components/
└── docker-compose.yml
```

---

## 🛠️ تکنولوژی‌ها

### Backend
- **ASP.NET Core 8** — REST API
- **CQRS** با MediatR
- **DDD** — Domain-Driven Design
- **Entity Framework Core** + **Dapper**
- **SQL Server 2022**
- **Redis** — کش
- **MinIO** — ذخیره‌سازی فایل
- **Quartz.NET** — Job scheduler
- **Serilog** → Seq

### Frontend
- **React 18** + **TypeScript**
- **Vite** — Build tool
- **React Router v6**
- **TanStack Query**
- **Tabler Icons**

---

## 🔑 احراز هویت

ورود با OTP پیامکی:
1. شماره موبایل → دریافت کد ۶ رقمی
2. تأیید کد → JWT token
3. Auto-refresh token

---

## 📦 سرویس‌های Docker

| Container | Image | Port |
|-----------|-------|------|
| `onlineshop-api` | custom (.NET 8) | 8080 |
| `onlineshop-frontend` | nginx:alpine | 5510 |
| `onlineshop-sql` | mssql/server:2022 | 1433 |
| `onlineshop-redis` | redis:7-alpine | 6379 |
| `onlineshop-minio` | minio/minio | 9000/9001 |

> **لاگ‌ها و مانیتورینگ**: از Seq و Grafana اشتراکی استفاده می‌شود.
