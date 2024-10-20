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

    public AssignmentService(IDutyRepository dutyRepository, IPersonalRepository personalRepository, IAssignmentRepository assignmentRepository)
    {
        _dutyRepository = dutyRepository;
        _personalRepository = personalRepository;
        _assignmentRepository = assignmentRepository;
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
}