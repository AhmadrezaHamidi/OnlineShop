using AhmadBase.Application;
using Ahmad.OnlineShop.Application.Commands;
using Ahmad.OnlineShop.Domain.Entities;
using Ahmad.OnlineShop.Domain.Exceptions;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Handlers;

public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, long>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<long> Handle(CreateCategoryCommand cmd, CancellationToken token)
    {
        var exists = await _categoryRepository.ExistsByNameAsync(cmd.Name, token);
        if (exists)
            throw new ProductDomainException("CATEGORY_DUPLICATE_NAME", $"A category with name '{cmd.Name}' already exists.");

        if (cmd.ParentId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(cmd.ParentId.Value, token);
            if (parent is null)
            {
                var (code, msg) = ProductErrors.Get(ProductErrors.CategoryNotFound);
                throw new ProductDomainException(code, msg);
            }
        }

        var id       = await _categoryRepository.GetNextIdAsync();
        var category = new Category(id, cmd.Name, cmd.ParentId);

        await _categoryRepository.AddAsync(category, token);
        return category.Id;
    }
}

public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, long>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<long> Handle(UpdateCategoryCommand cmd, CancellationToken token)
    {
        var category = await _categoryRepository.GetByIdAsync(cmd.Id, token);
        if (category is null)
        {
            var (code, msg) = ProductErrors.Get(ProductErrors.CategoryNotFound);
            throw new ProductDomainException(code, msg);
        }

        if (cmd.ParentId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(cmd.ParentId.Value, token);
            if (parent is null)
            {
                var (code, msg) = ProductErrors.Get(ProductErrors.CategoryNotFound);
                throw new ProductDomainException(code, msg);
            }
        }

        category.Rename(cmd.Name);
        category.ChangeParent(cmd.ParentId);

        await _categoryRepository.UpdateAsync(category, token);
        return category.Id;
    }
}
