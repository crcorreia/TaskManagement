using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static TaskApi.Enums.Emuns;

namespace TaskApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime StratDate { get; set; }
        public int Duration { get; set; }
        [Required]
        public State State { get; set; }
        public User AssignedTo { get; set; }
        public DateTime? InProgressDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
