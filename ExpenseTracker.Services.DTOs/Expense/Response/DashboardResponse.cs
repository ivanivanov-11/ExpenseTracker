using ExpenseTracker.Services.DTOs.Category.Response;

namespace ExpenseTracker.Services.DTOs.Expense.Response
{
    public class DashboardResponse
    {
        public string UserName { get; set; } = null!;
        public string MonthName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransaction { get; set; }
        public decimal? BudgetLimit { get; set; }
        public IEnumerable<ExpenseResponse> RecentExpenses { get; set; } = new List<ExpenseResponse>();
        public IEnumerable<CategorySpendResponse> CategoryBreakdown { get; set; } = new List<CategorySpendResponse>();
    }
}
