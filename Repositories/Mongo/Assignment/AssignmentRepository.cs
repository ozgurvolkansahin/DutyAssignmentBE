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
        public async Task SetAssignmentPaid(string dutyId, IEnumerable<string> sicil)
        {
            var updateTime = DateTime.Now;
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            var update = Builders<IAssignment>.Update.Set(x => x.PaidPersonal, sicil)
                .Set(x => x.AssignmentDate, updateTime)
                .Set(x => x.LastUpdate, updateTime)
                .Set(x => x.IsActive, true);
            await _collection.UpdateManyAsync(filter, update);
        }
        public async Task SetAssignmentUnPaid(string dutyId, IEnumerable<string> sicil, DateTime lastAssignmentDate)
        {
            var updateTime = DateTime.Now;
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            var update = Builders<IAssignment>.Update
                .Push("PreviousAssignments", new PreviousAssignments
                {
                    Personal = sicil,
                    Date = lastAssignmentDate
                })
                .Set(x => x.PaidPersonal, new List<string>())
                .Set(x => x.LastUpdate, updateTime)
                .Set(x => x.IsActive, false);
            await _collection.UpdateManyAsync(filter, update);
        }
        public async Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues)
        {

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
                { "AllPeople", new BsonDocument("$in", specificValues) }
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

        public async Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateAndGetByPage(int page, int pageSize)
        {
            // filter by PaidPersonal is null, then sort by AssignmentDate, then skip and take, then lookup with duty
            var pipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument
    {
        { "PaidPersonal", new BsonDocument("$eq", new BsonArray()) }
    }),
    new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "duty" },
        { "localField", "DutyId" },
        { "foreignField", "duty_id" },
        { "as", "Duty" }
    }),
    new BsonDocument("$unwind", "$Duty"),
    new BsonDocument("$sort", new BsonDocument("Duty.date", 1)),
    new BsonDocument("$skip", (page - 1) * pageSize),
    new BsonDocument("$limit", pageSize),
    new BsonDocument("$project", new BsonDocument
    {
        { "Duty", 1 },
        { "date", 1 },
        { "DutyId", 1 }
    })
            };

            return await _collection.Aggregate<AssignmentLookupDuty>(pipeline).ToListAsync();
        }
    }
}