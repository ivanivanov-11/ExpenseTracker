namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class ExpensePageResponse
    {
        public IEnumerable<ExpenseResponse> Expenses { get; set; } = new List<ExpenseResponse>();
        public decimal Total { get; set; }
    }
}
