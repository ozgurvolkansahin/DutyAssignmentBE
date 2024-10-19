using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public class PersonalRepository : MongoRepository<IPersonalExcel, string>, IPersonalRepository
    {
        public PersonalRepository() : base("personal")
        {

        }

        public async Task InsertPersonalDataAsync(IEnumerable<IPersonalExcel> entities)
        {
            await InsertManyAsync(entities);
        }
        public async Task<IEnumerable<IPersonalExcel>> GetPersonalById(IEnumerable<string> sicil)
        {
            var filter = Builders<IPersonalExcel>.Filter.In(x => x.Sicil, sicil);
            return await _collection.Find(filter).ToListAsync();
        }
        public async Task PushDutyIdToDutyArray(string dutyId, List<string> sicil)
        {
            var filter = Builders<IPersonalExcel>.Filter.In(x => x.Sicil, sicil);
            var update = Builders<IPersonalExcel>.Update.AddToSet(x => x.Duties, dutyId);
            await UpdateManyAsync(filter, update);
        }
        public async Task PushDutyIdToPaidDutyArray(string dutyId, List<string> sicil)
        {
            var filter = Builders<IPersonalExcel>.Filter.In(x => x.Sicil, sicil);
            var update = Builders<IPersonalExcel>.Update.AddToSet(x => x.PaidDuties, dutyId);
            await UpdateManyAsync(filter, update);
        }
        public async Task RemoveDutyIdFromPaidDutyArray(string dutyId)
        {
            // find all documents that have the dutyId in their PaidDuties array
            var filter = Builders<IPersonalExcel>.Filter.AnyEq(x => x.PaidDuties, dutyId);
            // remove the dutyId from the PaidDuties array
            var update = Builders<IPersonalExcel>.Update.Pull(x => x.PaidDuties, dutyId);
            await UpdateManyAsync(filter, update);
        }
        public async Task<int> GetTotalPaymentsAsync()
        {
            // get count of all documents that have a non-empty PaidDuties array
            var filter = Builders<IPersonalExcel>.Filter.SizeGt(x => x.PaidDuties, 0);
            return Convert.ToInt32(await _collection.CountDocumentsAsync(filter));
        }

                public async Task<int> GetTotalAssignedPersonal()
        {
            // get total count of PoliceAttendants and ResponsibleManagers
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "dutiesCount", new BsonDocument("$size", new BsonArray { "$Duties" }) }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "null" },
                    { "totalDutiesCount", new BsonDocument("$sum", "$dutiesCount") }
                })            };
            // return count property from result
            return (await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync())["totalDutiesCount"].AsInt32;
            
        }
    }
}