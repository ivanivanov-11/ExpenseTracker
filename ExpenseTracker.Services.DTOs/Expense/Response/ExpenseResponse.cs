namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class ExpenseResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}
