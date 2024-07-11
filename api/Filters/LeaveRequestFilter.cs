using api.Enums;

namespace api.Filters
{
    public class LeaveRequestFilter
    {
        public AbsenceReason? absenceReason { get; set; }
        public LeaveRequestStatus? leaveRequestStatus { get; set; }
    }
}
