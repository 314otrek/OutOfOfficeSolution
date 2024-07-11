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
    public class ApprovalRequestController : ControllerBase
    {

        private readonly ApprovalRequestService _approvalRequestService;

        public ApprovalRequestController(ApprovalRequestService approvalRequestService)
        {
            _approvalRequestService = approvalRequestService;
        }
        
        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("approval-request")]
        public async Task<List<ApprovalRequest>> GetApproval()
        {
            return await _approvalRequestService.getAllApprovalRequests();
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpGet("approval-request-sorted")]
        [ProducesResponseType(typeof(IEnumerable<ApprovalRequest>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApprovalRequest>>> GetEmployees(
        [FromQuery] ApprovalRequestSortOption sortBy = ApprovalRequestSortOption.ID,
        [FromQuery] string sortOrder = "asc")
        {
            var approvalRequests = await _approvalRequestService.GetSortedApprovalRequestAsync(sortBy, sortOrder);
            return Ok(approvalRequests);
        }

        [SwaggerGroupAttribute("v1-pr-manager","v1-hr")]
        [HttpGet("/approval-request-filters")]
        public async Task<ActionResult<IEnumerable<ApprovalRequest>>> FilterApprovalRequests([FromQuery]ApprovalRequestFilter approvalRequestFilter)
        {
            var approvalRequests = await _approvalRequestService.FilterApprovalRequests(approvalRequestFilter);
            return Ok(approvalRequests);
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpPut("approval-equest")]
        public async Task<ActionResult<ApprovalRequest>> approveRequest(int requestId)
        {
            return await _approvalRequestService.approveRequest(requestId);
        }

        [SwaggerGroupAttribute("v1-pr-manager", "v1-hr")]
        [HttpPut("reject-request")]
        public async Task<ActionResult<ApprovalRequest>> approveRequest(int requestId, string comment)
        {
            return await _approvalRequestService.rejectRequest(requestId, comment);
        }

    }
}
