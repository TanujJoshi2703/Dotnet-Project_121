using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;   // initialized to avoid nullability warning

        [Required]
        [Range(0.01, 10000000)]
        public decimal Amount { get; set; }

        [Required, StringLength(50)]
        public string Category { get; set; } = string.Empty; // initialized

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Notes can be null/empty, so make nullable
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Single-user app: optional user id (nullable)
        public string? UserId { get; set; }
    }
}
