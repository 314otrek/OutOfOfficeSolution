using api.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class ApprovalRequest
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ApproverId { get; set; }

        [Required]
        public int LeaveRequestId { get; set; }

        [Required]
        public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.New;

        public string? Comment { get; set; }
        [JsonIgnore]
        public virtual Employee? Approver { get; set; }
        [JsonIgnore]
        public virtual LeaveRequest? LeaveRequest { get; set; }
    }
}
