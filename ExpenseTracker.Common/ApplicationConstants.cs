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

        public const int DefaultPageSize = 10;
    }
}
