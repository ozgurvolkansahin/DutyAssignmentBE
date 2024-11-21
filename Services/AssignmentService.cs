using DutyAssignment.DTOs;
using DutyAssignment.Enum;
using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DutyAssignment.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IDutyRepository _dutyRepository;
    private readonly IPersonalRepository _personalRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IExcelService _excelService;

    public AssignmentService(IDutyRepository dutyRepository, IPersonalRepository personalRepository, IAssignmentRepository assignmentRepository, IExcelService excelService)
    {
        _dutyRepository = dutyRepository;
        _personalRepository = personalRepository;
        _assignmentRepository = assignmentRepository;
        _excelService = excelService;
    }

    public async Task<IEnumerable<IDutyAssignments>> GetAssignments(IEnumerable<string> dutyIds, int numToSelect, bool reAssign, int type)
    {
        List<IDutyAssignments> assignments = new List<IDutyAssignments>();
        foreach (var duty in dutyIds)
        {
            var assignment = await SelectPersonalToBePaid(duty, numToSelect, reAssign, type);
            assignments.Add(new DutyAssignments
            {
                DutyId = duty,
                Personal = assignment,
                Date = DateTime.Now
            });
        }
        return assignments;
    }
    public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<object>> GetPaidAssignments(int pageNumber, int pageSize)
    {
        return await _assignmentRepository.GetPaidAssignments(pageNumber, pageSize);
    }
    public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdWithPagination(string dutyId, int page, int pageSize)
    {
        return await _assignmentRepository.GetAssignedPersonalByDutyIdWithPagination(dutyId, page, pageSize);
    }
    public async Task<IGetAssignedPersonalByDutyIdWithPaginationResult<IPersonalExcel>> GetAssignedPersonalByDutyIdAndTypeWithPagination(string dutyId, int page, int pageSize, int type)
    {
        return await _assignmentRepository.GetAssignedPersonalByDutyIdAndTypeWithPagination(dutyId, page, pageSize, type);
    }
    public async Task<IEnumerable<PersonalExcel>> SelectPersonalToBePaid(string duty, int numToSelect, bool reAssign, int type)
    {

        var assignment = await _assignmentRepository.GetAssignmentByDutyIdAndType(duty, type);
        // check if the number of people to select is greater than the number of people assigned to this duty
        if (numToSelect > assignment.ResponsibleManagers.Count() + assignment.PoliceAttendants.Count())
        {
            throw new Exception("TOO_MANY_PEOPLE_TO_SELECT");
        }
        if ((assignment == null || assignment.PaidPersonal.Count() > 0) && reAssign == false)
        {
            return new List<PersonalExcel>();
        }
        if (reAssign == true)
        {
            await _assignmentRepository.SetAssignmentUnPaid(duty, assignment.PaidPersonal, assignment.AssignmentDate, type);
            await _personalRepository.RemoveDutyIdFromPaidDutyArray(duty, type);
        }
        // merge ResponsibleManagers and PoliceAttendants arrays as BsonArray
        var personals = await _personalRepository.GetPersonalByIdAndType(assignment.PoliceAttendants, type);
        // remove personals whos Birim is İlDışı Tayin
        personals = personals.Where(x => x.Birim != "İlDışı Tayin").ToList();
        // ResponsibleManagers are 100% selected
        // add them to selectedPeople
        var responsibleManagers = await _personalRepository.GetPersonalByIdAndType(assignment.ResponsibleManagers, type);
        List<PersonalExcel> selectedPeople = new List<PersonalExcel>();
        if (type == (int)PersonnelTypeEnum.KADRO)
        {
            selectedPeople = SelectKadroPersonnelToBePaid(assignment, personals, responsibleManagers, numToSelect, type);
        }
        else if (type == (int)PersonnelTypeEnum.SUBE)
        {
            selectedPeople = SelectSubePersonnelToBePaid(assignment, personals, responsibleManagers, numToSelect, type);
        }
        // push dutyId to PaidDuties array and update the database
        await _personalRepository.PushDutyIdToPaidDutyArray(duty, selectedPeople.Select(x => x.Sicil).ToList(), type);
        await _assignmentRepository.SetAssignmentPaid(duty, selectedPeople.Select(x => x.Sicil).ToList(), type);
        return selectedPeople.AsEnumerable();
    }
    public async Task<byte[]> DownloadPersonalReportForSpecificDuty(string dutyId, int type)
    {
        var assignment = await _assignmentRepository.GetAssignmentByDutyIdAndType(dutyId, type);
        var personnel = await _personalRepository.GetPersonalByIdAndType(assignment.PaidPersonal.ToList(), type);
        return _excelService.DownloadExcel(personnel);
    }
    public async Task<byte[]> DownloadAllPersonnelWithType(int type)
    {
        var personnel = await _personalRepository.GetAllPersonnelWithType(type);
        return _excelService.DownloadExcelWoTemplate(personnel);
    }

    public async Task<FilterAssignmentsByFilter> FilterAssignments(FilterAssignments filterAssignments)
    {
        return await _assignmentRepository.FilterAssignments(filterAssignments);
    }
    public async Task<UpdateResult> ResetAssignment(string dutyId, int type)
    {
        await _personalRepository.ResetAssignment(dutyId, type);
        return await _assignmentRepository.ResetAssignment(dutyId, type);
    }
    public async Task<string> ProcessPaidDuties()
    {
        var personnel = _excelService.ProcessPaymentFileExcelAndReturnPersonnel();
        foreach (var person in personnel)
        {
            // get dutyId in person

            var assignment = await _assignmentRepository.GetAssignmentByDutyIdAndType(person.DutyId, 1);
            if (assignment.PaidPersonal.Count() == 0)
            {
                await _assignmentRepository.SetAssignmentPaid(person.DutyId, person.Personnel.ToList(), 1);
                await _personalRepository.PushDutyIdToDutyArray(person.DutyId, person.Personnel.ToList(), 1);
                await _personalRepository.PushDutyIdToPaidDutyArray(person.DutyId, person.Personnel.ToList(), 1);
            }
        }
        return personnel.Count().ToString();
    }

    private List<PersonalExcel> SelectKadroPersonnelToBePaid(IAssignment assignment, IEnumerable<IPersonalExcel> personals, IEnumerable<IPersonalExcel> responsibleManagers, int numToSelect, int type)
    {
        List<PersonalExcel> selectedPeople = new List<PersonalExcel>();
        // ResponsibleManagers are 100% selected
        // add them to selectedPeople
        selectedPeople = selectedPeople.Concat(responsibleManagers.Cast<PersonalExcel>().ToList()).ToList();
        // remove ResponsibleManagers from personals
        personals = personals.Where(x => !assignment.ResponsibleManagers.Contains(x.Sicil)).ToList();

        // add a property to each personal in personals which will be assigned by the formula below
        // ((number of duties assigned to a person) / 3) - (number of paid duties assigned to a person)
        // add +1 because this may be their 3rd duty
        foreach (var x in personals)
        {
            // x.Priority = (((x.Duties.Count() + 1) / 3) - x.PaidDuties.Count()) >= 1 ? true : false;
            x.Priority = x.Duties.Count() >= (x.PaidDuties.Count() + 1) * 3 ? true : false;
        }
        var prioritizedPersonnel = personals.Where(x => x.Priority == true).ToList();
        // if there are people with priority, select them first
        if (prioritizedPersonnel.Count() > 0)
        {
            if (prioritizedPersonnel.Count() > numToSelect - responsibleManagers.Count())
            {
                selectedPeople = selectedPeople.Concat(prioritizedPersonnel.OrderBy(x => x.Duties.Count()).Take(numToSelect - responsibleManagers.Count()).Cast<PersonalExcel>().ToList()).ToList();
                personals = personals.Where(x => !prioritizedPersonnel.Contains(x)).ToList();
            }
            else
            {
                selectedPeople = selectedPeople.Concat(prioritizedPersonnel.Cast<PersonalExcel>().ToList()).ToList();
                personals = personals.Where(x => !prioritizedPersonnel.Contains(x)).ToList();
            }
        }
        // find min and max number of paid duties assigned to a person assigned to this duty
        int min = personals.Min(x => x.PaidDuties.Count());
        int max = personals.Max(x => x.PaidDuties.Count());

        // start random selection process
        var random = new Random();
        if (min == max)
        {
            // if all people have the same number of paid duties, select numToSelect people randomly
            selectedPeople = selectedPeople.Concat(personals.OrderBy(x => random.Next())
                .Take(numToSelect - selectedPeople.Count())
                .Cast<PersonalExcel>()
                .ToList()).ToList();
        }
        else
        {
            // pick random people starting from the min value until the max value and select numToSelect people
            for (int i = min; i < max; i++)
            {
                selectedPeople = selectedPeople.Concat(personals
                    .Where(x => x.PaidDuties.Count() == i)
                    .OrderBy(x => random.Next())
                    .Take(numToSelect - selectedPeople.Count())
                    .Cast<PersonalExcel>()
                    .ToList()).ToList();
                if (selectedPeople.Count() == numToSelect)
                {
                    // break the loop if the required number of people are selected
                    break;
                }
            }
        }
        return selectedPeople;
    }
    private List<PersonalExcel> SelectSubePersonnelToBePaid(IAssignment assignment, IEnumerable<IPersonalExcel> personals, IEnumerable<IPersonalExcel> responsibleManagers, int numToSelect, int type)
    {
        List<PersonalExcel> selectedPeople = new List<PersonalExcel>();
        // ResponsibleManagers are 100% selected
        // add them to selectedPeople
        selectedPeople = selectedPeople.Concat(responsibleManagers.Cast<PersonalExcel>().ToList()).ToList();
        // remove ResponsibleManagers from personals
        personals = personals.Where(x => !assignment.ResponsibleManagers.Contains(x.Sicil)).ToList();

        // add a property to each personal in personals which will be assigned by the formula below
        // ((number of duties assigned to a person) / 3) - (number of paid duties assigned to a person)
        // add +1 because this may be their 3rd duty
        foreach (var x in personals)
        {
            x.Priority = x.Duties.Count() >= (x.PaidDuties.Count() + 1) * 2 ? true : false;
        }
        var prioritizedPersonnel = personals.Where(x => x.Priority == true).ToList();
        // if there are people with priority, select them first
        if (prioritizedPersonnel.Count() > 0)
        {
            if (prioritizedPersonnel.Count() > numToSelect - responsibleManagers.Count())
            {
                selectedPeople = selectedPeople.Concat(prioritizedPersonnel.OrderBy(x => x.Duties.Count()).Take(numToSelect - responsibleManagers.Count()).Cast<PersonalExcel>().ToList()).ToList();
            }
            else
            {
                selectedPeople = selectedPeople.Concat(prioritizedPersonnel.Cast<PersonalExcel>().ToList()).ToList();
            }
        }
        return selectedPeople;
    }
}