using System.Text.Json.Serialization;
using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
    public interface IDashboard : IEntity<string>
    {
        SystemAdmins SystemAdmins { get; set; }
        [JsonIgnore]
        [BsonElement("name")]
        string Name { get; set; }
    }

    public class SystemAdmins
    {
        [BsonElement("BranchManager")]
        public required string BranchManager { get; set; }
        [BsonElement("Responsible")]
        public required string Responsible { get; set; }
        [BsonElement("ResponsibleManager")]
        public required string ResponsibleManager { get; set; }
    }

}
