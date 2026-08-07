using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ahmad.OnlineShop.Domain.Products.Enums;
using Ahmad.OnlineShop.Rest.EndPoints.Product;
using AhmadBase.External.Service.Storage;
using AhmadBase.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Ahmad.OnlineShop.Rest.Endpoints;

public class ProductImageEndpoints : IEndpoint
{
    private static readonly HashSet<string> _allowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProductConstants.Routes.BaseRoute)
            .WithApiVersionSet()
            .WithTags("Product Images");

        group.MapGetEndpoint<List<GetProductImageResponse>>(
            ProductConstants.Routes.GetImages,
            GetImages,
            ProductConstants.Names.GetImages,
            ProductConstants.Docs.GetImages.Summary,
            ProductConstants.Docs.GetImages.Description);

        // آپلود فایل به MinIO
        group.MapPost("{id:long}/images/upload", UploadImage)
            .WithName("Product.UploadImage")
            .WithSummary("آپلود عکس محصول به MinIO — multipart/form-data")
            .DisableAntiforgery();

        group.MapPostEndpoint(
            ProductConstants.Routes.AddImage,
            AddImage,
            ProductConstants.Names.AddImage,
            ProductConstants.Docs.AddImage.Summary,
            ProductConstants.Docs.AddImage.Description);

        group.MapDeleteEndpoint(
            ProductConstants.Routes.RemoveImage,
            RemoveImage,
            ProductConstants.Names.RemoveImage,
            ProductConstants.Docs.RemoveImage.Summary,
            ProductConstants.Docs.RemoveImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.SetPrimaryImage,
            SetPrimaryImage,
            ProductConstants.Names.SetPrimaryImage,
            ProductConstants.Docs.SetPrimaryImage.Summary,
            ProductConstants.Docs.SetPrimaryImage.Description);

        group.MapPatchEndpoint(
            ProductConstants.Routes.ReorderImage,
            ReorderImage,
            ProductConstants.Names.ReorderImage,
            ProductConstants.Docs.ReorderImage.Summary,
            ProductConstants.Docs.ReorderImage.Description);
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private static async Task<List<GetProductImageResponse>> GetImages(
        long id,
        IQueryBus queryBus,
        CancellationToken ct)
        => await queryBus.DispatchAsync<List<GetProductImageResponse>>(
            new GetProductImagesQuery(id), ct);

    /// <summary>
    /// آپلود عکس مستقیماً به MinIO — فایل رو می‌گیره، کلید یکتا می‌سازه،
    /// آپلود می‌کنه و URL رو در DB ذخیره می‌کنه.
    /// </summary>
    private static async Task<IResult> UploadImage(
        long id,
        IFormFile file,
        ImageType type,
        ICommandBus bus,
        IFileStorageService storage,
        IOptions<FileStorageOptions> storageOpts,
        CancellationToken ct)
    {
        if (!_allowedTypes.Contains(file.ContentType))
            return Results.BadRequest(ApiResponse<string>.Fail(
                $"فرمت {file.ContentType} پشتیبانی نمی‌شود. فرمت‌های مجاز: JPEG, PNG, WebP, GIF"));

        if (file.Length > 5 * 1024 * 1024)
            return Results.BadRequest(ApiResponse<string>.Fail("حداکثر حجم مجاز ۵ مگابایت است"));

        var ext       = Path.GetExtension(file.FileName).TrimStart('.');
        var objectKey = $"products/{id}/{Guid.NewGuid():N}.{ext}";
        var bucket    = storageOpts.Value.DefaultBucket;

        await using var stream = file.OpenReadStream();
        var uploadResult = await storage.UploadAsync(
            new FileUploadRequest(bucket, objectKey, stream, file.ContentType, file.Length), ct);

        if (!uploadResult.Success)
            return Results.Problem($"آپلود به MinIO ناموفق بود: {uploadResult.Error}");

        var cmd = new AddProductImageCommand(id, uploadResult.PublicUrl!, objectKey, type);
        var productId = await bus.Dispatch<long>(cmd, ct);

        return Results.Ok(ApiResponse<UploadImageResponse>.Ok(
            new UploadImageResponse(productId, uploadResult.PublicUrl!, objectKey)));
    }

    private static async Task<long> AddImage(
        long id,
        [FromBody] AddProductImageCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<long>(command with { ProductId = id }, ct);

    private static async Task<Guid> RemoveImage(
        long id,
        Guid imageId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(new RemoveProductImageCommand(id, imageId), ct);

    private static async Task<Guid> SetPrimaryImage(
        long id,
        Guid imageId,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(new SetPrimaryImageCommand(id, imageId), ct);

    private static async Task<Guid> ReorderImage(
        long id,
        Guid imageId,
        [FromBody] ReorderProductImageCommand command,
        ICommandBus bus,
        CancellationToken ct)
        => await bus.Dispatch<Guid>(command with { ProductId = id, ImageId = imageId }, ct);

    public sealed record UploadImageResponse(long ProductId, string Url, string BucketKey);
}
