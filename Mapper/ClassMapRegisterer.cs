using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson.Serialization;

namespace DutyAssignment.Mapper
{
    public class ClassMapRegisterer
    {
        static ClassMapRegisterer()
        {
            BsonClassMap.RegisterClassMap<Duty>();
        }

        public static void RegisterClassMaps()
        {            
        }
    }
}