using api.Data;
using api.Enums;
using api.Models;
using Microsoft.EntityFrameworkCore;
using YourProject.Enums;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;

    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        // Upewnij się, że baza danych jest utworzona
        await _context.Database.EnsureCreatedAsync();

        // Sprawdź, czy baza danych jest pusta
        if (!await _context.Employees.AnyAsync() && !await _context.Projects.AnyAsync() && !await _context.LeaveRequests.AnyAsync())
        {
            // Dodaj przykładowych pracowników
            var employees = new List<Employee>
            {
                new Employee { FullName = "Anna Nowak", Position = Position.HR_MANAGER, Status = EmployeeStatus.Active,PeoplePartnerId=null,Subdivision=Subdivision.HR ,outOfOfficeBalance=20,Photo="" },
                new Employee { FullName = "Jan Kowalski", Position = Position.EMPLOYEE, Status = EmployeeStatus.Active,PeoplePartnerId=1,Subdivision=Subdivision.IT,outOfOfficeBalance=20,Photo="" },
                new Employee { FullName = "Piotr Wiśniewski", Position = Position.EMPLOYEE, Status = EmployeeStatus.Active, PeoplePartnerId=1,Subdivision=Subdivision.IT,outOfOfficeBalance=20,Photo=""  },
                new Employee { FullName = "Piotr Kowal", Position = Position.PROJECT_MANAGER, Status = EmployeeStatus.Active , PeoplePartnerId=1,Subdivision=Subdivision.IT, outOfOfficeBalance = 20,Photo=""},
                new Employee { FullName = "PAtrycja Kowal", Position = Position.HR_MANAGER, Status = EmployeeStatus.Inactive, PeoplePartnerId=1, Subdivision=Subdivision.HR,outOfOfficeBalance = 20,Photo=""}
            };

            await _context.Employees.AddRangeAsync(employees);

            // Dodaj przykładowe projekty
            var projects = new List<Project>
            {
                new Project { ProjectType = ProjectType.WEB_DEVELOPMENT, StartDate = new DateOnly(2024, 5, 11), Status = ProjectStatus.Active, ProjectManagerId=4,Comment="Simple Web Application" }
            };

            await _context.Projects.AddRangeAsync(projects);

            var leaves = new List<LeaveRequest>();

            await _context.LeaveRequests.AddRangeAsync(leaves);

            await _context.SaveChangesAsync();
        }
    }
}