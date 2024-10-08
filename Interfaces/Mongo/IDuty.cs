using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
public interface IDuty: IEntity<string>
{
    [BsonElement("duty_id")]
    string DutyId { get; set; }

    [BsonElement("duty_description")]
    string Description { get; set; }

    [BsonElement("date")]
    DateTime Date { get; set; }
}
}
