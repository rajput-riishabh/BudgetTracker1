﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetTrackerAPI.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")] // For currency precision
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)] // Specify Date type
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)] // Specify Date type
        public DateTime EndDate { get; set; }
    }
}