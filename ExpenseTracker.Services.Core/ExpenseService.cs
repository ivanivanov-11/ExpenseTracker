namespace ExpenseTracker.Services.Core
{
    using ExpenseTracker.Data;
    using ExpenseTracker.DataModels;
    using ExpenseTracker.Services.DTOs.Expense.Request;
    using ExpenseTracker.Services.DTOs.Expense.Response;
    using Microsoft.EntityFrameworkCore;
    using ExpenseTracker.Common;
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Category.Response;

    public class ExpenseService : IExpenseService
    {
        private readonly ExpenseDbContext dbContext;

        public ExpenseService(ExpenseDbContext dbContext) => this.dbContext = dbContext;

        public async Task<DashboardResponse> GetDashboardAsync(string userId, DateTime now)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Select(u => new { u.Id, u.UserName })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) throw new UnauthorizedAccessException("User no longer exists.");

            var monthExpenses = dbContext.Expenses
                .AsNoTracking()
                .Include(expense => expense.Category)
                .Where(expense => expense.UserId == userId && 
                expense.PaidAt.Year == now.Year && 
                expense.PaidAt.Month == now.Month);

            var expenses = await monthExpenses
                .OrderByDescending(expense => expense.PaidAt)
                .ToListAsync();
            
            var budget = await dbContext.Budgets
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.UserId == userId && item.Year == now.Year && item.Month == now.Month);
            
            var total = expenses.Sum(expense => expense.Amount);

            return new DashboardResponse
            {
                UserName = user.UserName ?? string.Empty,
                MonthName = now.ToString("MMMM yyyy"),
                TotalSpent = total,
                TransactionCount = expenses.Count,
                AverageTransaction = expenses.Count == 0 ? 0 : total / expenses.Count,
                BudgetLimit = budget?.MonthlyLimit,
                RecentExpenses = expenses
                    .Take(ApplicationConstants.DefaultPageSize)
                    .Select(SummariseResponse)
                    .ToList(),

                CategoryBreakdown = expenses
                    .GroupBy(expense => new { expense.CategoryId, expense.Category.Name })
                    .Select(group => new CategorySpendResponse
                    {
                        CategoryName = group.Key.Name,
                        Amount = group.Sum(expense => expense.Amount),
                        Percentage = total == 0 ? 0 : group.Sum(expense => expense.Amount) / total * 100
                    })
                    .OrderByDescending(item => item.Amount)
                    .ToList()
            };
        }

        public async Task<ExpensePageResponse> GetIndexAsync(string userId, ExpenseFilterRequest filter)
        {
            var query = dbContext.Expenses
                .AsNoTracking()
                .Include(expense => expense.Category)
                .Where(expense => expense.UserId == userId);

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(expense => expense.CategoryId == filter.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(expense => expense.Title.Contains(filter.Search) || (expense.Notes != null && expense.Notes.Contains(filter.Search)));
            }

            if (filter.FromDateTime.HasValue)
            {
                query = query.Where(expense => expense.PaidAt >= filter.FromDateTime.Value.Date);
            }

            if (filter.ToDateTime.HasValue && filter.ToDateTime.Value.Date < DateTime.MaxValue.Date)
            {
                query = query.Where(expense => expense.PaidAt < filter.ToDateTime.Value.Date.AddDays(1));
            }

            var totalCount = await query.CountAsync();
            var total = await query.SumAsync(expense => expense.Amount);

            var pageSize = Math.Clamp(filter.PageSize, 1, 100);
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            var page = Math.Clamp(filter.Page, 1, totalPages);

            var expenses = await query
                .OrderByDescending(expense => expense.PaidAt)
                .ThenByDescending(expense => expense.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(expense => new ExpenseResponse
                {
                    Id = expense.Id,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    PaidAt = expense.PaidAt,
                    CategoryId = expense.CategoryId,
                    CategoryName = expense.Category.Name,
                    PaymentMethod = expense.PaymentMethod
                })
                .ToListAsync();

            return new ExpensePageResponse
            {
                Expenses = expenses,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<ExpenseResponse?> GetDetailsAsync(Guid id, string userId)
        {
            return await dbContext.Expenses
                .AsNoTracking()
                .Include(expense => expense.Category)
                .Where(expense => expense.Id == id && expense.UserId == userId)
                .Select(expense => new ExpenseResponse
                {
                    Id = expense.Id,
                    Title = expense.Title,
                    Notes = expense.Notes,
                    Amount = expense.Amount,
                    PaidAt = expense.PaidAt,
                    CategoryId = expense.CategoryId,
                    CategoryName = expense.Category.Name,
                    PaymentMethod = expense.PaymentMethod
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateAsync(ExpenseRequest model, string userId)
        {
            await ValidateCategoryAsync(model.CategoryId, userId);

            var expense = new DataModels.Expense();

            Apply(model, expense, userId);

            dbContext.Expenses.Add(expense);
            await SaveChangesAsync();

            return expense.Id;
        }

        public async Task<bool> UpdateAsync(Guid id, ExpenseRequest model, string userId)
        {
            var expense = await dbContext.Expenses
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

            if (expense is null) return false;

            await ValidateCategoryAsync(model.CategoryId, userId);

            Apply(model, expense, userId);
            await SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            var expense = await dbContext.Expenses
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == userId);

            if (expense is null) return false;

            dbContext.Expenses.Remove(expense);
            await SaveChangesAsync();

            return true;
        }

        private async Task SaveChangesAsync()
        {
            try 
            { 
                await dbContext.SaveChangesAsync(); 
            }
            catch (DbUpdateException)
            {
                throw new ArgumentException("This category is not available. Choose another category.", "CategoryId");
            }
        }

        private async Task ValidateCategoryAsync(Guid categoryId, string userId)
        {
            if (!await dbContext.Categories
                .AnyAsync(category => category.Id == categoryId && category.UserId == userId))
            {
                throw new ArgumentException("Choose an existing category.", "CategoryId");
            }
        }

        private static void Apply(ExpenseRequest model, DataModels.Expense expense, string userId)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || 
                model.Title.Trim().Length < EntityValidation.Expense.ExpenseTitleMin ||
                model.Title.Length > EntityValidation.Expense.ExpenseTitleMax)
            {
                throw new ArgumentException($"Enter a title between {EntityValidation.Expense.ExpenseTitleMin} and {EntityValidation.Expense.ExpenseTitleMax} characters.", "Title");
            }

            if (model.Amount <= 0 || model.Amount > decimal.Parse(EntityValidation.Expense.ExpenseAmountMax) || decimal.Round(model.Amount, 2) != model.Amount)
            {
                throw new ArgumentException("Enter a positive amount with at most two decimal places.", "Amount");
            }
            
            if (!ApplicationConstants.PaymentMethods.Contains(model.PaymentMethod))
            {
                throw new ArgumentException("Choose a supported payment method.", "PaymentMethod");
            }
         
            if (model.PaidAt.Year < 2000 || model.PaidAt > DateTime.Now.AddMinutes(5))
            {
                throw new ArgumentException("Enter a valid date that is not in the future.", "PaidAt");
            }

            if (model.Notes?.Length > EntityValidation.Expense.ExpenseNotesMax)
            {
                throw new ArgumentException($"Notes must be at most {EntityValidation.Expense.ExpenseNotesMax} characters.", "Notes");
            }

            expense.Title = model.Title.Trim();
            expense.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            expense.Amount = model.Amount;
            expense.PaidAt = model.PaidAt;
            expense.CategoryId = model.CategoryId;
            expense.PaymentMethod = model.PaymentMethod;
            expense.UserId = userId;
        }

        private static ExpenseResponse SummariseResponse(Expense expense) => new()
        {
            Id = expense.Id,
            Title = expense.Title,
            Amount = expense.Amount,
            PaidAt = expense.PaidAt,
            CategoryId = expense.CategoryId,
            CategoryName = expense.Category.Name,
            PaymentMethod = expense.PaymentMethod
        };
    }
}
