namespace ExpenseTracker.DataModels
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using static Common.EntityValidation.Budget;

    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(BudgetYearMin, BudgetYearMax)]
        public int Year { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        [Range(typeof(decimal), "0.01", "250000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyLimit { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public virtual IdentityUser User { get; set; } = null!;
    }
}
