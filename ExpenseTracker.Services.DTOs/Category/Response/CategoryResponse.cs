namespace ExpenseTracker.Services.DTOs.Category.Response
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool isDefault { get; set; }
        public int ExpenseCount { get; set; }
    }
}
