namespace ExpenseTracker.ViewModels.Expense
{
    using ExpenseTracker.ViewModels.Category;
    public class ExpenseIndexViewModel
    {
        public ExpenseFilterViewModel Filter { get; set; } = new ExpenseFilterViewModel();
        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public IEnumerable<ExpenseRowViewModel> Expenses { get; set; } = new List<ExpenseRowViewModel>();
        public decimal Total { get; set; }
    }
}
