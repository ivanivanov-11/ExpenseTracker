namespace ExpenseTracker.Services.Core.Contracts
{
    using ExpenseTracker.Services.DTOs.Category.Response;
    
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync(string userId);
    }
}
