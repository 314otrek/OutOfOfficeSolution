using api.Filters;
using api.Models;

namespace api.Repositories
{
    public interface IApprovalRepository
    {
        Task<ApprovalRequest?> GetById(int id);
        Task<List<ApprovalRequest>> GetAll();
        Task<ApprovalRequest> Add(ApprovalRequest approvalRequest);
        Task<ApprovalRequest> Update(ApprovalRequest approvalRequest);
        Task<ApprovalRequest> Delete(int id);

        Task SaveChangesAsync();

        Task<IEnumerable<ApprovalRequest>> FilterApprovalRequestsAsync(ApprovalRequestFilter filter);

    }
}
