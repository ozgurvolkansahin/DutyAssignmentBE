using DutyAssignment.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DutyAssignment.Services;

public class DutyService: IDutyService
{
    private readonly IMongoCollection<Duty> _dutyCollection;

    public DutyService(
        IOptions<DutyAssignmentDatabaseSettings> dutyAssignmentDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            dutyAssignmentDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            dutyAssignmentDatabaseSettings.Value.DatabaseName);

        _dutyCollection = mongoDatabase.GetCollection<Duty>(
            dutyAssignmentDatabaseSettings.Value.DutyCollectionName);
    }

    public async Task<List<Duty>> GetAsync() =>
        await _dutyCollection.Find(_ => true).ToListAsync();

    public async Task<Duty> GetAsync(string id) =>
        await _dutyCollection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Duty newBook) =>
        await _dutyCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Duty updatedBook) =>
        await _dutyCollection.ReplaceOneAsync(x => x.Id.ToString()  == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _dutyCollection.DeleteOneAsync(x => x.Id.ToString() == id);

    Task<Duty> IDutyService.CreateAsync(Duty duty)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Duty dutyIn)
    {
        throw new NotImplementedException();
    }
}