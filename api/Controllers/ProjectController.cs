using api.Data;
using api.dto;
using api.Enums;
using api.Filters;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using YourProject.Enums;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            this._projectService = projectService;
        }

        [HttpGet("projects")]
        [SwaggerGroupAttribute("v1-pr-manager")]
        public async Task<ActionResult<List<Project>>> GetAllProjects()
        {
                var projects = await _projectService.listAllProjects();
                return Ok(projects);
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("project-sorted")]
        [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects(
        [FromQuery] ProjectSortOption sortBy = ProjectSortOption.ID,
        [FromQuery] string sortOrder = "asc")
        {
            var project = await _projectService.GetSortedProjectsAsync(sortBy, sortOrder);
            return Ok(project);
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("/project-filters")]
        public async Task<ActionResult<IEnumerable<Project>>> FilterProject([FromQuery] ProjectFilter projectFilter)
        {
            var projects = await _projectService.FilterProjects(projectFilter);
            return Ok(projects);
        }

        [HttpPut]
        [SwaggerGroupAttribute("v1-pr-manager")]
        public async Task<ActionResult<Project>> DeactiveProject(int id)
        {
            var project = await _projectService.DeactiveProject(id);
            return Ok(project);
        }


        [HttpPost("project")]
        [SwaggerGroupAttribute("v1-pr-manager")]
        public async Task<ActionResult<Project>> createProject(Project project)
        {
                    var Project = await _projectService.createProject(project);
                    return Ok(Project);          
        }

        public async Task<ActionResult<Project>> UpdatePorject(int id, [FromBody]ProjectDto project)
        {
            var updatedProject = _projectService.UpdateProject(id, project);
            return Ok(updatedProject);
        }

        

    }
}
