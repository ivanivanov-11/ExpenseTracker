namespace ExpenseTracker.ViewModels.Category
{
    public class CategorySpendViewModel
    {
        public string CategoryName { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
