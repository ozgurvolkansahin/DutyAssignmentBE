using DutyAssignment.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutyAssignment.Models;

public abstract class Entity<T>: IEntity<T>
{
        [BsonId]
        public abstract T Id { get; set; }
}