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
        return ApiResultOk(await Service.GetDutiesByIdListAndTypeWithPagination(dutyIds.sicil, dutyIds.page, dutyIds.pageSize, dutyIds.isPaidDuties, dutyIds.type));
    }
    // [HttpGet("ProcessDutyExcelFiles")]
    // public OkObjectResult ProcessDutyExcelFiles()
    // {
    //     return ApiResultOk(Service.ProcessDutyExcelFilesAsync());
    // }
    [HttpGet("InsertDuties")]
    public async Task<OkObjectResult> InsertDuties([FromQuery] int type)
    {
        return ApiResultOk(await Service.InsertDuties(type));
    }
    [HttpPost("GetOccurrencesOfSpecificValues")]
    public async Task<OkObjectResult> GetOccurrencesOfSpecificValues([FromBody] BsonArray specificValues)
    {
        return ApiResultOk(await Service.GetOccurrencesOfSpecificValues(specificValues));
    }
    [HttpGet("Delete")]
    public async Task<IActionResult> DeleteDuty([FromQuery] string dutyId, [FromQuery] int type)
    {
        try
        {
            return ApiResultOk(await Service.DeleteDuty(dutyId, type));
        }
        catch (Exception ex)
        {
            return ApiResultNotOk(ex.Message);
        }
    }
}
