﻿namespace BudgetTrackerAPI.Models.DTOs
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}