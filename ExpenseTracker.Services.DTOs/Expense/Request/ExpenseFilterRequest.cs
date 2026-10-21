namespace ExpenseTracker.Services.DTOs.Expense.Request
{
    public class ExpenseFilterRequest
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
    }
}
