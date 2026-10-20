namespace ExpenseTracker.Services.Core.Contracts
{
    using ExpenseTracker.Services.DTOs.Expense.Request;
    using ExpenseTracker.Services.DTOs.Expense.Response;

    public interface IExpenseService
    {
        Task<DashboardResponse> GetDashboardAsync(string userId, DateTime now);
        Task<ExpensePageResponse> GetIndexAsync(string userId, ExpenseFilterRequest filter);
        Task<ExpenseResponse?> GetDetailsAsync(Guid id, string userId);
        Task<Guid> CreateAsync(ExpenseRequest request, string userId);
        Task<bool> UpdateAsync(Guid id, ExpenseRequest request, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
    }
}
