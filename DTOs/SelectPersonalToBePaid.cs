using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public class SelectPersonalToBePaid : ISelectPersonalToBePaid
    {
        public required IEnumerable<string> dutyIds { get; set; }
        public int assignmentCount { get; set; }
        public int type { get; set; }
        public bool reAssign { get; set; }
    }
    public class GetAssignedPersonalByDutyIdWithPaginationPostObject : IGetAssignedPersonalByDutyIdWithPaginationPostObject
    {
        public required string dutyId { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int type { get; set; }
    }
    public class GetAssignedPersonalByDutyIdWithPaginationResult<T> : IGetAssignedPersonalByDutyIdWithPaginationResult<T>
    {
        public required IEnumerable<T> data { get; set; }
        public int total { get; set; }
    }
    public class FilterPersonnel : IFilterPersonnel
    {
        public required string sicil { get; set; }
        public required string tcKimlik { get; set; }
        public required string isim { get; set; }
        public required string rutbe { get; set; }
        public required string birim { get; set; }
        public required string nokta { get; set; }
        public required string grup { get; set; }
        public required string tel { get; set; }
        public required string iban { get; set; }
        public required string order { get; set; }
        public required string orderBy { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int type { get; set; }

    }

    public class FilterPersonnelWithTotalCount
    {
        public required IEnumerable<IPersonalExcel> data { get; set; }
        public int total { get; set; }
    }
}
