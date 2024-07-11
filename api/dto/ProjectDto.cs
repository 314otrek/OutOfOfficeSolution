using api.Enums;

namespace api.dto
{
    public class ProjectDto
    {
        public ProjectType? ProjectType { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? ProjectManagerId { get; set; }
        public string? Comment { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
