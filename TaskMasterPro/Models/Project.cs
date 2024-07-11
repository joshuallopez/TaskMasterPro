// Developed by Josue Lopez Lozano
// Last Updated March 30th, 2024

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMasterPro.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation property for related tasks
        public ICollection<Task> Tasks { get; set; }

        // Add a foreign key for User
        public int UserId { get; set; }

        // Navigation property for User
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
