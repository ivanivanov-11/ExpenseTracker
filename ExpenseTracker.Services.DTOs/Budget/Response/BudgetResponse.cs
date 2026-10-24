namespace ExpenseTracker.Services.DTOs.Budget.Response
{
    public class BudgetResponse
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal MonthlyLimit { get; set; }
    }
}
