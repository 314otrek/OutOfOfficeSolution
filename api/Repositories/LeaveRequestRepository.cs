using api.Data;
using api.Enums;
using api.Filters;
using api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequest?> GetById(int id)
        {
            return await _context.LeaveRequests.FindAsync(id);
        }

        public async Task<List<LeaveRequest>> GetByEmployeeId(int employeeId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetAll()
        {
            return await _context.LeaveRequests.ToListAsync();
        }

        public async Task Add(LeaveRequest leaveRequest)
        {
            await _context.LeaveRequests.AddAsync(leaveRequest);
        }

        public async Task Update(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);
        }

        public async Task Delete(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<LeaveRequest>> FilterLeaveRequestsAsync(LeaveRequestFilter filter)
        {
            var query = "SELECT * FROM LeaveRequests WHERE 1=1 ";
            var parameters = new List<SqliteParameter>();

            if (filter.absenceReason.HasValue)
            {
                query += " AND AbsenceReason = @AbsenceReason";
                parameters.Add(new SqliteParameter("@AbsenceReason", filter.absenceReason.Value));
            }

            if (filter.leaveRequestStatus.HasValue)
            {
                query += " AND Status = @Status";
                parameters.Add(new SqliteParameter("@Status", filter.leaveRequestStatus.Value));
            }

            return await _context.LeaveRequests.FromSqlRaw(query, parameters.ToArray()).ToListAsync();
        }
    }
}
