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
}
