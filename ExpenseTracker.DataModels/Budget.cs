namespace ExpenseTracker.DataModels
{
    using ExpenseTracker.Common;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Common.EntityValidation.Budget;

    public class Budget
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(BudgetYearMin, BudgetYearMax)]
        public int Year { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        [Range(typeof(decimal), EntityValidation.Budget.BudgetAmountMin, EntityValidation.Budget.BudgetAmountMax)]
        [Column(TypeName = "decimal(12,2)")]
        public decimal MonthlyLimit { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual IdentityUser User { get; set; } = null!;
    }
}
