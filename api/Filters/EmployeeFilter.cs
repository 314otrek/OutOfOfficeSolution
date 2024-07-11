using api.Enums;
using YourProject.Enums;

namespace api.Filters
{
    public class EmployeeFilter
    {
        public Subdivision? Subdivision { get; set; }
        public Position? Position { get; set; }
        public EmployeeStatus? EmployeeStatus { get; set; }
    }
}
