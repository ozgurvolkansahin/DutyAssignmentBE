using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Models
{
    public partial class Duty : IDuty
    {
        [BsonElement("duty_id")]
        public required string DutyId { get; set; }
        [BsonElement("duty_description")]
        public required string Description { get; set; }
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }
    }
    public class PeopleCount
    {
        [BsonElement("_id")]

        public string Id { get; set; }
        [BsonElement("count")]

        public int Count { get; set; }
    }
}