using DutyAssignment.Interfaces;
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
    }
}