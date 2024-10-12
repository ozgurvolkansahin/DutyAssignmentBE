using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Interfaces
{
    public interface IEntity<T>
    {
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        T Id { get; set; }
    }
    public abstract class Entity<T> : IEntity<T>
    {
        public abstract T Id { get; set; }
    }
}
