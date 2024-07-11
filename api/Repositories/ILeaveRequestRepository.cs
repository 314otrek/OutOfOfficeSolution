using api.Filters;
using api.Models;

namespace api.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<LeaveRequest?> GetById(int id);
        Task<List<LeaveRequest>> GetByEmployeeId(int employeeId);
        Task<List<LeaveRequest>> GetAll();
        Task Add(LeaveRequest leaveRequest);
        Task Update(LeaveRequest leaveRequest);
        Task Delete(int id);
        Task SaveChangesAsync();
        Task<IEnumerable<LeaveRequest>> FilterLeaveRequestsAsync(LeaveRequestFilter filter);
    }
}
