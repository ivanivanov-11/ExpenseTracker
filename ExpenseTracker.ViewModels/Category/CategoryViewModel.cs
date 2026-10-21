namespace ExpenseTracker.ViewModels.Category
{
    public class CategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool isDefault { get; set; }
        public int ExpenseCount { get; set; }
    }
}
