using api.Data;
using api.Filters;
using api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly ApplicationDbContext _context;

        public ApprovalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalRequest?> GetById(int id)
        {
            return await _context.ApprovalRequests.FindAsync(id);
        }

        public async Task<List<ApprovalRequest>> GetAll()
        {
            return await _context.ApprovalRequests.ToListAsync();
        }

        public async Task<ApprovalRequest> Add(ApprovalRequest approvalRequest)
        {
            var entry = await _context.ApprovalRequests.AddAsync(approvalRequest); 
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<ApprovalRequest> Update(ApprovalRequest approvalRequest)
        {
            var entry = await _context.ApprovalRequests.FirstOrDefaultAsync(p => p.ID == approvalRequest.ID);
            entry.ApproverId = approvalRequest.ApproverId;
            entry.Comment = approvalRequest.Comment;
            entry.Status = approvalRequest.Status;
            entry.LeaveRequestId = approvalRequest.LeaveRequestId;
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<ApprovalRequest> Delete(int id)
        {
            var approvalRequest = await _context.ApprovalRequests.FirstOrDefaultAsync(p => p.ID == id);
            var result = _context.ApprovalRequests.Remove(approvalRequest);
            return result.Entity;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApprovalRequest>> FilterApprovalRequestsAsync(ApprovalRequestFilter filter)
        {
            var query = "SELECT * FROM ApprovalRequests WHERE 1=1 ";
            var parameters = new List<SqliteParameter>();

            if (filter.Status.HasValue)
            {
                query += " AND Status = @Status";
                parameters.Add(new SqliteParameter("@Status", filter.Status.Value));
            }

            return await _context.ApprovalRequests.FromSqlRaw(query, parameters.ToArray()).ToListAsync();
        }
    }
}
