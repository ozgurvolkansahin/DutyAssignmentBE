using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public class SelectPersonalToBePaid: ISelectPersonalToBePaid
    {
        public required IEnumerable<string> dutyIds { get; set; }
        public int assignmentCount { get; set; }
        public bool reAssign { get; set; }
    }
    public class GetAssignedPersonalByDutyIdWithPaginationPostObject: IGetAssignedPersonalByDutyIdWithPaginationPostObject
    {
        public required string dutyId { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
    public class GetAssignedPersonalByDutyIdWithPaginationResult<T>: IGetAssignedPersonalByDutyIdWithPaginationResult<T>
    {
        public required IEnumerable<T> data { get; set; }
        public int total { get; set; }
    }
}
