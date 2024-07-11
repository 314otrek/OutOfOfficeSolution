using api.Data;
using api.Enums;
using api.Filters;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestController : ControllerBase
    {

        private readonly LeaveRequestService _leaveRequestService;


        public LeaveRequestController(LeaveRequestService leaveRequest)
        {
            _leaveRequestService = leaveRequest;
        }

        [HttpGet("leave-requests")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        public Task<List<LeaveRequest>> GetAllRequests()
        {
            return _leaveRequestService.getAllLeaveRequests();
        }

        [HttpGet("leave-request-sorted")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr", "v1-employee")]
        [ProducesResponseType(typeof(IEnumerable<LeaveRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetEmployees(
        [FromQuery] LeaveRequestSortOption sortBy = LeaveRequestSortOption.ID,
        [FromQuery] string sortOrder = "asc")
        {
            var leaveRequest = await _leaveRequestService.GetSortedLeaveRequestAsync(sortBy, sortOrder);
            return Ok(leaveRequest);
        }

        [HttpGet("/leave-request-filters")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr", "v1-employee")]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> FilterEmployee([FromQuery] LeaveRequestFilter leaveRequestFilter)
        {
            var leaveRequests = await _leaveRequestService.FilterEmployees(leaveRequestFilter);
            return Ok(leaveRequests);
        }

        [HttpPut("leave-requests/submit")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr", "v1-employee")]
        public async Task<ActionResult<LeaveRequest>> SubbmitLeaveRequest(int requestId, int approverId)
        {
            var leaveRequest = await _leaveRequestService.subbmitRequest(requestId, approverId);
            return Ok(leaveRequest);
        }
        [HttpPut("leave-requests/cancel")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr", "v1-employee")]
        public async Task<ActionResult<LeaveRequest>> CancelRequest(int requestId)
        {
            var leaveRequest = await _leaveRequestService.CancelRequest(requestId);
            return Ok(leaveRequest);
        }



        [HttpPost("leave-request")]
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr", "v1-employee")]
        public async Task<ActionResult<LeaveRequest>> CreateLeaveRequest([FromBody] LeaveRequest request)
        {
            var leaveRequest = await _leaveRequestService.createLeaveRequest(request);
            return Ok(leaveRequest);
        }

    }
}
