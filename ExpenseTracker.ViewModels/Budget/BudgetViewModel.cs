namespace ExpenseTracker.ViewModels.Budget
{
    using ExpenseTracker.Common;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using System.ComponentModel.DataAnnotations;

    public class BudgetViewModel
    {
        [Required]
        [Range(typeof(decimal), EntityValidation.Budget.BudgetAmountMin, EntityValidation.Budget.BudgetAmountMax)]
        [Display(Name = "Monthly spending limit")]
        public decimal MonthlyLimit { get; set; }

        [BindNever]
        public int Year { get; set; } = DateTime.Now.Year;
        [BindNever]
        public int Month { get; set; } = DateTime.Now.Month;

        public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

    }
}
