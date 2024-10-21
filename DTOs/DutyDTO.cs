namespace DutyAssignment.DTOs
{
    public class GetDutiesByIdList: IGetDutiesByIdList
    {
        public required string sicil { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public bool isPaidDuties { get; set; }
    }
}