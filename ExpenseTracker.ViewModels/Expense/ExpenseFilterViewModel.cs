namespace ExpenseTracker.ViewModels.Expense
{
    using System.ComponentModel.DataAnnotations;
    using static ExpenseTracker.Common.EntityValidation.Expense;
    public class ExpenseFilterViewModel
    {
        // untrusted -> trusted!!!
        // description is larger than the title, so we can use the same validation consts
        [StringLength(ExpenseNotesMax, MinimumLength = ExpenseTitleMin, ErrorMessage = "Search must be less than {1} characters")]
        public string? Search { get; set; }

        public Guid? CategoryId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDateTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDateTime { get; set; }
    }
}
