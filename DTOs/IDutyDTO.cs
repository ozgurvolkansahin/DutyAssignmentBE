
namespace DutyAssignment.DTOs
{
    public interface IGetDutiesByIdList
    {
        string sicil { get; set; }
        int page { get; set; }
        int pageSize { get; set; }
        bool isPaidDuties { get; set; }
    }
}