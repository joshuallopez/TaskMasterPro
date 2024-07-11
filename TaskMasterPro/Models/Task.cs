// Developed by Josue Lopez Lozano
// Last Updated March 30th, 2024

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMasterPro.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsComplete { get; set; }

        // Foreign key for Project
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        // Add a foreign key for User
        public int UserId { get; set; }

        // Navigation property for User
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
