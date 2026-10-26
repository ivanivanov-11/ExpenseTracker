namespace ExpenseTracker.Services.DTOs.Expense.Request
{
    using static ExpenseTracker.Common.ApplicationConstants;
    public class ExpenseFilterRequest
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public Guid? CategoryId { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
    }
}
