namespace DutyAssignment.DTOs
{
    public class FilterAssignments: IFilterAssignments
    {
        public required string dutyId { get; set; }
        public required string dutyDescription { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }

    }
}