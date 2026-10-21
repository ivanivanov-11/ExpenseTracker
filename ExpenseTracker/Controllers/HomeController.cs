namespace ExpenseTracker.Controllers
{
    using ExpenseTracker.ViewModels;
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.ViewModels.Dashboard;
    using ExpenseTracker.ViewModels.Expense;
    using ExpenseTracker.ViewModels.Category;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Security.Claims;

    public class HomeController(IExpenseService expenseService) : Controller
    {
        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = GetUserId();
            var result = await expenseService.GetDashboardAsync(userId, DateTime.Now);

            DashboardViewModel dashboardViewModel = new DashboardViewModel()
            {
                UserName = result.UserName,
                MonthName = result.MonthName,
                TotalSpent = result.TotalSpent,
                TransactionCount = result.TransactionCount,
                AverageTransaction = result.AverageTransaction,
                BudgetLimit = result.BudgetLimit,

                RecentExpenses = result.RecentExpenses.Select(item => new ExpenseRowViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Amount = item.Amount,
                    PaidAt = item.PaidAt,
                    CategoryName = item.CategoryName,
                    PaymentMethod = item.PaymentMethod
                }).ToList(),

                CategoryBreakdown = result.CategoryBreakdown.Select(item => new CategorySpendViewModel()
                {
                    CategoryName = item.CategoryName,
                    Amount = item.Amount,
                    Percentage = item.Percentage
                }).ToList()
            };

            return View(dashboardViewModel);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim not found.");
    }
}
