using DutyAssignment.DTOs;
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

    public async Task<IEnumerable<IDutyAssignments>> GetAssignments(IEnumerable<string> dutyIds, int numToSelect, bool reAssign)
    {
        List<IDutyAssignments> assignments = new List<IDutyAssignments>();
        foreach (var duty in dutyIds)
        {
            var assignment = await SelectPersonalToBePaid(duty, numToSelect, reAssign);
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
    public async Task<IEnumerable<PersonalExcel>> SelectPersonalToBePaid(string duty, int numToSelect, bool reAssign)
    {

        var assignment = await _assignmentRepository.GeAssignmentByDutyId(duty);
        // check if the number of people to select is greater than the number of people assigned to this duty
        if (numToSelect > assignment.ResponsibleManagers.Count() + assignment.PoliceAttendants.Count()) {
            throw new Exception("TOO_MANY_PEOPLE_TO_SELECT");
        }
        if ((assignment == null || assignment.PaidPersonal.Count() > 0) && reAssign == false)
        {
            return new List<PersonalExcel>();
        }
        if (reAssign == true)
        {
            await _assignmentRepository.SetAssignmentUnPaid(duty, assignment.PaidPersonal, assignment.AssignmentDate);
            await _personalRepository.RemoveDutyIdFromPaidDutyArray(duty);
        }
        // merge ResponsibleManagers and PoliceAttendants arrays as BsonArray
        var personals = await _personalRepository.GetPersonalById(assignment.PoliceAttendants);
        // List<PersonalExcel> selectedPeople = new List<PersonalExcel>();
        List<PersonalExcel> selectedPeople = new List<PersonalExcel>();

        // ResponsibleManagers are 100% selected
        // add them to selectedPeople
        var responsibleManagers = await _personalRepository.GetPersonalById(assignment.ResponsibleManagers);
        selectedPeople = selectedPeople.Concat(responsibleManagers.Cast<PersonalExcel>().ToList()).ToList();
        // remove ResponsibleManagers from personals
        personals = personals.Where(x => !assignment.ResponsibleManagers.Contains(x.Sicil)).ToList();
        
        // add a property to each personal in personals which will be assigned by the formula below
        // ((number of duties assigned to a person) / 3) - (number of paid duties assigned to a person)
        // add +1 because this may be their 3rd duty
        foreach (var x in personals)
        {
            x.Priority = (((x.Duties.Count()+1) / 3) - x.PaidDuties.Count()) >= 1 ? true : false;
        }
        var prioritizedPersonnel = personals.Where(x => x.Priority == true).ToList();
        // if there are people with priority, select them first
        if (prioritizedPersonnel.Count() > 0)
        {
            selectedPeople = selectedPeople.Concat(prioritizedPersonnel.Cast<PersonalExcel>().ToList()).ToList();
            personals = personals.Where(x => !prioritizedPersonnel.Contains(x)).ToList();
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
                .Take(numToSelect - responsibleManagers.Count())
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
        // push dutyId to PaidDuties array and update the database
        await _personalRepository.PushDutyIdToPaidDutyArray(duty, selectedPeople.Select(x => x.Sicil).ToList());
        await _assignmentRepository.SetAssignmentPaid(duty, selectedPeople.Select(x => x.Sicil).ToList());
        return selectedPeople.AsEnumerable();
    }
    public async Task<byte[]> DownloadPersonalReportForSpecificDuty(string dutyId)
    {
        var assignment = await _assignmentRepository.GeAssignmentByDutyId(dutyId);
        var personnel = await _personalRepository.GetPersonalById(assignment.PaidPersonal.ToList());
        return _excelService.DownloadExcel(personnel);
    }

    public async Task<FilterAssignmentsByFilter> FilterAssignments(FilterAssignments filterAssignments)
    {
        return await _assignmentRepository.FilterAssignments(filterAssignments);
    }
    public async Task<UpdateResult> ResetAssignment(string dutyId)
    {
        await _personalRepository.ResetAssignment(dutyId);
        return await _assignmentRepository.ResetAssignment(dutyId);
    }
}