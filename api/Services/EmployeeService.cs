using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;
using api.Repositories;
using api.Enums;
using YourProject.Enums;
using api.Filters;
using System.Diagnostics.Contracts;
using api.dto;

namespace api.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeProjectRepository _employeeProjectRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository,
            IEmployeeProjectRepository employeeProjectRepository,
            ILeaveRequestRepository leaveRequestRepository)
        {
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _employeeProjectRepository = employeeProjectRepository;
            _leaveRequestRepository = leaveRequestRepository;
        }

        public async Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId)
        {
            return await _leaveRequestRepository.GetByEmployeeId(employeeId);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<List<Project>> getProjectsOfEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee with id not found");
            }

            return employee.Projects.ToList();
            
        }

        public async Task<Employee> AddEmployeeAsync(Employee employee)
        {
            return await _employeeRepository.AddAsync(employee);
        }

        public  Task<Employee> GetEmployeeByIdAsync(int id)
        {
            var employee =  _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException("Employee with ID {id} not found.");
            }
            return employee;
        }

        public async Task<Employee> UnActivateEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
             if (employee== null)
            {
                throw new KeyNotFoundException("Employee with ID {id} not found.");
            }
            employee.Status = EmployeeStatus.Inactive;
            await _employeeRepository.UpdateAsync(employee);
            return employee;

        }

        public async Task<Employee> CreateEmploye(Employee employee)
        {

            Employee emp = new Employee();
            
                emp.ID = default;
            emp.FullName = employee.FullName;
            emp.Subdivision = employee.Subdivision;
            emp.Position = employee.Position;
            emp.outOfOfficeBalance = employee.outOfOfficeBalance;
            emp.Photo = employee.Photo;

            var hr= await _employeeRepository.GetByIdAsync(employee.PeoplePartnerId.Value);

            if (hr.Position != Position.HR_MANAGER)
            {
                throw new ArgumentException("People Partner need to has HR Manager position");
            }

            emp.PeoplePartnerId = employee.PeoplePartnerId;
            emp.Status = employee.Status;

            return await _employeeRepository.AddAsync(emp);
        }

        public async Task<Employee> UpdateEmployee(EmployeeDTO employeeDTO, int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if(employee != null) 
            {
                throw new KeyNotFoundException("Employee not found with provided id");
            }

            if (employeeDTO.Subdivision != null)
            {
                employee.Subdivision = employeeDTO.Subdivision.Value;
            }
            if(employeeDTO.Position!=null)
            {
                employee.Position = employeeDTO.Position.Value;
            }
            if(employeeDTO.Status != null)
            {
                employee.Status = employeeDTO.Status.Value;
            }

            var hr = await _employeeRepository.GetByIdAsync(employeeDTO.PeoplePartnerId.Value);

            if (hr.Position != Position.HR_MANAGER || hr == null)
            {
                throw new ArgumentException("Wrong poeple partener id People Partner ");
            }
            employee.PeoplePartnerId = employeeDTO.PeoplePartnerId.Value;

            if (employeeDTO.OutOfOfficeBalance != null)
            {
                employee.outOfOfficeBalance = employeeDTO.OutOfOfficeBalance.Value;
            }
            if(employeeDTO.Photo != null)
            {
                employee.Photo = employeeDTO.Photo;
            }

            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return employee;


        }

        public async Task<Employee> AddEmployeeToProject(int projectId, int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee with ID not found.");
            
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException("Project with ID not found.");

            if (employee.Position == Position.PROJECT_MANAGER || employee.ID == project.ProjectManagerId)
            {
                throw new ArgumentException("Project already has a project manager");
            }


                EmployeeProject entity = new()
            {
                Id = default,
                EmployeeId = employeeId,
                ProjectId = projectId,
            };

            if (employee.Projects.Contains(project)) {
                throw new Exception("The employee already take part in this project");    
            }
            employee.Projects.Add(project);
            await _employeeRepository.SaveChangesAsync();
            var res = await _employeeProjectRepository.AddAsync(entity);
            employee = await _employeeRepository.GetByIdAsync(employeeId);
            return employee;
        }


        public async Task<IEnumerable<Employee>> GetSortedEmployeesAsync(EmployeeSortOption sortBy, string sortOrder)
        {
            var employees = await _employeeRepository.GetAllAsync();

            // Okreœlamy funkcjê sortuj¹c¹ na podstawie wybranej opcji
            Func<Employee, object> orderByFunc = sortBy switch
            {
                EmployeeSortOption.FullName => e => e.FullName,
                EmployeeSortOption.Subdivision => e => e.Subdivision,
                EmployeeSortOption.Position => e => e.Position,
                EmployeeSortOption.Status => e => e.Status,
                EmployeeSortOption.OutOfOfficeBalance => e => e.outOfOfficeBalance,
                _ => e => e.ID,
            };

            // Sortujemy listê
            var sortedEmployees = sortOrder.ToLower() == "desc"
                ? employees.OrderByDescending(orderByFunc)
                : employees.OrderBy(orderByFunc);

            return sortedEmployees;
        }


        public async Task<IEnumerable<Employee>> FilterEmployees(EmployeeFilter filter)
        {
            return await _employeeRepository.FilterEmployeesAsync(filter);
        }


        public async Task<IEnumerable<Employee>> SearchByFullName(string fullName)
        {
            return await _employeeRepository.SearchByfullNameAsync(fullName);
        }
    }
}
