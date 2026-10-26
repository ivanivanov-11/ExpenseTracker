namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class ExpensePageResponse
    {
        public IEnumerable<ExpenseResponse> Expenses { get; set; } = new List<ExpenseResponse>();
        public decimal Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
