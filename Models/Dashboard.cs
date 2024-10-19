using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace DutyAssignment.Models;

public class Dashboard : IDashboard
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    public required SystemAdmins SystemAdmins { get; set; }
}

