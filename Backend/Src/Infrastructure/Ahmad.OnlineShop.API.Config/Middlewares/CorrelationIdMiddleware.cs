using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Ahmad.OnlineShop.Config.Middlewares;

/// <summary>
/// به هر Request یک CorrelationId یکتا اختصاص می‌دهد.
/// اگر کلاینت X-Correlation-Id بفرستد، همان استفاده می‌شود؛ وگرنه GUID جدید ساخته می‌شود.
/// CorrelationId در response header و در همه لاگ‌های همان request ظاهر می‌شود.
/// </summary>
public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var incoming) && !string.IsNullOrWhiteSpace(incoming)
            ? incoming.ToString()
            : Guid.NewGuid().ToString("N");

        context.Items[HeaderName]   = correlationId;
        context.Response.Headers.TryAdd(HeaderName, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

/// <summary>
/// CorrelationId middleware رو اول از همه در pipeline قرار می‌دهد
/// تا همه لاگ‌های هر Request — از جمله لاگ‌های AhmadBase — CorrelationId داشته باشند.
/// </summary>
public class CorrelationIdStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        => app =>
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            next(app);
        };
}
