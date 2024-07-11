using api.Data;
using api.Models;


namespace api.Repositories
{
    public class EmployeeProjectRepository : IEmployeeProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeProject?> AddAsync(EmployeeProject employeeProject)
        {
            var entry = await _context.EmployeeProjects.AddAsync(employeeProject);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
