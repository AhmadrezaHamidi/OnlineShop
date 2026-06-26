using AhmadBase.Application.Query;
using Ahmad.OnlineShop.Application.Dtos;
using Ahmad.OnlineShop.Application.Query.Queries;
using Ahmad.OnlineShop.Domain.Repositories;

namespace Ahmad.OnlineShop.Application.Query.Handlers;

public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<List<CategoryDto>> HandleAsync(GetCategoriesQuery q, CancellationToken token)
    {
        var categories = await _categoryRepository.GetAllAsync(token);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.ParentId))
            .ToList();
    }
}
