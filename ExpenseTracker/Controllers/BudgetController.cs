namespace ExpenseTracker.Web.Controllers
{
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Budget.Request;
    using ExpenseTracker.ViewModels.Budget;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Authorize]
    public class BudgetController(IBudgetService budgetService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var result = await budgetService.GetCurrentAsync(GetUserId(), DateTime.Now);
            return View(new BudgetViewModel { Year = result.Year, Month = result.Month, MonthlyLimit = result.MonthlyLimit });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BudgetViewModel model)
        {
            var now = DateTime.Now;
            model.Year = now.Year;
            model.Month = now.Month;
            if (!ModelState.IsValid) return View(model);
            try
            {
                await budgetService.SaveAsync(new BudgetRequest { MonthlyLimit = model.MonthlyLimit }, GetUserId(), now);
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(nameof(model.MonthlyLimit), exception.Message);
                return View(model);
            }
            TempData["Toast"] = $"Budget saved for {model.MonthName}.";
            return RedirectToAction(nameof(Index), "Home");
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim not found.");

    }
}
