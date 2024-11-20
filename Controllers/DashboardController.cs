using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DutyAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class DashboardController : BaseController<DashboardController, IDashboardService, IDuty, string>
{
    public DashboardController(ILogger<DashboardController> logger, IDashboardService dashboardService) : base(dashboardService, logger)
    {
    }

    [HttpGet("")]
    public async Task<OkObjectResult> GetDashboardData()
    {
        return ApiResultOk(await Service.GetDashboardData());
    }

    [HttpGet("branch")]
    public async Task<OkObjectResult> GetBranchDashboardData([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int type)
    {
        return ApiResultOk(await Service.GetBranchDashboardData(pageNumber, pageSize, type));
    }
}
