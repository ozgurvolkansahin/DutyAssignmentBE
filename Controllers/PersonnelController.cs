using DutyAssignment.DTOs;
using DutyAssignment.Interfaces;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DutyAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonnelController : BaseController<PersonnelController, IPersonalRepository, IPersonalExcel, string>
{
    public PersonnelController(ILogger<PersonnelController> logger, IPersonalRepository personalRepository) : base(personalRepository, logger)
    {
    }

    [HttpGet("")]
    public async Task<OkObjectResult> GetDashboardData([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        return ApiResultOk(await Service.GetPersonalWithPagination(pageNumber, pageSize));
    }
}