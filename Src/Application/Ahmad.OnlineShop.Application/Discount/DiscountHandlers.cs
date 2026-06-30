using Ahmad.OnlineShop.Application.Commands.Discount;
using Ahmad.OnlineShop.Domain.Discount.Args;
using Ahmad.OnlineShop.Domain.Discount.Exceptions;
using Ahmad.OnlineShop.Domain.Discount.Repositories;
using Ahmad.OnlineShop.Persistence.EF;
using DiscountAggregate = Ahmad.OnlineShop.Domain.Discount.Aggregates.Discount;
using ProductPackage    = Ahmad.OnlineShop.Domain.Discount.Aggregates.ProductPackage;

namespace Ahmad.OnlineShop.Application.Handlers.Discount;

public sealed class DiscountHandlers(
    IDiscountRepository      discountRepo,
    IProductPackageRepository packageRepo,
    ApplicationDbContext      context) :
    ICommandHandler<CreateDiscountCommand, long>,
    ICommandHandler<ActivateDiscountCommand, bool>,
    ICommandHandler<DeactivateDiscountCommand, bool>,
    ICommandHandler<ApplyDiscountCommand, decimal>,
    ICommandHandler<CreatePackageCommand, long>,
    ICommandHandler<AddPackageItemCommand, bool>,
    ICommandHandler<RemovePackageItemCommand, bool>,
    ICommandHandler<ActivatePackageCommand, bool>,
    ICommandHandler<DeactivatePackageCommand, bool>
{
    #region Discount Commands

    public async Task<long> Handle(CreateDiscountCommand cmd, CancellationToken token)
    {
        if (await discountRepo.CodeExistsAsync(cmd.Code, token))
            throw new DiscountCodeAlreadyExistsException();

        var id       = discountRepo.GetNextId();
        var discount = DiscountAggregate.Create(new CreateDiscountArg(
            id, cmd.Code, cmd.Title, cmd.Type,
            cmd.Value, cmd.MinOrderAmount, cmd.MaxUsage, cmd.ExpiresAt));

        await discountRepo.AddAsync(discount, token);
        await context.SaveChangesAsync(token);
        return discount.Id;
    }

    public async Task<bool> Handle(ActivateDiscountCommand cmd, CancellationToken token)
    {
        var discount = await discountRepo.GetByIdAsync(cmd.DiscountId, token)
            ?? throw new DiscountNotFoundException();

        discount.Activate();
        await discountRepo.UpdateAsync(discount, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(DeactivateDiscountCommand cmd, CancellationToken token)
    {
        var discount = await discountRepo.GetByIdAsync(cmd.DiscountId, token)
            ?? throw new DiscountNotFoundException();

        discount.Deactivate();
        await discountRepo.UpdateAsync(discount, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    public async Task<decimal> Handle(ApplyDiscountCommand cmd, CancellationToken token)
    {
        var discount = await discountRepo.GetByCodeAsync(cmd.Code, token)
            ?? throw new DiscountNotFoundException();

        var discountAmount = discount.Apply(cmd.OrderAmount);
        await discountRepo.UpdateAsync(discount, token);
        await context.SaveChangesAsync(token);
        return discountAmount;
    }

    #endregion

    #region Package Commands

    public async Task<long> Handle(CreatePackageCommand cmd, CancellationToken token)
    {
        var id  = packageRepo.GetNextId();
        var pkg = ProductPackage.Create(new CreatePackageArg(
            id, cmd.Title, cmd.Description,
            cmd.DiscountPercent, cmd.ValidFrom, cmd.ValidTo));

        await packageRepo.AddAsync(pkg, token);
        await context.SaveChangesAsync(token);
        return pkg.Id;
    }

    public async Task<bool> Handle(AddPackageItemCommand cmd, CancellationToken token)
    {
        var pkg = await packageRepo.GetByIdAsync(cmd.PackageId, token)
            ?? throw new ProductPackageNotFoundException();

        var itemId = packageRepo.GetNextItemId();
        pkg.AddItem(itemId, cmd.ProductId, cmd.Quantity);

        await packageRepo.UpdateAsync(pkg, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(RemovePackageItemCommand cmd, CancellationToken token)
    {
        var pkg = await packageRepo.GetByIdAsync(cmd.PackageId, token)
            ?? throw new ProductPackageNotFoundException();

        pkg.RemoveItem(cmd.ProductId);
        await packageRepo.UpdateAsync(pkg, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(ActivatePackageCommand cmd, CancellationToken token)
    {
        var pkg = await packageRepo.GetByIdAsync(cmd.PackageId, token)
            ?? throw new ProductPackageNotFoundException();

        pkg.Activate();
        await packageRepo.UpdateAsync(pkg, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> Handle(DeactivatePackageCommand cmd, CancellationToken token)
    {
        var pkg = await packageRepo.GetByIdAsync(cmd.PackageId, token)
            ?? throw new ProductPackageNotFoundException();

        pkg.Deactivate();
        await packageRepo.UpdateAsync(pkg, token);
        await context.SaveChangesAsync(token);
        return true;
    }

    #endregion
}
