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
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeService _employeService;


        public EmployeeController(EmployeeService employeService)
        {
            _employeService = employeService;
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("employees")]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employess = await _employeService.GetAllEmployeesAsync();

            return Ok(employess);
        }

        [SwaggerGroupAttribute("v1-employee")]
        [HttpGet("projects-of-employees")]
        public async Task<ActionResult<List<Project>>> getProjectOfEmployee([FromHeader]int id)
        {
             List<Project> projects = await _employeService.getProjectsOfEmployee(id);
            return Ok(projects);
        }


        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("employee/{id}")]
        public async Task<ActionResult<List<Employee>>> GetEmplyeeById(int employeId)
        {
            try
            {
                var employee = await _employeService.GetEmployeeByIdAsync(employeId);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("employee-sorted")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
        [FromQuery] EmployeeSortOption sortBy = EmployeeSortOption.ID,
        [FromQuery] string sortOrder = "asc")
        {
            var employees = await _employeService.GetSortedEmployeesAsync(sortBy, sortOrder);
            return Ok(employees);
        }


        [HttpGet("/employee-filters")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        public async Task<ActionResult<IEnumerable<Employee>>> FilterEmployee([FromQuery] EmployeeFilter employeeFilter)
        {
            var employees = await _employeService.FilterEmployees(employeeFilter);
            return Ok(employees);
        }

        [HttpGet("employee-search-by-fullName")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        public async Task<ActionResult<IEnumerable<Employee>>> SearchByFullName(string fullName)
        {
            var employee = await _employeService.SearchByFullName(fullName);
            return Ok(employee);
        }


        [HttpPut("add-employee-to-project")]
        [SwaggerGroupAttribute("v1-pr-manager")]
        public async Task<ActionResult<Employee>> AddEmployeeToProject(int projectId, int employeId)
        {
            try
            {
                var Employee = await _employeService.AddEmployeeToProject(projectId, employeId);
                return Ok(Employee);
            }
            catch (Exception ex)
            {
                throw new Exception("Bad Request");
            }
        }



        [HttpPut]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        public async Task<ActionResult<Employee>> InActivaeEmployee(int id)
        {
            var employee = await _employeService.UnActivateEmployee(id);
            return Ok(employee);
        }

        [HttpPut("update-employee")]
        [SwaggerGroupAttribute( "v1-hr")]
        public async Task<ActionResult<Employee>> UpdateEmployee(EmployeeDTO employeeDto, int id)
        {
            var employee = await _employeService.UpdateEmployee(employeeDto,id);
            return Ok(employee);
        }


        [HttpPost("employee")]
        [SwaggerGroupAttribute("v1-hr")]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            var Project = await _employeService.CreateEmploye(employee);
            return Ok(Project);
        }

    }



    
}
