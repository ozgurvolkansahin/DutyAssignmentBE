using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Services;

public class DutyService : IDutyService
{
    private readonly IDutyRepository _dutyRepository;
    private readonly IPersonalRepository _personalRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IExcelService _excelService;

    public DutyService(IDutyRepository dutyRepository, IExcelService excelService, IPersonalRepository personalRepository, IAssignmentRepository assignmentRepository)
    {
        _dutyRepository = dutyRepository;
        _excelService = excelService;
        _personalRepository = personalRepository;
        _assignmentRepository = assignmentRepository;
    }
    public async Task<IEnumerable<IDuty>> GetDuties()
    {
        return await _dutyRepository.GetDutiesAsync();
    }
    public async Task<object> InsertDuties()
    {
        var files = await ProcessDutyExcelFilesAsync();
        var records = await _dutyRepository.GetDutiesById(files.Select(x => x.DutyId).ToList());
        // we only want to insert records that do not exist in the database
        files = files.Where(x => !records.Any(y => y.DutyId == x.DutyId)).ToList();
        if (files.Count() != 0)
        {
            await _dutyRepository.InsertManyAsync(files);
        }
        return new
        {
            added = files.Select(x => x.DutyId).ToList(),
            ignored = records.Select(x => x.DutyId).ToList()
        };
    }

    public async Task<IEnumerable<IDuty>> ProcessDutyExcelFilesAsync()
    {
        List<string> files = _excelService.FindFiles();
        return await PrepareProcessedDutiesListAsync(files);
    }
    private async Task<IEnumerable<IDuty>> PrepareProcessedDutiesListAsync(List<string> files)
    {
        List<IDuty> duties = new List<IDuty>();
        foreach (string file in files)
        {
            PersonalInDuty personalInDuty = _excelService.ReadDutyFile(file + ".xlsx");
            await InsertPersonal(personalInDuty.ResponsibleManagers);
            await InsertPersonal(personalInDuty.PoliceAttendants);
            _ = SaveAssignment(file.Split("-")[0].Trim(), personalInDuty);
            _ = _personalRepository.PushDutyIdToDutyArray(file.Split("-")[0].Trim(), personalInDuty.ResponsibleManagers.Select(x => x.Sicil).ToList());
            _ = _personalRepository.PushDutyIdToDutyArray(file.Split("-")[0].Trim(), personalInDuty.PoliceAttendants.Select(x => x.Sicil).ToList());
            duties.Add(CreateProcessedDutyObject(file));
        }
        return duties;
    }
    private IDuty CreateProcessedDutyObject(string file)
    {
        // we expect the file name to be in the format "dutyId-description-date"
        // where description and date has a format with underscores
        List<string> fileInfo = file.Split("-").ToList();
        // date format is day_month_year_hour_minute
        List<string> dateInfo = fileInfo[2].Split("_").ToList();
        return new Duty
        {
            Id = ObjectId.GenerateNewId().ToString(),
            DutyId = fileInfo[0].Trim(),
            Description = fileInfo[1].Trim().Replace("_", "-"),
            // int year, int month, int day, int hour, int minute, int second
            Date = new DateTime(int.Parse(dateInfo[2]), int.Parse(dateInfo[1]), int.Parse(dateInfo[0]), 0, 0, 0)
        };
    }
    private async Task InsertPersonal(IEnumerable<IPersonalExcel> personalExcel)
    {
        var records = await _personalRepository.GetPersonalById(personalExcel.Select(x => x.Sicil).ToList());
        // we only want to insert records that do not exist in the database
        var notExistedPersonal = personalExcel.Where(x => !records.Any(y => y.Sicil == x.Sicil)).ToList();
        if (notExistedPersonal.Count() != 0)
        {
            await _personalRepository.InsertManyAsync(notExistedPersonal);
        }
    }
    private async Task SaveAssignment(string dutyId, PersonalInDuty personalInDuty)
    {
        var record = await _assignmentRepository.GeAssignmentByDutyId(dutyId);
        // we only want to insert records that do not exist in the database
        if (record != null) return;
        DateTime dateTime = DateTime.Now;
        _ = _assignmentRepository.CreateAsync(new Assignment
        {
            DutyId = dutyId,
            ResponsibleManagers = personalInDuty.ResponsibleManagers.Select(x => x.Sicil).ToList(),
            PoliceAttendants = personalInDuty.PoliceAttendants.Where(x => !personalInDuty.ResponsibleManagers.Select(y => y.Sicil).Contains(x.Sicil)).Select(x => x.Sicil).ToList(),
            Id = ObjectId.GenerateNewId().ToString(),
            PreviousAssignments = new List<PreviousAssignments>(),
            LastUpdate = dateTime,
            AssignmentDate = dateTime,
            IsActive = false,
            PaidPersonal = new List<string>()
        });
    }
    public async Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues)
    {
        var result = await _assignmentRepository.GetOccurrencesOfSpecificValues(specificValues);
        return result;
    }
}