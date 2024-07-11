using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore;
using api.Filters;
using Microsoft.Data.Sqlite;

namespace api.Repositories
{
    public class EmployeeRepository  : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmployeeProjects)
                .FirstOrDefaultAsync(e => e.ID == id);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
           _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> FilterEmployeesAsync(EmployeeFilter filter)
        {
            var query = "SELECT * FROM Employees WHERE 1=1 ";
            var parameters = new List<SqliteParameter>();

            if (filter.Subdivision.HasValue)
            {
                query += " AND Subdivision = @Subdivision";
                parameters.Add(new SqliteParameter("@Subdivision", filter.Subdivision.Value));
                await Console.Out.WriteLineAsync(filter.Subdivision.Value.ToString());
            }

            if (filter.Position.HasValue)
            {
                query += " AND Position = @Position";
                parameters.Add(new SqliteParameter("@Position", filter.Position.Value));
            }

            if (filter.EmployeeStatus.HasValue)
            {
                query += " AND Status = @Status";
                parameters.Add(new SqliteParameter("@Status", filter.EmployeeStatus.Value));
            }

            return await _context.Employees.FromSqlRaw(query, parameters.ToArray()).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchByfullNameAsync(string fullName)
        {
            var query = "SELECT * FROM Employees WHERE FullName Like @FullName";
            var parameters = new SqliteParameter[]
            {
            new SqliteParameter("@FullName", $"%{fullName}%")
            };

            return await _context.Employees.FromSqlRaw(query, parameters.ToArray()).ToListAsync();
        }

    } 
}
