using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
    public interface IDuty : IEntity<string>
    {
        string DutyId { get; set; }
        string Description { get; set; }
        int Type { get; set; }
        DateTime Date { get; set; }
    }
}