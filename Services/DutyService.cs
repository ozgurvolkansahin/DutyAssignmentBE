using DutyAssignment.Enum;
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
    public async Task<IEnumerable<IDuty>> GetDutiesByIdListWithPagination(string sicil, int page, int pageSize, bool isPaidDuties)
    {
        var personnel = await _personalRepository.GetPersonalById(new List<string> { sicil });
        var personnelList = personnel?.ToList();
        if (personnelList == null || personnelList.Count == 0 || personnelList[0].Duties == null)
        {
            return Enumerable.Empty<IDuty>();
        }
        if (isPaidDuties)
        {
            return await _dutyRepository.GetDutiesByIdWithPagination(personnelList[0].PaidDuties, page, pageSize);
        }
        return await _dutyRepository.GetDutiesByIdWithPagination(personnelList[0].Duties, page, pageSize);
    }
    public async Task<IEnumerable<IDuty>> GetDutiesByIdListAndTypeWithPagination(string sicil, int page, int pageSize, bool isPaidDuties, int type)
    {
        var personnel = await _personalRepository.GetPersonalByIdAndType(new List<string> { sicil }, type);
        var personnelList = personnel?.ToList();
        if (personnelList == null || personnelList.Count == 0 || personnelList[0].Duties == null)
        {
            return Enumerable.Empty<IDuty>();
        }
        if (isPaidDuties)
        {
            return await _dutyRepository.GetDutiesByIdAndTypeWithPagination(personnelList[0].PaidDuties, page, pageSize, type);
        }
        return await _dutyRepository.GetDutiesByIdAndTypeWithPagination(personnelList[0].Duties, page, pageSize, type);
    }
    public async Task<object> InsertDuties(int type)
    {
        var files = await ProcessDutyExcelFilesAsync(type);
        var records = await _dutyRepository.GetDutiesByIdAndType(files.Select(x => x.DutyId).ToList(), type);
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

    public async Task<IEnumerable<IDuty>> ProcessDutyExcelFilesAsync(int type)
    {
        List<string> files = _excelService.FindFiles(type);
        return await PrepareProcessedDutiesListAsync(files, type);
    }
    private async Task<IEnumerable<IDuty>> PrepareProcessedDutiesListAsync(List<string> files, int type)
    {
        List<IDuty> duties = new List<IDuty>();
        foreach (string file in files)
        {
            PersonalInDuty personalInDuty = _excelService.ReadDutyFile(file + ".xlsx", type);
            await InsertPersonal(personalInDuty.ResponsibleManagers, type);
            await InsertPersonal(personalInDuty.PoliceAttendants, type);
            _ = SaveAssignment(file.Split("-")[0].Trim(), personalInDuty, type);
            _ = _personalRepository.PushDutyIdToDutyArray(file.Split("-")[0].Trim(), personalInDuty.ResponsibleManagers.Select(x => x.Sicil).ToList(), type);
            _ = _personalRepository.PushDutyIdToDutyArray(file.Split("-")[0].Trim(), personalInDuty.PoliceAttendants.Select(x => x.Sicil).ToList(), type);
            // if (type == (int)PersonnelTypeEnum.CEVIK)
            // {
            //     _ = _personalRepository.PushDutyIdToPaidDutyArray(file.Split("-")[0].Trim(), personalInDuty.ResponsibleManagers.Select(x => x.Sicil).ToList(), type);
            //     _ = _personalRepository.PushDutyIdToPaidDutyArray(file.Split("-")[0].Trim(), personalInDuty.PoliceAttendants.Select(x => x.Sicil).ToList(), type);
            // }
            duties.Add(CreateProcessedDutyObject(file, type));
        }
        return duties;
    }
    private IDuty CreateProcessedDutyObject(string file, int type)
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
            Date = new DateTime(int.Parse(dateInfo[2]), int.Parse(dateInfo[1]), int.Parse(dateInfo[0]), 0, 0, 0),
            Type = type
        };
    }
    private async Task InsertPersonal(IEnumerable<IPersonalExcel> personalExcel, int type)
    {
        var records = await _personalRepository.GetPersonalByIdAndType(personalExcel.Select(x => x.Sicil).ToList(), type);
        // we only want to insert records that do not exist in the database
        var notExistedPersonal = personalExcel.Where(x => !records.Any(y => y.Sicil == x.Sicil)).ToList();
        if (notExistedPersonal.Count() != 0)
        {
            // set type
            foreach (var personal in notExistedPersonal)
            {
                personal.Type = type;
            }
            await _personalRepository.InsertManyAsync(notExistedPersonal);
        }
    }
    private async Task SaveAssignment(string dutyId, PersonalInDuty personalInDuty, int type)
    {
        var record = await _assignmentRepository.GetAssignmentByDutyIdAndType(dutyId, type);
        // we only want to insert records that do not exist in the database
        if (record != null) return;
        DateTime dateTime = DateTime.Now;
        _ = _assignmentRepository.CreateAsync(new Assignment
        {
            DutyId = dutyId,
            Type = type,
            ResponsibleManagers = personalInDuty.ResponsibleManagers.Select(x => x.Sicil).ToList(),
            PoliceAttendants = personalInDuty.PoliceAttendants.Where(x => !personalInDuty.ResponsibleManagers.Select(y => y.Sicil).Contains(x.Sicil)).Select(x => x.Sicil).ToList(),
            Id = ObjectId.GenerateNewId().ToString(),
            PreviousAssignments = new List<PreviousAssignments>(),
            LastUpdate = dateTime,
            AssignmentDate = dateTime,
            IsActive = false,
            // // if type = 3 then all personnel are paid
            // PaidPersonal = type == (int)PersonnelTypeEnum.CEVIK ? personalInDuty.ResponsibleManagers.Concat(personalInDuty.PoliceAttendants).Select(x => x.Sicil).ToList() : new List<string>(),
            PaidPersonal = new List<string>(),

        });
    }
    public async Task<IEnumerable<PeopleCount>> GetOccurrencesOfSpecificValues(BsonArray specificValues)
    {
        var result = await _assignmentRepository.GetOccurrencesOfSpecificValues(specificValues);
        return result;
    }
    public async Task<DeleteResult> DeleteDuty(string dutyId, int type)
    {
        var assignment = await _assignmentRepository.GeAssignmentByDutyId(dutyId);
        if (assignment != null && (assignment.PaidPersonal.Count() > 0 || assignment.IsActive == true))
        {
            throw new Exception("ASSIGNMENT_EXISTS");
        }

        // we will delete the personnel that are only assigned to this duty
        // because dashboard should not count them as assigned to any duty 
        var personal = await _personalRepository.GetPersonalById(assignment.ResponsibleManagers.Concat(assignment.PoliceAttendants).ToList());
        var personalOnlyInThisDuty = personal.Where(x => x.Duties.Contains(dutyId) && x.Duties.Count() == 1).ToList();
        if (personalOnlyInThisDuty.Count() > 0)
        {
            await _personalRepository.DeleteManyPersonnel(personalOnlyInThisDuty.Select(x => x.Sicil).ToList());
        }
        await _personalRepository.PullDutyIdFromDutyArray(dutyId, assignment.ResponsibleManagers.Concat(assignment.PoliceAttendants).ToList(), type);
        await _assignmentRepository.DeleteAssignment(dutyId);
        return await _dutyRepository.DeleteDuty(dutyId);
    }
}