using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ICollection<ExpenseResponse> RecentExpenses { get; set; } = new List<ExpenseResponse>();
        public ICollection<CategorySpendResponse> CategoryBreakdown { get; set; } = new List<CategorySpendResponse>();
    }
}
