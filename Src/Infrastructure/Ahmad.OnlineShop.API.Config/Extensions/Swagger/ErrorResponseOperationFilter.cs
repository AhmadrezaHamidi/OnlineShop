using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ahmad.OnlineShop.Config.Extensions.Swagger;

/// <summary>
/// همه response های خطا (400/401/403/404/422/500) رو با ProblemDetails جایگزین می‌کنه
/// جلوگیری از نمایش مقدار success (مثل true یا long) برای کدهای خطا
/// </summary>
public class ErrorResponseOperationFilter : IOperationFilter
{
    private static readonly Dictionary<int, string> ErrorDescriptions = new()
    {
        { 400, "درخواست نامعتبر — validation یا business rule خطا" },
        { 401, "احراز هویت نشده — Bearer token ارسال نشده یا منقضی شده" },
        { 403, "دسترسی ممنوع — نقش کافی ندارید" },
        { 404, "یافت نشد" },
        { 422, "خطای منطق تجاری" },
        { 500, "خطای داخلی سرور" },
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var problemDetailsRef = context.SchemaGenerator.GenerateSchema(
            typeof(ProblemDetails), context.SchemaRepository);

        foreach (var (statusCode, description) in ErrorDescriptions)
        {
            var key = statusCode.ToString();

            if (!operation.Responses.TryGetValue(key, out var response))
            {
                response = new OpenApiResponse { Description = description };
                operation.Responses[key] = response;
            }
            else
            {
                response.Description = description;
            }

            response.Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = problemDetailsRef
                }
            };
        }
    }
}
