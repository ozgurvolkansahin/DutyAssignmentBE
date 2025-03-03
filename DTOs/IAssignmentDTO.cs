namespace DutyAssignment.DTOs
{
    public interface IFilterAssignments
    {
        string dutyId { get; set; }
        string dutyDescription { get; set; }
        int type { get; set; }
        int page { get; set; }
        int pageSize { get; set; }
    }
    public class FilterAssignmentsByFilter
    {
        public required IEnumerable<object> data { get; set; }
        public int total { get; set; }
    }
}
