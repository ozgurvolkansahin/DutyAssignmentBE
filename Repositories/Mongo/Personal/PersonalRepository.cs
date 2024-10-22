using DutyAssignment.DTOs;
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

        // get all personal with pagination
        public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetPersonalWithPagination(int page, int pageSize)
        {
            // find, skip, and limit documents; then empty Duties and PaidDuties array and set their count to DutiesCount and PaidDutiesCount before emptying them
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument
                {
                    { "SN", 1 },
                    { "Sicil", 1 },
                    { "TcKimlik", 1 },
                    { "Ad", 1 },
                    { "Soyad", 1 },
                    { "Rutbe", 1 },
                    { "Birim", 1 },
                    { "Nokta", 1 },
                    { "Grup", 1 },
                    { "Tel", 1 },
                    { "Iban", 1 },
                    // { "Duties", 1 },
                    // { "PaidDuties", 1 },
                    { "DutiesCount", new BsonDocument("$size", "$Duties") },
                    { "PaidDutiesCount", new BsonDocument("$size", "$PaidDuties") }
                }),
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize)
            };
            // return result
            return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>
            {
                data = await _collection.Aggregate<IPersonalExcel>(pipeline).ToListAsync(),
                total = Convert.ToInt32(await _collection.CountDocumentsAsync(new BsonDocument()))
            };

            // var personnel = await _collection.Find(new BsonDocument()).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
            // var count = Convert.ToInt32(await _collection.CountDocumentsAsync(new BsonDocument()));
            // return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel> { data = personnel, total = count };
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
        public async Task<int> GetTotalPaymentsDone()
        {
            // get total count of PoliceAttendants and ResponsibleManagers
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "null" },
                    { "totalDutiesCount", new BsonDocument("$sum", new BsonDocument("$size", "$PaidDuties")) }
                })};
            // return count property from result
            return (await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync())["totalDutiesCount"].AsInt32;

        }

        public async Task<FilterPersonnelWithTotalCount> FilterPersonnel(FilterPersonnel filter)
        {
            var filterBuilder = Builders<IPersonalExcel>.Filter;
            var filters = new List<FilterDefinition<IPersonalExcel>>();

            if (!string.IsNullOrEmpty(filter.sicil))
                filters.Add(filterBuilder.Regex(x => x.Sicil, new BsonRegularExpression(filter.sicil, "i")));

            if (!string.IsNullOrEmpty(filter.tcKimlik))
                filters.Add(filterBuilder.Regex(x => x.TcKimlik, new BsonRegularExpression(filter.tcKimlik, "i")));

            if (!string.IsNullOrEmpty(filter.rutbe))
                filters.Add(filterBuilder.Regex(x => x.Rutbe, new BsonRegularExpression(filter.rutbe, "i")));

            if (!string.IsNullOrEmpty(filter.birim))
                filters.Add(filterBuilder.Regex(x => x.Birim, new BsonRegularExpression(filter.birim, "i")));

            if (!string.IsNullOrEmpty(filter.nokta))
                filters.Add(filterBuilder.Regex(x => x.Nokta, new BsonRegularExpression(filter.nokta, "i")));

            if (!string.IsNullOrEmpty(filter.grup))
                filters.Add(filterBuilder.Regex(x => x.Grup, new BsonRegularExpression(filter.grup, "i")));

            if (!string.IsNullOrEmpty(filter.tel))
                filters.Add(filterBuilder.Regex(x => x.Tel, new BsonRegularExpression(filter.tel, "i")));

            if (!string.IsNullOrEmpty(filter.iban))
                filters.Add(filterBuilder.Regex(x => x.Iban, new BsonRegularExpression(filter.iban, "i")));

            if (!string.IsNullOrEmpty(filter.isim))
            {
                var normalizedAd = filter.isim.ToUpper(new System.Globalization.CultureInfo("tr-TR")); // Türkçe karakterler
                var adFilter = filterBuilder.Regex(x => x.Ad, new BsonRegularExpression(normalizedAd, "i"));
                var soyadFilter = filterBuilder.Regex(x => x.Soyad, new BsonRegularExpression(normalizedAd, "i"));
                filters.Add(filterBuilder.Or(adFilter, soyadFilter));

            }
            // Diğer filtreler burada...

            var finalFilter = filterBuilder.And(filters);
            var filteredPersonnel = await _collection.Find(finalFilter).Skip((filter.page - 1) * filter.pageSize).Limit(filter.pageSize).ToListAsync();
            var total = Convert.ToInt32(await _collection.CountDocumentsAsync(finalFilter));
            return new FilterPersonnelWithTotalCount { data = filteredPersonnel, total = total };
        }
        public async Task<UpdateResult> ResetAssignment(string dutyId)
        {
            var filter = Builders<IPersonalExcel>.Filter.And(
                Builders<IPersonalExcel>.Filter.Exists(p => p.PaidDuties, true),
                Builders<IPersonalExcel>.Filter.Type(p => p.PaidDuties, BsonType.Array),
                // find records that have the dutyId in their PaidDuties array
                Builders<IPersonalExcel>.Filter.AnyEq(p => p.PaidDuties, dutyId)
            );

            var update = Builders<IPersonalExcel>.Update.Set("PaidDuties", new BsonArray());

            return await _collection.UpdateManyAsync(filter, update);
        }
    }

}