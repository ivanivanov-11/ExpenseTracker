namespace ExpenseTracker.Services.Core.Contracts
{
    using ExpenseTracker.Services.DTOs.Budget.Request;
    using ExpenseTracker.Services.DTOs.Budget.Response;
    
    public interface IBudgetService
    {
        Task<BudgetResponse> GetCurrentAsync(string userId, DateTime now);
        Task SaveAsync(BudgetRequest request, string userId, DateTime now);
    }
}
