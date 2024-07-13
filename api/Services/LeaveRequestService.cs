using api.Data;
using api.Enums;
using api.Filters;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace api.Services
{
    public class LeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly EmployeeService _employeeService;
        private readonly ApplicationDbContext _context;


        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository,
            EmployeeService employeeService,
            ApplicationDbContext context )
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeService = employeeService;
            _context = context;
        }

        public async Task<List<LeaveRequest>> getAllLeaveRequests()
        {
            return await  _leaveRequestRepository.GetAll();
            
        }

        public async Task<LeaveRequest> getById(int id)
        {
            var leaveRequest = await _leaveRequestRepository.GetById(id);
            if (leaveRequest == null)
            {
                throw new KeyNotFoundException("There is no LeaveRequest with this id");
            }

            return leaveRequest;
        }

        public async Task<LeaveRequest> createLeaveRequest(
            LeaveRequest leaveRequest)
        {
            Employee eployee = _employeeService.GetEmployeeByIdAsync(leaveRequest.EmployeeId).Result;

            LeaveRequest leave1 = new LeaveRequest
            {
                ID = default,
                EmployeeId = leaveRequest.EmployeeId,
                AbsenceReason = leaveRequest.AbsenceReason,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Comment = leaveRequest.Comment,
                Status = leaveRequest.Status,

            };
            eployee.LeaveRequests.Add(leave1);
            await _leaveRequestRepository.Add(leave1);
            await _leaveRequestRepository.SaveChangesAsync();

            return leave1;
        }

        public async Task<LeaveRequest> subbmitRequest(int requestId, int approverId)
        {
            var leaveReuest = await _leaveRequestRepository.GetById(requestId);
            if(leaveReuest == null)
                throw new KeyNotFoundException("There is no LeaveRequest with this id");

            var approver = await _employeeService.GetEmployeeByIdAsync(approverId)
                ?? throw new KeyNotFoundException(
                    "There is no Aprover with this id");

            if(approver.Position!= Enums.Position.PROJECT_MANAGER && approver.Position != Enums.Position.PROJECT_MANAGER)
                throw new Exception("Approver need to be Project Manager or Hr Manager");

            LeaveRequest request = leaveReuest;
            if(request.Status!= Enums.LeaveRequestStatus.New)
                throw new ArgumentException("Only New Requests can be submitted");

            request.Status = LeaveRequestStatus.Submitted;
            await _leaveRequestRepository.Update(request);

            ApprovalRequest approvalRequest = new()
            {
                ID = default,
                ApproverId = approver.ID,
                LeaveRequestId = leaveReuest.ID,
                Status = Enums.ApprovalRequestStatus.New,
                Comment = String.Empty,
            };

            approver.AprovalRequest.Add(approvalRequest);
            await _context.AddAsync(approvalRequest);
            await _context.SaveChangesAsync();
            return request;
                
        }
        public async Task<LeaveRequest> CancelRequest(int requestId)
        {
            var leaveRequest  = _leaveRequestRepository.GetById(requestId);
            if(leaveRequest == null)
            {
                throw new KeyNotFoundException("There is no LeaveRequest with this id");
            }
            LeaveRequest request = leaveRequest.Result;

            if(request.Status != Enums.LeaveRequestStatus.New)
            {
                throw new ArgumentException("Only New LeaveRequests can be Canceled");
            }
            request.Status = Enums.LeaveRequestStatus.Cancelled;
            await _leaveRequestRepository.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }



        public async Task<LeaveRequest> approve(int requestId,Boolean accepted,string comment)
        {
            var  leaveRequest = _leaveRequestRepository.GetById(requestId);
            if(leaveRequest == null )
            {
                throw new KeyNotFoundException("LeaveRequest with this id does not exist");
            }

            LeaveRequest leave1 = leaveRequest.Result;
            if (leave1.Status != Enums.LeaveRequestStatus.Submitted)
            {
                throw new ArgumentException("Only subbmited requests can be approved/rejected");
            }
            if(accepted)
            {
                leave1.Comment = comment;
                int period = leave1.EndDate.DayNumber - leave1.StartDate.DayNumber;
                var employee = _employeeService.GetEmployeeByIdAsync(leave1.EmployeeId);
                if (employee!=null)
                {
                    if (employee.Result.outOfOfficeBalance >= period)
                    {
                        employee.Result.outOfOfficeBalance -= period;
                    }
                    else
                    {
                        throw new ArgumentException("Not enough out of Office days for this leaveRequest");
                    }


                    await _context.SaveChangesAsync();
                    return leave1;
                }
            }
            else
            {
                leave1.Status = Enums.LeaveRequestStatus.Rejected;
                leave1.Comment = comment;
                await _leaveRequestRepository.Update(leave1);
                await _context.SaveChangesAsync();
                return leave1;
            }
            throw new Exception("Bad request");
        }

        public async Task<IEnumerable<LeaveRequest>> GetSortedLeaveRequestAsync(LeaveRequestSortOption sortBy, string sortOrder)
        {
            var leaveRequests = await _leaveRequestRepository.GetAll();

            Func<LeaveRequest, object> orderByFunc = sortBy switch
            {
                LeaveRequestSortOption.StartDate => e => e.StartDate,
                LeaveRequestSortOption.EndDate => e => e.EndDate,
                LeaveRequestSortOption.EmployeeId => e => e.EmployeeId,
                LeaveRequestSortOption.AbsenceReason => e => e.AbsenceReason,
                LeaveRequestSortOption.Status => e => e.Status,
                _ => e => e.ID
            };

            var sortedLeaveRequest = sortOrder.ToLower() == "desc"
                ? leaveRequests.OrderByDescending(orderByFunc) 
                : leaveRequests.OrderBy(orderByFunc);

            return leaveRequests;
        }

        public async Task<IEnumerable<LeaveRequest>> FilterEmployees(LeaveRequestFilter filter)
        {
            return await _leaveRequestRepository.FilterLeaveRequestsAsync(filter);
        }

    }

}
