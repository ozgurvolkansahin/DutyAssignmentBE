using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.DTOs
{
    public interface ISelectPersonalToBePaid
    {
        IEnumerable<string> dutyIds { get; set; }
        int assignmentCount { get; set; }
    }
}
