using api.Data;
using api.Enums;
using api.Filters;
using api.Models;
using api.Repositories;

namespace api.Services
{
    public class ApprovalRequestService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly LeaveRequestService _leaveRequestService;
        private readonly EmployeeService _employeeService;

        public ApprovalRequestService(IApprovalRepository approvalRepository,
            IEmployeeRepository employeeRepository,
            LeaveRequestService leaveRequestService,
            EmployeeService employeeService)
        {
            _approvalRepository = approvalRepository;
            _employeeRepository = employeeRepository;
            _leaveRequestService = leaveRequestService;
            _employeeService = employeeService;
        }


        public async Task<ApprovalRequest> addAproval(ApprovalRequest approvalRequest)
        {
            if(approvalRequest == null)
            {
                throw new InvalidDataException("Provided Approval is null");
            }

            await _approvalRepository.Add(approvalRequest);
            return approvalRequest;
        }
        public async Task<ApprovalRequest> getById(int id)
        {
            var approval = await _approvalRepository.GetById(id);
            if (approval == null)
                throw new KeyNotFoundException("Approval Request with this Id not found");

            return approval;
        }

        public async Task<List<ApprovalRequest>> getAllApprovalRequests()
        {
            return await _approvalRepository.GetAll();
        }

        public async Task<ApprovalRequest> approveRequest(int requestId)
        {
            var approveReq = await getById(requestId);
            if (approveReq == null)
            {
                throw new KeyNotFoundException("Approval Request with this Id not found");

            }
            var leaveReq = await _leaveRequestService.getById(approveReq.LeaveRequestId);
            int employeeId = leaveReq.EmployeeId;
            if (employeeId == 0)
            {
                throw new KeyNotFoundException("Id is 0");
            }


            var employee = _employeeService.GetEmployeeByIdAsync(employeeId).Result;

            if (employee == null)
            {
                throw new KeyNotFoundException("Employee not found");
            }

            int period = leaveReq.EndDate.DayNumber - leaveReq.StartDate.DayNumber;
            if (period > employee.outOfOfficeBalance)
            {
                throw new Exception("Not enough out of office days");
            }

            employee.outOfOfficeBalance -= period;
            approveReq.Status = Enums.ApprovalRequestStatus.Approved;
            await _employeeRepository.UpdateAsync(employee);
            await _approvalRepository.Update(approveReq);
            await _approvalRepository.SaveChangesAsync();
            await _employeeRepository.SaveChangesAsync();
            return approveReq;
        }

        public async Task<ApprovalRequest> rejectRequest(int requestId, string comment)
        {
            var approveReq = await getById(requestId);
            if (approveReq == null)
            {
                throw new KeyNotFoundException("Approval Request with this Id not found");

            }
            approveReq.Status = Enums.ApprovalRequestStatus.Rejected;
            approveReq.Comment = comment;
            await _approvalRepository.Update(approveReq);
            await _approvalRepository.SaveChangesAsync();
            return approveReq;
        }

        public async Task<IEnumerable<ApprovalRequest>> GetSortedApprovalRequestAsync(ApprovalRequestSortOption sortBy, string sortOrder)
        {
            var approval = await _approvalRepository.GetAll();

            Func<ApprovalRequest, object> orderByFunc = sortBy switch
            {
                ApprovalRequestSortOption.ApproverId => e => e.ApproverId,
                ApprovalRequestSortOption.LeaveRequestId => e => e.LeaveRequestId,
                ApprovalRequestSortOption.Status => e => e.Status,
                _ => e => e.ID
            };

            var sortedApprovals = sortOrder.ToLower() == "desc" 
                ? approval.OrderByDescending(orderByFunc) 
                : approval.OrderBy(orderByFunc);

            return sortedApprovals;
        
        }

        public async Task<IEnumerable<ApprovalRequest>> FilterApprovalRequests(ApprovalRequestFilter filter)
        {
            return await _approvalRepository.FilterApprovalRequestsAsync(filter);
        }




    }
}
