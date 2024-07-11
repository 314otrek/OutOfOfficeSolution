using api.Filters;
using api.Models;

public interface IEmployeeRepository
{
    Task<Employee> GetByIdAsync(int id);
    Task<List<Employee>> GetAllAsync();
    Task<Employee> AddAsync(Employee employee);
    Task SaveChangesAsync();
    Task UpdateAsync(Employee employee);
    Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilter filter);

    Task<IEnumerable<Employee>> SearchByfullNameAsync(string fullName);

}