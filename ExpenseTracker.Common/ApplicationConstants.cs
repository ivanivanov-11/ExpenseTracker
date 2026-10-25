namespace ExpenseTracker.Common
{
    using System.Globalization;
    
    public class ApplicationConstants
    {
        public static readonly string[] PaymentMethods = new[] {
            "Card",
            "Cash",
            "Bank Transfer",
            "Other"
        };

        public const string OtherCategoryName = "Other";
        public static readonly Dictionary<string, string> DefaultCategories = new Dictionary<string, string>
        {
            { "Housing", "Rent, utilities, and home services." },
            { "Food", "Groceries, restaurants, and coffee." },
            { "Transport", "Fuel, public transit, and rides." },
            { "Health", "Medical, pharmacy, and wellness." },
            { "Subscriptions", "Recurring digital and membership services." },
            { "Personal", "Clothing, hobbies, and personal care." },
            { "Travel", "Trips, accommodation, and activities." },
            { OtherCategoryName, "Anything that does not fit another category." }
        };

        public const int DefaultPageSize = 10;
    }
}
