using DutyAssignment.Interfaces;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DutyAssignment.Services;

public class DutyService: IDutyService
{
    private readonly IDutyRepository _dutyRepository;

    public DutyService(IDutyRepository dutyRepository)
    {
        _dutyRepository = dutyRepository;
    }
    public async Task<IEnumerable<IDuty>> GetDuties()
    {
        return await _dutyRepository.GetDutiesByDateAsync();
    }
}