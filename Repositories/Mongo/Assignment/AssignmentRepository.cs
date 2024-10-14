using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public class AssignmentRepository : MongoRepository<IAssignment, string>, IAssignmentRepository
    {
        public AssignmentRepository() : base("assignment")
        {

        }

        public async Task<IAssignment> GeAssignmentByDutyId(string dutyId)
        {
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(string[] specificValues)
        {
            var spexxxcificValues = new BsonArray {"431039", "488965", "270347", "321498"};

            var pipeline = new BsonDocument[]
{
            // ResponsibleManagers ve PoliceAttendants dizilerini birleştiriyoruz
            new BsonDocument("$project", new BsonDocument
            {
                { "AllPeople", new BsonDocument("$concatArrays", new BsonArray { "$ResponsibleManagers", "$PoliceAttendants" }) }
            }),

            // AllPeople dizisini açıyoruz
            new BsonDocument("$unwind", "$AllPeople"),

            // Belirtilen ID'lerle eşleşenleri filtreliyoruz
            new BsonDocument("$match", new BsonDocument
            {
                { "AllPeople", new BsonDocument("$in", spexxxcificValues) }
            }),
            // Aynı ID'yi gruplayıp sayısını alıyoruz
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$AllPeople" },  // Tekrarlayan ID
                { "count", new BsonDocument("$sum", 1) }  // Sayısını topluyoruz
            }),

            // Sayıya göre sıralama
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };
            return await _collection.Aggregate<PeopleCount>(pipeline).ToListAsync();
            // total assignment result listesinde
            // zorunlu atanacakları ekle ve en azdan olacak şekilde istenilen miktarda kişiyi göreve ata
        }
    }
}