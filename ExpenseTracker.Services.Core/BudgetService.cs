namespace ExpenseTracker.Services.Core
{
    using ExpenseTracker.Common;
    using ExpenseTracker.Data;
    using ExpenseTracker.DataModels;
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Budget.Request;
    using ExpenseTracker.Services.DTOs.Budget.Response;
    using Microsoft.EntityFrameworkCore;

    public class BudgetService(ExpenseDbContext dbContext) : IBudgetService
    {
        public async Task<BudgetResponse> GetCurrentAsync(string userId, DateTime now)
        {
            Budget? budget = await dbContext.Budgets
                .AsNoTracking()
                .FirstOrDefaultAsync(item =>
                item.UserId == userId && item.Year == now.Year && item.Month == now.Month);

            BudgetResponse response = new BudgetResponse
            {
                Year = now.Year,
                Month = now.Month,
                MonthlyLimit = budget?.MonthlyLimit ?? 0
            };

            return response;
        }

        public async Task SaveAsync(BudgetRequest request, string userId, DateTime now)
        {
            decimal budgetAmountMax = decimal.Parse(EntityValidation.Budget.BudgetAmountMax);

            if (request.MonthlyLimit <= 0 || request.MonthlyLimit > budgetAmountMax || decimal.Round(request.MonthlyLimit, 2) != request.MonthlyLimit)
                throw new ArgumentException("Enter a positive budget with maximum two decimal places.");

            var budget = await dbContext.Budgets
                .FirstOrDefaultAsync(item => item.UserId == userId && item.Year == now.Year && item.Month == now.Month);

            if (budget is null)
            {
                Budget budgetToAdd = new Budget
                {
                    UserId = userId,
                    Year = now.Year,
                    Month = now.Month,
                    MonthlyLimit = request.MonthlyLimit
                };

                dbContext.Budgets.Add(budgetToAdd);
            }
            else
            {
                budget.MonthlyLimit = request.MonthlyLimit;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
