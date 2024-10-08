using DutyAssignment.Interfaces;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DutyAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class DutyController : BaseController<DutyController, IDutyService, IDuty, string>
{
    public DutyController(ILogger<DutyController> logger, IDutyService dutyService) : base(dutyService, logger)
    {
    }


    [HttpGet("GetDutiesByDate")]
    public async Task<IEnumerable<IDuty>> GetAsync()
    {
        return await Service.GetDuties();
    }

}
