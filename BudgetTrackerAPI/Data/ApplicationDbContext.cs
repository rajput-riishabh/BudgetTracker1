using BudgetTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetTrackerAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Income> Incomes { get; set; } // **Add DbSet for Income**
        public DbSet<Saving> Savings { get; set; } // **Add DbSet for Saving**
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure unique index for Category Name per User(using Fluent API -alternative to Data Annotation in Category.cs)
             modelBuilder.Entity<Category>()
                 .HasIndex(c => new { c.Name, c.UserId })
                 .IsUnique()
                 .HasFilter("[UserId] IS NOT NULL"); // Apply filter for user-defined categories only

            // Seed Predefined Categories - moved to Database Initializer (see later step)
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Food", IsPredefined = true },
                new Category { CategoryId = 2, Name = "Transportation", IsPredefined = true },
                new Category { CategoryId = 3, Name = "Entertainment", IsPredefined = true },
                new Category { CategoryId = 4, Name = "Utilities", IsPredefined = true }
            );
        }
    }
}