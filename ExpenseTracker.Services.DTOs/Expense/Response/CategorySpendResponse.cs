namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class CategorySpendResponse
    {
        public string CategoryName { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
