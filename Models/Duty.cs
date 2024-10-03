using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Models;

public class Duty: Entity
{
    [BsonElement("duty_id")]
    public string DutyId { get; set; } = null!;

    [BsonElement("duty_description")]
    public string Description { get; set; } = null!;

    [BsonElement("date")]
    public DateTime Date { get; set; }
}