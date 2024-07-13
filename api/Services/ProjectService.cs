using api.Data;
using api.dto;
using api.Enums;
using api.Filters;
using api.Models;
using api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.Services
{
    public class ProjectService
    {

        private readonly IProjectRepository _projectRepository;
        private readonly EmployeeService _employeeService;
        private readonly ApplicationDbContext _context;



        public ProjectService(IProjectRepository projectRepository, EmployeeService employeeService, ApplicationDbContext context)
        {
            _projectRepository = projectRepository;
            _employeeService = employeeService;
            _context = context;
        }


        public async Task<bool> addProjectToEmployee(int employeeId, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                throw new ArgumentException("Project not Found");
            }
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
            {
                throw new ArgumentException("Employee not Found");
            }
            if (employee.Position == Position.PROJECT_MANAGER && employee.ID != project.ProjectManagerId)
            {
                throw new ArgumentException("Project already has a project manager");
            }
            if (employee.Projects.Contains(project))
            {
                throw new Exception("The employee already take part in this project");
            }
            employee.Projects.Add(project);

            var newAssignment = new EmployeeProject
            {
                EmployeeId = employeeId,
                ProjectId = projectId
            };

            _context.EmployeeProjects.Add(newAssignment);
            await _projectRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> removeEmployeeFromProject(int projId, int empId)
        {
            var project = await _projectRepository.GetByIdAsync(projId);
            if (project == null)
            {
                throw new ArgumentException("Project not Found");
            }
            var employee = await _employeeService.GetEmployeeByIdAsync(empId);
            if (employee == null)
            {
                throw new ArgumentException("Employee not Found");
            }

            employee.Projects.Remove(project);
            var newAssignment = new EmployeeProject
            {
                EmployeeId = empId,
                ProjectId = projId
            };

            _context.EmployeeProjects.Add(newAssignment);
            await _projectRepository.SaveChangesAsync();
            return true;
        }


        public async Task<Project> getProjectById(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project != null)
            {
                return project;
            }
            throw new ArgumentException("Project not found!");
        }

        public async Task<List<Project>> listAllProjects()
        {
            var projList = await _projectRepository.GetAllAsync();
            return projList;
        }

        public async Task<Project> UpdateProject(int id, ProjectDto updateProjectDto)
        {

            var project= await _projectRepository.GetByIdAsync(id);
            if(project == null)
                throw new ArgumentException("Project not found!");

            if (updateProjectDto.ProjectType != null)
            {
                project.ProjectType = updateProjectDto.ProjectType.Value;
            }
            if (updateProjectDto.StartDate != null)
            {
                project.StartDate = updateProjectDto.StartDate.Value;
            }
            project.EndDate = updateProjectDto.EndDate.Value;

            if(updateProjectDto.ProjectManagerId!=null || updateProjectDto.ProjectManagerId!= project.ProjectManagerId)
            {
                int value = (int)updateProjectDto.ProjectManagerId;

                var emp = _employeeService.GetEmployeeByIdAsync(value);
                if(emp.Result.Position != Enums.Position.PROJECT_MANAGER)
                {
                    throw new ArgumentException("Provided project manager does not have Project Manager Rolse");
                }

                project.ProjectManagerId = updateProjectDto.ProjectManagerId.Value;
            }

            if (updateProjectDto.Comment != null)
            {
                project.Comment = updateProjectDto.Comment;
            }
            if (updateProjectDto.Status != null)
            {
                project.Status = updateProjectDto.Status.Value;
            }

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();
            return project;
        }


        public async Task<List<int>> GetProjectsIdOfEmployee(int employeeId)
        {
            Employee employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            return employee.EmployeeProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId)
            .ToList();
        }

        public async Task<Project> createProject(Project project)
        {
            Project project1 = new Project();


            project1.ProjectType = project.ProjectType;
            project1.StartDate = project.StartDate;
            project1.EndDate = project.EndDate;

            var projManager = _employeeService.GetEmployeeByIdAsync(project.ProjectManagerId);
            if(projManager == null || projManager.Result.Position!= Position.PROJECT_MANAGER)
            {
                throw new ArgumentException("Provided Employee does not have Project Manager Position ");
            }

            project1.ProjectManagerId = project.ProjectManagerId;
            project1.Comment = project.Comment;
            project1.Status = project.Status;
            await _employeeService.AddEmployeeToProject(project1.ID, project.ProjectManagerId);
                  

            return await _projectRepository.AddAsync(project1);
        }

        public async Task<IEnumerable<Project>> GetSortedProjectsAsync(ProjectSortOption sortBy, string sortOrder)
        {
            var projects = await _projectRepository.GetAllAsync();

            Func<Project, object> orderByFunc = sortBy switch
            {
                ProjectSortOption.ProjectType => e => e.ProjectType,
                ProjectSortOption.StartDate => e => e.StartDate,
                ProjectSortOption.EndDate => e => e.EndDate,
                _ => e => e.ID
            };

            var sortedProjects = sortOrder.ToLower() == "desc"
                ? projects.OrderByDescending(orderByFunc)
                : projects.OrderBy(orderByFunc);
            
            
            return sortedProjects;
        }

        public async Task<Project> DeactiveProject(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                throw new ArgumentException("Project not found!");
            }
            project.Status = ProjectStatus.Inactive;
            await _projectRepository.UpdateAsync(project); 
            return project;
        }

        public async Task<IEnumerable<Project>> FilterProjects(ProjectFilter filter)
        {
            return await _projectRepository.FilterProjectsAsync(filter);
        }

    }

}
