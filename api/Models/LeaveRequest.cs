using api.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class LeaveRequest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public string? Comment { get; set; }

        [Required]
        public LeaveRequestStatus Status { get; set; }

        [JsonIgnore]
        public virtual Employee? Employee { get; set; } = null!;
        [JsonIgnore]
        public virtual ApprovalRequest? ApprovalRequest { get; set; } = null!;
        
    }
}
