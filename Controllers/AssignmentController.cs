using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DutyAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController : BaseController<AssignmentController, IAssignmentService, IDuty, string>
{
    public AssignmentController(ILogger<AssignmentController> logger, IAssignmentService assignmentService) : base(assignmentService, logger)
    {
    }

    [HttpPost("SelectPersonalToBePaid")]
    public async Task<IActionResult> SelectPersonalToBePaid([FromBody] SelectPersonalToBePaid selectPersonalToBePaid)
    {
        try
        {
            return ApiResultOk(await Service.GetAssignments(selectPersonalToBePaid.dutyIds, selectPersonalToBePaid.assignmentCount, selectPersonalToBePaid.reAssign, selectPersonalToBePaid.type));
        }
        catch (Exception ex)
        {
            return ApiResultNotOk(ex.Message);
        }
    }
    [HttpGet("PaidAssignments")]
    public async Task<OkObjectResult> PaidAssignments([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return ApiResultOk(await Service.GetPaidAssignments(pageNumber, pageSize));
    }
    [HttpPost("GetAssignedPersonalByDutyIdAndTypeWithPagination")]
    public async Task<OkObjectResult> GetAssignedPersonalByDutyIdAndTypeWithPagination([FromBody] GetAssignedPersonalByDutyIdWithPaginationPostObject getAssignedPersonalByDutyIdWithPaginationPostObject)
    {
        return ApiResultOk(await Service.GetAssignedPersonalByDutyIdAndTypeWithPagination(getAssignedPersonalByDutyIdWithPaginationPostObject.dutyId, getAssignedPersonalByDutyIdWithPaginationPostObject.page, getAssignedPersonalByDutyIdWithPaginationPostObject.pageSize, getAssignedPersonalByDutyIdWithPaginationPostObject.type, getAssignedPersonalByDutyIdWithPaginationPostObject.isAll));
    }
    [HttpGet("DownloadPersonalReportForSpecificDuty")]
    public async Task<IActionResult> DownloadPersonalReportForSpecificDuty([FromQuery] string dutyId, [FromQuery] int type)
    {
        var fileBytes = await Service.DownloadPersonalReportForSpecificDuty(dutyId, type);
        var docName = $"{dutyId}_OdemeListesi.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", docName);
    }
    [HttpGet("DownloadAllPersonnelWithType")]
    public async Task<IActionResult> DownloadAllPersonnelWithType([FromQuery] int type)
    {
        var fileBytes = await Service.DownloadAllPersonnelWithType(type);
        var docName = "PersonelListesi.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", docName);
    }

    [HttpPost("Filter")]
    public async Task<OkObjectResult> FilterAssignments([FromBody] FilterAssignments filterAssignments)
    {
        return ApiResultOk(await Service.FilterAssignments(filterAssignments));
    }

    [HttpGet("ResetAssignment")]
    public async Task<OkObjectResult> ResetAssignment([FromQuery] string dutyId, [FromQuery] int type)
    {
        return ApiResultOk(await Service.ResetAssignment(dutyId, type));
    }
    [HttpGet("ProcessPaidDuties")]
    public async Task<OkObjectResult> ProcessPaidDuties()
    {
        return ApiResultOk(await Service.ProcessPaidDuties());
    }
}
