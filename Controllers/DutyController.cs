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

    [HttpGet("GetDuties")]
    public async Task<OkObjectResult> GetDuties()
    {
        return ApiResultOk(await Service.GetDuties());
    }
    [HttpGet("ProcessDutyExcelFiles")]
    public OkObjectResult ProcessDutyExcelFiles()
    {
        return ApiResultOk(Service.ProcessDutyExcelFilesAsync());
    }
    [HttpGet("InsertDuties")]
    public async Task<OkObjectResult> InsertDuties()
    {
        return ApiResultOk(await Service.InsertDuties());
    }
    [HttpGet("AssignPersonalForPayment")]
    public async Task<OkObjectResult> AssignPersonalForPayment()
    {
        return ApiResultOk(await Service.GetOccurrencesOfSpecificValues(new string[] { "Personal for Payment" }));
    }
}
