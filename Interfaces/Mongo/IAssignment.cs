using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
    public interface IAssignment : IEntity<string>
    {
        string DutyId { get; set; }
        IEnumerable<string> ResponsibleManagers { get; set; }
        IEnumerable<string> PoliceAttendants { get; set; }
    }
}
