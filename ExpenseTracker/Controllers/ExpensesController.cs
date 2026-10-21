namespace ExpenseTracker.Web.Controllers
{
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Expense.Request;
    using ExpenseTracker.ViewModels.Category;
    using ExpenseTracker.ViewModels.Expense;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Authorize]
    public class ExpensesController(IExpenseService expenseService, ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index([Bind(Prefix = "Filter")] ExpenseFilterViewModel filter)
        {
            if (filter.FromDateTime > filter.ToDateTime)
            {
                ModelState.AddModelError("Filter.ToDateTime", "The end date must be on or after the start date.");
            }

            string userId = GetUserId();

            var result = await expenseService.GetIndexAsync(userId, new ExpenseFilterRequest()
            {
                Search = filter.Search,
                CategoryId = filter.CategoryId,
                FromDateTime = filter.FromDateTime,
                ToDateTime = filter.ToDateTime
            });

            ExpenseIndexViewModel expenseIndexViewModel = new ExpenseIndexViewModel()
            {
                Filter = filter,
                Total = result.Total,
                Expenses = result.Expenses
                .Select(item => new ExpenseRowViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Amount = item.Amount,
                    PaidAt = item.PaidAt,
                    CategoryName = item.CategoryName,
                    PaymentMethod = item.PaymentMethod
                }).ToList(),
                Categories = (await categoryService.GetAllAsync(userId))
                .Select(item => new CategoryViewModel()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    isDefault = item.isDefault,
                    ExpenseCount = item.ExpenseCount
                }).ToList()
            };

            return View(expenseIndexViewModel);
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim not found.");
    }
}
