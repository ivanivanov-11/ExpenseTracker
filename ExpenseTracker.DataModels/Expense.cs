namespace ExpenseTracker.DataModels
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Common.EntityValidation.Expense;

    public class Expense
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(ExpenseTitleMax, MinimumLength = ExpenseTitleMin, ErrorMessage = "Title must be between {2} and {1} characters")]
        public string Title { get; set; } = null!;

        [StringLength(ExpenseNotesMax, MinimumLength = ExpenseNotesMin, ErrorMessage = "Notes must be between {2} and {1} characters")]
        public string? Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string PaymentMethod { get; set; } = "Card";

        [Required]
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual Category Category { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual IdentityUser User { get; set; } = null!;
    }
}
