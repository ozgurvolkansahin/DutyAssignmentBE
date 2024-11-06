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
        public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetPersonalWithPagination(int page, int pageSize, int type)
        {
            // find, skip, and limit documents; then empty Duties and PaidDuties array and set their count to DutiesCount and PaidDutiesCount before emptying them
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("Type", type)),
                new BsonDocument("$project", new BsonDocument
                {
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
                total = Convert.ToInt32(await _collection.CountDocumentsAsync(new BsonDocument("Type", type)))
            };

            // var personnel = await _collection.Find(new BsonDocument()).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();
            // var count = Convert.ToInt32(await _collection.CountDocumentsAsync(new BsonDocument()));
            // return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel> { data = personnel, total = count };
        }

        public async Task InsertPersonalDataAsync(IEnumerable<IPersonalExcel> entities, int type = 1)
        {
            foreach (var entity in entities)
            {
                entity.Type = type;
            }
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
                })};
            // return count property from result
            var res = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return res != null && res.Contains("totalDutiesCount") ? res["totalDutiesCount"].AsInt32 : 0;

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
            var res = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return res != null && res.Contains("totalDutiesCount") ? res["totalDutiesCount"].AsInt32 : 0;

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
            
            if (filter.type != 0)
                filters.Add(filterBuilder.Eq(x => x.Type, filter.type));

            if (!string.IsNullOrEmpty(filter.isim))
            {
                var normalizedAd = filter.isim.ToUpper(new System.Globalization.CultureInfo("tr-TR")); // Türkçe karakterler
                var adFilter = filterBuilder.Regex(x => x.Ad, new BsonRegularExpression(normalizedAd, "i"));
                var soyadFilter = filterBuilder.Regex(x => x.Soyad, new BsonRegularExpression(normalizedAd, "i"));
                filters.Add(filterBuilder.Or(adFilter, soyadFilter));

            }
            var finalFilter = filterBuilder.And(filters);
            var renderedFilter = finalFilter.Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry);

            var aggregationStages = new List<BsonDocument>
                {
                    new BsonDocument { { "$match", renderedFilter } },
                    new BsonDocument
                    {
                        { "$addFields", new BsonDocument
                            {
                                { "DutiesCount", new BsonDocument("$size", "$Duties") },
                                { "PaidDutiesCount", new BsonDocument("$size", "$PaidDuties") }
                            }
                        }
                    }
                };
            SortDefinition<IPersonalExcel> sort = null;
            var orderBy = filter.orderBy == "dutiesCount" ? "DutiesCount" :  "PaidDutiesCount";
            // DutiesCount and PaidDutiesCount are set by Duties and PaidDuties arrays count respectively
            // so we need to create these properties before sorting

            if (filter.order != "" && filter.orderBy != "")
            {
                sort = filter.order == "asc" ? Builders<IPersonalExcel>.Sort.Ascending(filter.orderBy) : Builders<IPersonalExcel>.Sort.Descending(filter.orderBy);
                var sortOrder = filter.order == "asc" ? 1 : -1;
                aggregationStages.Add(
                    new BsonDocument
                    {
                        { "$sort", new BsonDocument(orderBy, sortOrder) }
                    }
                );
            }
            aggregationStages.Add(new BsonDocument { { "$skip", (filter.page - 1) * filter.pageSize } });
            aggregationStages.Add(new BsonDocument { { "$limit", filter.pageSize } });
            var result = await _collection.Aggregate<IPersonalExcel>(aggregationStages).ToListAsync();
            // var res2 = await _collection.Find<IPersonalExcel>(finalFilter).ToListAsync();
            // var filteredPersonnel = await _collection.Find(finalFilter).Skip((filter.page - 1) * filter.pageSize).Limit(filter.pageSize).ToListAsync();
            // // set dutiesCount and paidDutiesCount properties for each filtered personnel
            // foreach (var personnel in filteredPersonnel)
            // {
            //     personnel.DutiesCount = personnel.Duties.Count();
            //     personnel.PaidDutiesCount = personnel.PaidDuties.Count();
            // }
            // var total = Convert.ToInt32(await _collection.CountDocumentsAsync(finalFilter));
            var total = await _collection.CountDocumentsAsync(finalFilter);

            return new FilterPersonnelWithTotalCount { data = result, total = (int)total };
        }
        public async Task<UpdateResult> ResetAssignment(string dutyId)
        {
            var filter = Builders<IPersonalExcel>.Filter.And(
                Builders<IPersonalExcel>.Filter.Exists(p => p.PaidDuties, true),
                Builders<IPersonalExcel>.Filter.Type(p => p.PaidDuties, BsonType.Array),
                // find records that have the dutyId in their PaidDuties array
                Builders<IPersonalExcel>.Filter.AnyEq(p => p.PaidDuties, dutyId)
            );

            var update = Builders<IPersonalExcel>.Update.Pull(p => p.PaidDuties, dutyId);
            return await _collection.UpdateManyAsync(filter, update);
        }
        public async Task<DeleteResult> DeleteManyPersonnel(List<string> sicil)
        {
            var filter = Builders<IPersonalExcel>.Filter.In(x => x.Sicil, sicil);
            return await _collection.DeleteManyAsync(filter);
        }
    }

}