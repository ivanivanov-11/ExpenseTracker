namespace ExpenseTracker.ViewModels.Dashboard
{
    using ExpenseTracker.ViewModels.Category;
    using ExpenseTracker.ViewModels.Expense;

    public class DashboardViewModel
    {
        // trusted -> untrusted
        public string UserName { get; set; } = null!;
        public string MonthName { get; set; } = null!;
        public decimal TotalSpent { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransaction { get; set; }
        public decimal? BudgetLimit { get; set; }
        public decimal BudgetRemaining => BudgetLimit.HasValue ? BudgetLimit.Value - TotalSpent : 0;
        public List<ExpenseRowViewModel> RecentExpenses { get; set; } = [];
        public List<CategorySpendViewModel> CategoryBreakdown { get; set; } = [];
    }
}