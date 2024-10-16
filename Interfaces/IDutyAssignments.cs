using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
public interface IDutyAssignments
{
    string DutyId { get; set; }
    IEnumerable<IPersonalExcel> Personal { get; set; }
    DateTime Date { get; set; }
}
}
