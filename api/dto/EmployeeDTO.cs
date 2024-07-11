using api.Enums;
using YourProject.Enums;

namespace api.dto
{
    public class EmployeeDTO
    {
        public string? FullName { get; set; }
        public Subdivision? Subdivision { get; set; }
        public Position? Position { get; set; }
        public EmployeeStatus? Status { get; set; }
        public int? PeoplePartnerId { get; set; }
        public int? OutOfOfficeBalance { get; set; }
        public string? Photo { get; set; }
    }
}
