using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace api.Models
{
    public class EmployeeProject
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [JsonIgnore]
        public  Employee Employee { get; set; }

        [Required]
        [JsonIgnore]
        public int ProjectId { get; set; }
        [JsonIgnore]
        public virtual Project Project { get; set; }
    }
}
