namespace ExpenseTracker.Web.Controllers
{
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Expense.Request;
    using ExpenseTracker.Services.DTOs.Expense.Response;
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

            ExpensePageResponse result = await expenseService.GetIndexAsync(userId, new ExpenseFilterRequest()
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ExpenseViewModel expenseViewModel = new ExpenseViewModel()
            {
                PaidAt = DateTime.Now,
                Categories = await GetCategoryOptionsAsync()
            };

            return View(expenseViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            model.Categories = await GetCategoryOptionsAsync();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input.");
                model.Categories = await GetCategoryOptionsAsync();

                return View(model);
            }


            if (!model.Categories.Any(category => category.Id == model.CategoryId) || model.CategoryId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Choose an existing category.");

                model.Categories = await GetCategoryOptionsAsync();

                return View(model);
            }

            try
            {
                ExpenseRequest request = new ExpenseRequest
                {
                    Title = model.Title,
                    Notes = model.Notes,
                    Amount = model.Amount,
                    PaidAt = model.PaidAt,
                    CategoryId = model.CategoryId,
                    PaymentMethod = model.PaymentMethod
                };

                await expenseService.CreateAsync(request, GetUserId());

                TempData["Toast"] = "Expense added successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(exception.ParamName ?? string.Empty, exception.Message);

                model.Categories = await GetCategoryOptionsAsync();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            ExpenseResponse? result = await expenseService.GetDetailsAsync(id, GetUserId());

            if (result == null)
            {
                return NotFound();
            }

            ExpenseDetailsViewModel expense = new ExpenseDetailsViewModel
            {
                Id = result.Id,
                Title = result.Title,
                Notes = result.Notes,
                Amount = result.Amount,
                PaidAt = result.PaidAt,
                CategoryName = result.CategoryName,
                PaymentMethod = result.PaymentMethod
            };

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ExpenseDetailsViewModel model)
        {
            bool isDeleted = await expenseService.DeleteAsync(model.Id, GetUserId());

            if (!isDeleted)
            {
                return NotFound();
            }

            TempData["Toast"] = "Expense deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            ExpenseResponse? result = await expenseService.GetDetailsAsync(id, GetUserId());

            if (result == null)
            {
                return NotFound();
            }

            ExpenseDetailsViewModel expense = new ExpenseDetailsViewModel
            {
                Id = result.Id,
                Title = result.Title,
                Notes = result.Notes,
                Amount = result.Amount,
                PaidAt = result.PaidAt,
                CategoryName = result.CategoryName,
                PaymentMethod = result.PaymentMethod
            };

            return View(expense);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            ExpenseResponse? result = await expenseService.GetDetailsAsync(id, GetUserId());

            if (result is null)
            {
                return NotFound();
            }

            ExpenseViewModel model = new ExpenseViewModel
            {
                Id = result.Id,
                Title = result.Title,
                Notes = result.Notes,
                Amount = result.Amount,
                PaidAt = result.PaidAt,
                CategoryId = result.CategoryId,
                PaymentMethod = result.PaymentMethod,
                Categories = await GetCategoryOptionsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseViewModel model)
        {
            model.Categories = await GetCategoryOptionsAsync();

            if (model.Id == Guid.Empty)
            {
                return NotFound();
            }

            if (!model.Categories.Any(category => category.Id == model.CategoryId) || model.CategoryId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Choose an existing category.");

                model.Categories = await GetCategoryOptionsAsync();

                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ExpenseRequest request = new ExpenseRequest
                    {
                        Title = model.Title,
                        Notes = model.Notes,
                        Amount = model.Amount,
                        PaidAt = model.PaidAt,
                        CategoryId = model.CategoryId,
                        PaymentMethod = model.PaymentMethod
                    };

                    bool updated = await expenseService.UpdateAsync(model.Id, request, GetUserId());

                    if (!updated)
                    {
                        return NotFound();
                    }

                    TempData["Toast"] = "Transaction updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException exception)
                {
                    ModelState.AddModelError(exception.ParamName ?? string.Empty, exception.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid input.");

                model.Categories = await GetCategoryOptionsAsync();

                return View(model);
            }
            return View(model);
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim not found.");

        private async Task<List<CategoryOptionViewModel>> GetCategoryOptionsAsync() =>
            (await categoryService.GetAllAsync(GetUserId()))
            .Select(item => new CategoryOptionViewModel { Id = item.Id, Name = item.Name }).ToList();
    }
}
