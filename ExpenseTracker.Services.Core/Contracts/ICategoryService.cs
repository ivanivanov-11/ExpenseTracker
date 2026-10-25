namespace ExpenseTracker.Services.Core.Contracts
{
    using ExpenseTracker.Services.DTOs.Category.Request;
    using ExpenseTracker.Services.DTOs.Category.Response;
    
    public interface ICategoryService
    {
        Task InitializeAsync(string userId);
        Task<IEnumerable<CategoryResponse>> GetAllAsync(string userId);
        Task<CategoryResponse?> GetAsync(Guid id, string userId);
        Task<Guid> CreateAsync(CategoryRequest request, string userId);
        Task<CategoryDeleteResponse> DeleteAsync(Guid id, string userId);
    }
}
