namespace ExpenseTracker.Services.Core
{
    using ExpenseTracker.Data;
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Category.Response;
    using Microsoft.EntityFrameworkCore;

    public class CategoryService(ExpenseDbContext dbContext) : ICategoryService
    {
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync(string userId)
        {
            return await dbContext.Categories.AsNoTracking()
                .Where(category => category.UserId == userId)
                .OrderBy(category => category.Name)
                .Select(category => new CategoryResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    isDefault = category.isDefault,
                    ExpenseCount = category.Expenses.Count
                }).ToListAsync();
        }
    }
}
