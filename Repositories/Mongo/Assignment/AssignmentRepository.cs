using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DutyAssignment.Repositories.Mongo.Duty
{
    public class AssignmentRepository : MongoRepository<IAssignment, string>, IAssignmentRepository
    {
        private readonly IDutyRepository _dutyRepository;
        public AssignmentRepository(IDutyRepository dutyRepository) : base("assignment")
        {
            _dutyRepository = dutyRepository;
        }

        public async Task<IAssignment> GeAssignmentByDutyId(string dutyId)
        {
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<IAssignment> GetAssignmentByDutyIdAndType(string dutyId, int type)
        {
            var filter = Builders<IAssignment>.Filter.And(
                Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId),
                Builders<IAssignment>.Filter.Eq(x => x.Type, type)
            );
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task SetAssignmentPaid(string dutyId, IEnumerable<string> sicil, int type)
        {
            var updateTime = DateTime.Now;
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            filter &= Builders<IAssignment>.Filter.Eq(x => x.Type, type);
            var update = Builders<IAssignment>.Update.Set(x => x.PaidPersonal, sicil)
                .Set(x => x.AssignmentDate, updateTime)
                .Set(x => x.LastUpdate, updateTime)
                .Set(x => x.IsActive, true);
            await _collection.UpdateManyAsync(filter, update);
        }
        public async Task SetAssignmentUnPaid(string dutyId, IEnumerable<string> sicil, DateTime lastAssignmentDate, int type)
        {
            var updateTime = DateTime.Now;
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            filter &= Builders<IAssignment>.Filter.Eq(x => x.Type, type);
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
        public async Task<int> GetWaitingAssignmentsCount()
        {
            // match by PaidPersonal is null
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "PaidPersonal", new BsonDocument("$eq", new BsonArray()) }
                }),
                new BsonDocument("$count", "count")
            };
            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return result?["count"].AsInt32 ?? 0;
        }
        public async Task<int> GetWaitingAssignmentsByTypeCount(int type)
        {
            // match by PaidPersonal is null
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "PaidPersonal", new BsonDocument("$eq", new BsonArray()) },
                    { "Type", type }
                }),
                new BsonDocument("$count", "count")
            };
            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return result?["count"].AsInt32 ?? 0;
        }
        public async Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateAndGetByPage(int page, int pageSize)
        {
            // filter by PaidPersonal is null, then sort by AssignmentDate, then skip and take, then lookup with duty
            var pipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument
            {
        { "PaidPersonal", new BsonDocument("$eq", new BsonArray()) },
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "duty" },
                { "localField", "DutyId" },
                { "foreignField", "duty_id" },
                { "as", "Duty" }
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "PaidPersonalCount", new BsonDocument("$size", "$PaidPersonal") },
                { "PoliceAttendantsCount", new BsonDocument("$size", "$PoliceAttendants") },
                { "ResponsibleManagersCount", new BsonDocument("$size", "$ResponsibleManagers") }
            }),
            new BsonDocument("$unwind", "$Duty"),
            new BsonDocument("$sort", new BsonDocument("Duty.date", 1)),
            new BsonDocument("$skip", (page - 1) * pageSize),
            new BsonDocument("$limit", pageSize),
            new BsonDocument("$project", new BsonDocument
            {
                { "Duty", 1 },
                { "date", 1 },
                { "DutyId", 1 },
                // Arraylerin boyutlarını projelendiriyoruz
                { "PaidPersonalCount", 1 },
                { "PoliceAttendantsCount", 1 },
                { "ResponsibleManagersCount", 1 }
            })
            };

            return await _collection.Aggregate<AssignmentLookupDuty>(pipeline).ToListAsync();
        }
        public async Task<IEnumerable<IAssignmentLookupDuty>> SortAssignmentsByDateTypeAndGetByPage(int page, int pageSize, int type)
        {
            // filter by PaidPersonal is null, then sort by AssignmentDate, then skip and take, then lookup with duty
            var pipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument
            {
                { "PaidPersonal", new BsonDocument("$eq", new BsonArray()) },
                { "Type", type }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                // there is a field as type in duty collection
                // so make a lookup with type
                { "from", "duty" },
                { "localField", "DutyId" },
                { "foreignField", "duty_id" },
                { "as", "Duty" },
            }),
            new BsonDocument("$addFields", new BsonDocument
            {
                { "PaidPersonalCount", new BsonDocument("$size", "$PaidPersonal") },
                { "PoliceAttendantsCount", new BsonDocument("$size", "$PoliceAttendants") },
                { "ResponsibleManagersCount", new BsonDocument("$size", "$ResponsibleManagers") }
            }),
            new BsonDocument("$unwind", "$Duty"),
            new BsonDocument("$match", new BsonDocument
            {
                { "Duty.type", new BsonDocument("$eq", type) } // desiredType yerine filtrelemek istediğiniz type değerini yazın
            }),
            new BsonDocument("$sort", new BsonDocument("Duty.date", 1)),
            new BsonDocument("$skip", (page - 1) * pageSize),
            new BsonDocument("$limit", pageSize),
            new BsonDocument("$project", new BsonDocument
            {
                { "Duty", 1 },
                { "date", 1 },
                { "DutyId", 1 },
                // Arraylerin boyutlarını projelendiriyoruz
                { "PaidPersonalCount", 1 },
                { "PoliceAttendantsCount", 1 },
                { "ResponsibleManagersCount", 1 }
            })
            };

            return await _collection.Aggregate<AssignmentLookupDuty>(pipeline).ToListAsync();
        }
        public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize)
        {
            // // get assignments with PaidPersonal array not empty
            // var filter = Builders<IAssignment>.Filter.Ne("PaidPersonal", new BsonArray());
            // return await _collection.Find(filter).ToListAsync();
            // filter by PaidPersonal is null, then sort by AssignmentDate, then skip and take, then lookup with duty
            var pipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument {
                { "PaidPersonal", new BsonDocument("$ne", new BsonArray()) }
            }),
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "duty" },
                { "localField", "DutyId" },
                { "foreignField", "duty_id" },
                { "as", "Duty" }
            }),
            new BsonDocument("$unwind", "$Duty"),
            // Duty.type ile Type alanını eşleştiriyoruz
            new BsonDocument("$match", new BsonDocument
            {
            { "$expr", new BsonDocument("$eq", new BsonArray { "$Duty.type", "$Type" }) }
            }),
                new BsonDocument("$addFields", new BsonDocument
    {
        { "PaidPersonalCount", new BsonDocument("$size", "$PaidPersonal") },
        { "PoliceAttendantsCount", new BsonDocument("$size", "$PoliceAttendants") },
        { "ResponsibleManagersCount", new BsonDocument("$size", "$ResponsibleManagers") }
    }),
            new BsonDocument("$sort", new BsonDocument("AssignmentDate", -1)),
            new BsonDocument("$skip", (pageNumber - 1) * pageSize),
            new BsonDocument("$limit", pageSize),
            // create one data and one total count properties
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value }, // Id alanı burada gruplama için gerekli ancak null yapılıyor
                // create data array consisting of Duty, PaidPersonalCount, PoliceAttendantsCount, ResponsibleManagersCount
        { "data", new BsonDocument("$push", new BsonDocument
            {
                { "Duty", "$Duty" },
                { "PaidPersonalCount", "$PaidPersonalCount" },
                { "PoliceAttendantsCount", "$PoliceAttendantsCount" },
                { "ResponsibleManagersCount", "$ResponsibleManagersCount" }
            })
        }            })
            };
            var resultDocument = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var pipeLineCount = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument {
                { "PaidPersonal", new BsonDocument("$ne", new BsonArray()) }
            }),
            new BsonDocument("$count", "totalCount")
            };
            var countDocument = await _collection.Aggregate<BsonDocument>(pipeLineCount).FirstOrDefaultAsync();
            var result = resultDocument?["data"].AsBsonArray.Select(p =>
            {
                var bsonDocument = p.AsBsonDocument;
                var dutyDocument = bsonDocument["Duty"].AsBsonDocument;
                dutyDocument.Remove("_id"); // Duty içindeki _id alanını kaldır
                bsonDocument["Duty"] = dutyDocument;
                return BsonSerializer.Deserialize<object>(bsonDocument);
            }).ToList();
            // remove _id from all Duty objects


            // get TotalCount from result
            var totalCount = countDocument?["totalCount"].AsInt32;
            return new GetAssignedPersonalByDutyIdWithPaginationResult<object>

            {
                total = totalCount ?? 0,
                data = result
            };
        }


        public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize)
        {
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            var assignment = await _collection.Find(filter).FirstOrDefaultAsync();
            if (assignment == null)
            {
                return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>
                {
                    total = 0,
                    data = new List<PersonalExcel>() // Initialize the required 'data' member
                };
            }
            // create a pipeline to lookup PaidPersonal string array with personal collection with pagination
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument
                {
                    { "DutyId", dutyId }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "personal" },
                    { "localField", "PaidPersonal" },
                    { "foreignField", "Sicil" },
                    { "as", "Personal" }
                }),
                // to get total count of PersonalArray
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "TotalCount", new BsonDocument("$size", "$Personal") }
                }),
                new BsonDocument("$unwind", "$Personal"),
                // assign total count to a variable
                new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
                    new BsonDocument("$group", new BsonDocument
                {
                    { "_id", BsonNull.Value }, // Id alanı burada gruplama için gerekli ancak null yapılıyor
                    { "PersonalArray", new BsonDocument("$push", "$Personal") },
                    // get totalcount as in
                    { "TotalCount", new BsonDocument("$first", "$TotalCount") }
                    // get total count of PersonalArray
                })
                // new BsonDocument("$project", new BsonDocument
                // {
                //     { "Personal", 1 },
                // })
            };
            // get PersonalArray from result


            var resultDocument = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var result = resultDocument?["PersonalArray"].AsBsonArray.Select(p => BsonSerializer.Deserialize<PersonalExcel>(p.AsBsonDocument)).ToList();
            // get TotalCount from result
            var totalCount = resultDocument?["TotalCount"].AsInt32;
            return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>
            {
                total = totalCount ?? 0,
                data = result ?? new List<PersonalExcel>()
            }; ;
        }

        public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdAndTypeWithPagination(string dutyId, int page, int pageSize, int type, bool isAll)
        {
            var filter = Builders<IAssignment>.Filter.And(
        Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId),
        Builders<IAssignment>.Filter.Eq(x => x.Type, type)
    );
            var assignment = await _collection.Find(filter).FirstOrDefaultAsync();
            if (assignment == null)
            {
                return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>
                {
                    total = 0,
                    data = new List<PersonalExcel>() // Initialize the required 'data' member
                };
            }

            // if type is isAll, merge ResponsibleManagers and PoliceAttendants else use PaidPersonal
            var localField = isAll ? "AllPersonnel" : "PaidPersonal";

            // create lookup pipeline
            var pipeline = new List<BsonDocument>
    {
        new BsonDocument("$match", new BsonDocument
        {
            { "DutyId", dutyId },
            { "Type", type }
        })
    };

            if (isAll)
            {
                pipeline.Add(new BsonDocument("$addFields", new BsonDocument
        {
            { "AllPersonnel", new BsonDocument("$concatArrays", new BsonArray { "$ResponsibleManagers", "$PoliceAttendants" }) }
        }));
            }

            pipeline.Add(new BsonDocument("$lookup", new BsonDocument
    {
        { "from", "personal" },
        { "localField", localField },
        { "foreignField", "Sicil" },
        { "as", "Personal" }
    }));

            pipeline.Add(new BsonDocument("$addFields", new BsonDocument
    {
        { "TotalCount", new BsonDocument("$size", "$Personal") }
    }));

            pipeline.Add(new BsonDocument("$unwind", "$Personal"));
            pipeline.Add(new BsonDocument("$skip", (page - 1) * pageSize));
            pipeline.Add(new BsonDocument("$limit", pageSize));
            pipeline.Add(new BsonDocument("$group", new BsonDocument
    {
        { "_id", BsonNull.Value },
        { "PersonalArray", new BsonDocument("$push", "$Personal") },
        { "TotalCount", new BsonDocument("$first", "$TotalCount") }
    }));

            var resultDocument = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var result = resultDocument?["PersonalArray"].AsBsonArray.Select(p => BsonSerializer.Deserialize<PersonalExcel>(p.AsBsonDocument)).ToList();
            var totalCount = resultDocument?["TotalCount"].AsInt32;

            return new GetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>
            {
                total = totalCount ?? 0,
                data = result ?? new List<PersonalExcel>()
            };
        }
        public async Task<FilterAssignmentsByFilter> FilterAssignments(FilterAssignments filterAssignments)
        {
            var normalizedDes = filterAssignments.dutyDescription.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            var matchStage = new BsonDocument("$match", new BsonDocument());

            if (filterAssignments.type != 0)
            {
                matchStage = new BsonDocument("$match", new BsonDocument
                {
                    { "Type", new BsonDocument("$eq", filterAssignments.type) }
                });
            }
            var pipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument {
                { "PaidPersonal", new BsonDocument("$ne", new BsonArray()) }
            }),
            // use filterAssignments to filter also
            new BsonDocument("$match", new BsonDocument
            {
                { "DutyId", new BsonDocument("$regex", filterAssignments.dutyId) },
            }),
            matchStage,
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "duty" },
                { "localField", "DutyId" },
                { "foreignField", "duty_id" },
                { "as", "Duty" }
            }),
            // use filtersAssignments.dutyDescription to filter also
            new BsonDocument("$match", new BsonDocument
            {
                { "Duty.duty_description", new BsonDocument
                    {
                        { "$regex", normalizedDes },
                        { "$options", "i" }  // Case-insensitive arama için "i" kullanılıyor
                    }
                }
            }),
            // new BsonDocument("$addFields", new BsonDocument
            //     {
            //         { "TotalCount", new BsonDocument("$size", "$Duty") }
            //     }),
            new BsonDocument("$unwind", "$Duty"),
                        new BsonDocument("$match", new BsonDocument
            {
            { "$expr", new BsonDocument("$eq", new BsonArray { "$Duty.type", "$Type" }) }
            }),
                new BsonDocument("$addFields", new BsonDocument
    {
        { "PaidPersonalCount", new BsonDocument("$size", "$PaidPersonal") },
        { "PoliceAttendantsCount", new BsonDocument("$size", "$PoliceAttendants") },
        { "ResponsibleManagersCount", new BsonDocument("$size", "$ResponsibleManagers") },
    }),
            new BsonDocument("$sort", new BsonDocument("Duty.date", 1)),
            new BsonDocument("$skip", (filterAssignments.page - 1) * filterAssignments.pageSize),
            new BsonDocument("$limit", filterAssignments.pageSize),
            // create one data and one total count properties
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", BsonNull.Value }, // Id alanı burada gruplama için gerekli ancak null yapılıyor
                // { "total", new BsonDocument("$first", "$TotalCount") },
                // create data array consisting of Duty, PaidPersonalCount, PoliceAttendantsCount, ResponsibleManagersCount
        { "data", new BsonDocument("$push", new BsonDocument
            {
                { "Duty", "$Duty" },
                { "PaidPersonalCount", "$PaidPersonalCount" },
                { "PoliceAttendantsCount", "$PoliceAttendantsCount" },
                { "ResponsibleManagersCount", "$ResponsibleManagersCount" }
            })
        }            })
            };

            // Toplam belge sayısını ayrı bir sorgu ile alın
            var totalCountPipeline = new BsonDocument[]
            {
                    new BsonDocument("$match", new BsonDocument {
                { "PaidPersonal", new BsonDocument("$ne", new BsonArray()) }
            }),
            // use filterAssignments to filter also
            new BsonDocument("$match", new BsonDocument
            {
                { "DutyId", new BsonDocument("$regex", filterAssignments.dutyId) },
            }),
            matchStage,
            new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "duty" },
                { "localField", "DutyId" },
                { "foreignField", "duty_id" },
                { "as", "Duty" }
            }),
            // use filtersAssignments.dutyDescription to filter also
            new BsonDocument("$match", new BsonDocument
            {
                { "Duty.duty_description", new BsonDocument
                    {
                        { "$regex", normalizedDes },
                        { "$options", "i" }  // Case-insensitive arama için "i" kullanılıyor
                    }
                }
            }),
                new BsonDocument("$count", "TotalCount")
            };

            var resultDocument = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            var result = resultDocument?["data"].AsBsonArray.Select(p =>
            {
                var bsonDocument = p.AsBsonDocument;
                var dutyDocument = bsonDocument["Duty"].AsBsonDocument;
                dutyDocument.Remove("_id"); // Duty içindeki _id alanını kaldır
                bsonDocument["Duty"] = dutyDocument;
                return BsonSerializer.Deserialize<object>(bsonDocument);
            }).ToList();

            var totalCountResult = await _collection.Aggregate<BsonDocument>(totalCountPipeline).FirstOrDefaultAsync();
            var totalCount = totalCountResult != null ? totalCountResult["TotalCount"].AsInt32 : 0;

            // var totalCount = resultDocument?["total"].AsInt32;
            return new FilterAssignmentsByFilter
            {
                data = result,
                total = totalCount
            };
        }
        public async Task<UpdateResult> ResetAssignment(string dutyId, int type)
        {
            var filter = Builders<IAssignment>.Filter.And(
                Builders<IAssignment>.Filter.Eq(a => a.DutyId, dutyId),
                Builders<IAssignment>.Filter.Eq(a => a.Type, type),
                Builders<IAssignment>.Filter.Exists(a => a.PaidPersonal, true),
                Builders<IAssignment>.Filter.Type(a => a.PaidPersonal, BsonType.Array)
            );

            var update = Builders<IAssignment>.Update
                .Set(a => a.PaidPersonal, new List<string>())
                .Set(a => a.IsActive, false);

            return await _collection.UpdateManyAsync(filter, update);

        }
        public async Task<DeleteResult> DeleteAssignment(string dutyId)
        {
            var filter = Builders<IAssignment>.Filter.Eq(x => x.DutyId, dutyId);
            return await _collection.DeleteOneAsync(filter);
        }
    }
}
