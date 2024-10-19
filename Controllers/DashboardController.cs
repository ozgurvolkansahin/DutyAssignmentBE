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
    public async Task<OkObjectResult> GetDashboardData([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return ApiResultOk(await Service.GetDashboardData(pageNumber, pageSize));
    }
}
