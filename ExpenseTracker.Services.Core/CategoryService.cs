namespace ExpenseTracker.Services.Core
{
    using static ExpenseTracker.Common.ApplicationConstants;
    using ExpenseTracker.Data;
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Category.Request;
    using ExpenseTracker.Services.DTOs.Category.Response;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using ExpenseTracker.DataModels;
    using ExpenseTracker.Common;
    using System.Data;
    using Microsoft.EntityFrameworkCore.Storage;

    public class CategoryService(ExpenseDbContext dbContext) : ICategoryService
    {
        public async Task InitializeAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            if (await dbContext.Categories.AnyAsync(category => category.UserId == userId)) 
            {
                return;
            }

            dbContext.Categories.AddRange(DefaultCategories.Select(item => new Category
            {
                Name = item.Key,
                Description = item.Value,
                UserId = userId,
                isDefault = item.Key == OtherCategoryName
            }));
            await dbContext.SaveChangesAsync();
        }

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

        public async Task<CategoryResponse?> GetAsync(Guid id, string userId)
        {
            Task<CategoryResponse?> cats = dbContext.Categories
                .AsNoTracking()
                .Where(category => category.Id == id && category.UserId == userId)
                .Select(category => new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    isDefault = category.isDefault,
                    ExpenseCount = category.Expenses.Count
                }).FirstOrDefaultAsync();

            return await cats;
        } 

        public async Task<Guid> CreateAsync(CategoryRequest request, string userId)
        {
            string name = request.Name?.Trim() ?? string.Empty;
            if (name.Length < EntityValidation.Category.CategoryNameMin ||
                name.Length > EntityValidation.Category.CategoryNameMax)
            {
                throw new ArgumentException($"Enter a category name between {EntityValidation.Category.CategoryNameMin} and {EntityValidation.Category.CategoryNameMax} characters.", nameof(request.Name));
            }

            var normalizedName = name.ToUpperInvariant();

            if (request.Description?.Length > EntityValidation.Category.CategoryDescriptionMax || request.Description?.Length < EntityValidation.Category.CategoryDescriptionMin)
            {
                throw new ArgumentException($"Description must be between {EntityValidation.Category.CategoryDescriptionMin} and {EntityValidation.Category.CategoryDescriptionMax} characters max.", nameof(request.Description));
            }

            if (normalizedName == ApplicationConstants.OtherCategoryName.ToUpperInvariant())
            {
                throw new ArgumentException("The category name Other is reserved for your default category.", nameof(request.Name));
            }

            if (await dbContext.Categories.AnyAsync(category => category.UserId == userId && category.Name.ToUpper() == normalizedName))
            {
                throw new ArgumentException("You already have a category with this name.", nameof(request.Name));
            }

            Category? category = new Category
            {
                Name = name,
                Description = request.Description?.Trim() ?? string.Empty,
                UserId = userId
            };

            dbContext.Categories.Add(category);

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ArgumentException("An error occured during category creation.", nameof(request.Name));
            }

            return category.Id;
        }

        public async Task<CategoryDeleteResponse> DeleteAsync(Guid id, string userId)
        {
            await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            Category? category = await dbContext.Categories
                .SingleOrDefaultAsync(item => item.Id == id && item.UserId == userId);

            if (category is null)
            {
                return new CategoryDeleteResponse()
                {
                    Success = false,
                    UpdatedExpensesCount = 0
                };
            };

            if (category.isDefault)
            {
                throw new ArgumentException("The Other category cannot be deleted.");
            }

            Guid otherId = await dbContext.Categories
                .Where(item => item.UserId == userId && item.isDefault)
                .Select(item => item.Id).SingleAsync();

            IEnumerable<Expense> expensesToUpdate = await dbContext.Expenses
                .Where(expense => expense.UserId == userId && expense.CategoryId == id)
                .ToListAsync();

            foreach (Expense expense in expensesToUpdate)
            {
                expense.CategoryId = otherId;
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            CategoryDeleteResponse deleteResponse = new CategoryDeleteResponse()
            { 
                Success = true,
                UpdatedExpensesCount = expensesToUpdate.Count() 
            };

            return deleteResponse;
        }

        public async Task<Guid> EditAsync(Guid catId, CategoryRequest request, string userId)
        {
            string name = request.Name?.Trim() ?? string.Empty;
            if (name.Length < EntityValidation.Category.CategoryNameMin ||
                name.Length > EntityValidation.Category.CategoryNameMax)
            {
                throw new ArgumentException($"Enter a category name between {EntityValidation.Category.CategoryNameMin} and {EntityValidation.Category.CategoryNameMax} characters.", nameof(request.Name));
            }

            var normalizedName = name.ToUpperInvariant();

            if (request.Description?.Length > EntityValidation.Category.CategoryDescriptionMax || request.Description?.Length < EntityValidation.Category.CategoryDescriptionMin)
            {
                throw new ArgumentException($"Description must be between {EntityValidation.Category.CategoryDescriptionMin} and {EntityValidation.Category.CategoryDescriptionMax} characters max.", nameof(request.Description));
            }

            if (normalizedName == ApplicationConstants.OtherCategoryName.ToUpperInvariant())
            {
                throw new ArgumentException("The category name Other is reserved for your default category.", nameof(request.Name));
            }

            Category? category = await dbContext.Categories
                .SingleOrDefaultAsync(item => item.Id == catId && item.UserId == userId);

            if (category is null)
            {
                throw new ArgumentException("Category not found.", nameof(request.Name));
            }

            if (await dbContext.Categories.AnyAsync(item => item.UserId == userId && item.Id != catId && item.Name.ToUpper() == normalizedName))
            {
                throw new ArgumentException("You already have a category with this name.", nameof(request.Name));
            }

            category.Name = name;
            category.Description = request.Description?.Trim() ?? string.Empty;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ArgumentException("An error occured during category update.", nameof(request.Name));
            }
            return category.Id;
        }
    }
}
