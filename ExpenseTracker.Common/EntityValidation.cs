namespace ExpenseTracker.Common
{
    public static class EntityValidation
    {
        public static class Category
        {
            public const int CategoryNameMin = 3;
            public const int CategoryNameMax = 30;

            public const int CategoryDescriptionMin = 10;
            public const int CategoryDescriptionMax = 300;
        }

        public static class Expense
        {
            public const int ExpenseTitleMin = 3;
            public const int ExpenseTitleMax = 50;

            public const int ExpenseNotesMin = 10;
            public const int ExpenseNotesMax = 300;

            public const string ExpenseAmountMin = "0.01";
            public const string ExpenseAmountMax = "1000000.00";

            public const string DateTimeFormat = "smalldatetime";
        }

        public static class Budget
        {
            public const int BudgetYearMin = 2000;
            public const int BudgetYearMax = 2026;
        }
    }
}
