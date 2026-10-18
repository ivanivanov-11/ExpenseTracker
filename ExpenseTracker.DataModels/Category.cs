namespace ExpenseTracker.DataModels
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Common.EntityValidation.Category;
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(CategoryNameMax, MinimumLength = CategoryDescriptionMin, ErrorMessage = "Name must be between {2} and {1} characters")]
        public string Name { get; set; } = null!;

        [StringLength(CategoryDescriptionMax, MinimumLength = CategoryDescriptionMin, ErrorMessage = "Description must be between {2} and {1} characters")]
        public string? Description { get; set; }

        public bool isDefault { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual IdentityUser User { get; set; } = null!;


        public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    }
}
