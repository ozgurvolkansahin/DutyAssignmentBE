using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
    [HttpPost("GetDutiesByIdList")]
    public async Task<OkObjectResult> GetDutiesByIdList([FromBody] GetDutiesByIdList dutyIds)
    {
        return ApiResultOk(await Service.GetDutiesByIdListWithPagination(dutyIds.sicil, dutyIds.page, dutyIds.pageSize, dutyIds.isPaidDuties));
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
    [HttpPost("GetOccurrencesOfSpecificValues")]
    public async Task<OkObjectResult> GetOccurrencesOfSpecificValues([FromBody] BsonArray specificValues)
    {
        return ApiResultOk(await Service.GetOccurrencesOfSpecificValues(specificValues));
    }
}
