using DutyAssignment.Models;
using DutyAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DutyAssignment.Controllers;

[ApiController]
[Route("[controller]")]
public class DutyController : ControllerBase
{
    private readonly ILogger<DutyController> _logger;
    private readonly IDutyService _dutyService;


    public DutyController(ILogger<DutyController> logger, IDutyService dutyService)
    {
        _logger = logger;
        _dutyService = dutyService;
    }

    public ILogger<DutyController> Logger => _logger;

    [HttpGet("GetDuty")]
    public IEnumerable<Duty> Get()
    {
        return _dutyService.GetAsync().Result;
    }

}
