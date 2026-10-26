namespace ExpenseTracker.Web.Controllers
{
    using ExpenseTracker.Services.Core.Contracts;
    using ExpenseTracker.Services.DTOs.Category.Request;
    using ExpenseTracker.Services.DTOs.Category.Response;
    using ExpenseTracker.ViewModels.Category;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [Authorize]
    public class CategoriesController(ICategoryService categoryService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            IEnumerable<CategoryResponse> categories = await categoryService.GetAllAsync(GetUserId());
            
            IEnumerable<CategoryViewModel> categoryViewModels = categories.Select(item => new CategoryViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                isDefault = item.isDefault,
                ExpenseCount = item.ExpenseCount
            }).ToList();

            return View(categoryViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CategoryRequest request = new CategoryRequest(){
                        Name = model.Name,
                        Description = model.Description
                    };

                    await categoryService.CreateAsync(request, GetUserId());

                    TempData["Toast"] = "Category added.";

                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException exception)
                {
                    ModelState.AddModelError(exception.ParamName ?? string.Empty, exception.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            CategoryResponse? category = await categoryService.GetAsync(id, GetUserId());
            if (category == null)
                {
                return NotFound();
            }

            CategoryViewModel categoryViewModel = new CategoryViewModel()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                isDefault = category.isDefault,
                ExpenseCount = category.ExpenseCount
            };

            return View(categoryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {

                CategoryDeleteResponse categoryDeleteResponse = await categoryService.DeleteAsync(id, GetUserId());
                int updated = categoryDeleteResponse.UpdatedExpensesCount;

                if (!categoryDeleteResponse.Success)
                {
                    return NotFound();
                }

                if (updated > 0)
                {
                    TempData["Toast"] = $"Category deleted. {updated} transactions have been moved to Other.";
                }
                else
                {
                    TempData["Toast"] = "Category deleted.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                var category = await categoryService.GetAsync(id, GetUserId());
                return category is null ? NotFound() : View(new CategoryViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    isDefault = category.isDefault,
                    ExpenseCount = category.ExpenseCount
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            CategoryResponse? category = await categoryService.GetAsync(id, GetUserId());
            if (category == null)
            {
                return NotFound();
            }

            CategoryFormViewModel categoryFormViewModel = new CategoryFormViewModel()
            {
                Name = category.Name,
                Description = category.Description
            };

            return View(categoryFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CategoryRequest request = new CategoryRequest()
                    {
                        Name = model.Name,
                        Description = model.Description
                    };

                    await categoryService.EditAsync(id, request, GetUserId());
                    TempData["Toast"] = "Category updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException exception)
                {
                    ModelState.AddModelError(exception.ParamName ?? string.Empty, exception.Message);
                }
            }
            return View(model);
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User ID claim not found.");

    }
}
