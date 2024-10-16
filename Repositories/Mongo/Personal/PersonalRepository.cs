using DutyAssignment.Interfaces;
using DutyAssignment.Models;
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
    }
}