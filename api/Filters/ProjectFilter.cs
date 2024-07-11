using api.Enums;

namespace api.Filters
{
    public class ProjectFilter
    {
        public ProjectType? ProjectType { get; set; }
        public ProjectStatus? ProjectStatus { get; set; }
    }
}
