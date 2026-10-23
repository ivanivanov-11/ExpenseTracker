namespace ExpenseTracker.ViewModels.Expense
{
    using ExpenseTracker.ViewModels.Category;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static ExpenseTracker.Common.EntityValidation.Expense;
    
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ExpenseTitleMax, MinimumLength = ExpenseTitleMin, ErrorMessage = "Title must be between {2} and {1} characters")]
        [Display(Name = "What was it for?")]
        public string Title { get; set; } = null!;

        [StringLength(ExpenseNotesMax, MinimumLength = ExpenseNotesMin, ErrorMessage = "Notes must be between {2} and {1} characters")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required]
        [Range(typeof(decimal), ExpenseAmountMin, ExpenseAmountMax, ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = DateTimeFormat)]
        [Display(Name = "Date")]
        public DateTime PaidAt { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Card";

        public IEnumerable<CategoryOptionViewModel> Categories { get; set; } = new List<CategoryOptionViewModel>();
    }
}
