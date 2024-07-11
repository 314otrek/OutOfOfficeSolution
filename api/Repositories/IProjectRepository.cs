using api.Filters;
using api.Models;

namespace api.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(int id);
        Task<List<Project>> GetAllAsync();
        Task<Project> AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        Task<List<Project>> GetProjectsByEmployeeIdAsync(int employeeId);
        Task SaveChangesAsync();
        Task<IEnumerable<Project>> FilterProjectsAsync(ProjectFilter filter);
    }
}
