using api.Models;

namespace api.Repositories
{
    public interface IEmployeeProjectRepository
    {
        Task<EmployeeProject?> AddAsync(EmployeeProject employeeProject);
    }
}
