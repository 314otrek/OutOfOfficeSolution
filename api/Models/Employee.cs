using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using YourProject.Enums;
using api.Enums;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public Subdivision Subdivision { get; set; }

        [Required]
        public Position Position{ get; set; }

        [Required]
        public EmployeeStatus Status { get; set; }

       
        public int? PeoplePartnerId { get; set; }

        [Required]
        public int outOfOfficeBalance { get; set; }

        public string? Photo { get; set; }
        [JsonIgnore]
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        [JsonIgnore]
        public ICollection<ApprovalRequest> AprovalRequest { get; set; } = new List<ApprovalRequest>();
        [JsonIgnore]
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
        [JsonIgnore]
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
