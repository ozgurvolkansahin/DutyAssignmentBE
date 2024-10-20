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
    public async Task<OkObjectResult> SelectPersonalToBePaid([FromBody] SelectPersonalToBePaid selectPersonalToBePaid)
    {
        return ApiResultOk(await Service.GetAssignments(selectPersonalToBePaid.dutyIds, selectPersonalToBePaid.assignmentCount, selectPersonalToBePaid.reAssign));
    }
    [HttpGet("PaidAssignments")]
    public async Task<OkObjectResult> PaidAssignments([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return ApiResultOk(await Service.GetPaidAssignments(pageNumber, pageSize));
    }
    [HttpPost("GetAssignedPersonalByDutyIdWithPagination")]
    public async Task<OkObjectResult>  GetAssignedPersonalByDutyIdWithPagination([FromBody] GetAssignedPersonalByDutyIdWithPaginationPostObject getAssignedPersonalByDutyIdWithPaginationPostObject)
    {
        return ApiResultOk(await Service.GetAssignedPersonalByDutyIdWithPagination(getAssignedPersonalByDutyIdWithPaginationPostObject.dutyId, getAssignedPersonalByDutyIdWithPaginationPostObject.page, getAssignedPersonalByDutyIdWithPaginationPostObject.pageSize));
    }

}
