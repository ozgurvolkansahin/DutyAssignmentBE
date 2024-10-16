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
}
