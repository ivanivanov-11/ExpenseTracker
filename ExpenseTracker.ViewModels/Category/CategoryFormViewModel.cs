namespace ExpenseTracker.ViewModels.Category
{
    using System.ComponentModel.DataAnnotations;
    using static Common.EntityValidation;

    public class CategoryFormViewModel
    {
        [Required]
        [StringLength(Category.CategoryNameMax, MinimumLength = Category.CategoryNameMin)]
        public string Name { get; set; } = string.Empty;


        [StringLength(Category.CategoryDescriptionMax)]
        public string? Description { get; set; }
    }
}
