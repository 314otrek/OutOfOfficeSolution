using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;
using api.Enums;

namespace api.Models
{
    public class Project
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public ProjectType ProjectType { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [Required]
        public int ProjectManagerId { get; set; }

        public string? Comment { get; set; }

        [Required]
        public ProjectStatus Status { get; set; }
        [JsonIgnore]
        public virtual Employee? ProjectManager { get; set; }

        
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }
}
