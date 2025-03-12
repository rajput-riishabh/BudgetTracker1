namespace BudgetTrackerWebApp.Models.ViewModels
{
    public class DashboardViewModel
    {
        public string? WelcomeMessage { get; set; }
        public decimal TotalExpensesThisMonth { get; set; }
        public decimal BudgetRemaining { get; set; }
        // ... Add other properties to match your API Dashboard response data ...
    }
}