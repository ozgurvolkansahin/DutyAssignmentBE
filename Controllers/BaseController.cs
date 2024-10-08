using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Net;
using DutyAssignment.Interfaces;
using DutyAssignment.Repositories.Mongo;

namespace DutyAssignment.Controllers
{
    [ApiController]
    [Route("[controller]")]
    // [Authorize]
    public abstract class BaseController<TController, TService, TEntity, TId> : BaseController<TController>
        where TEntity : IEntity<TId>
    {
        protected TService Service { get; }

        public BaseController(TService service, ILogger<TController> logger) : base(logger)
        {
            Service = service;
        }

        public OkObjectResult ApiResultOk<T>(T result, bool success = true, int itemCount = 1)
        {
            if (result is ICollection collection && itemCount == 1)
            {
                itemCount = collection.Count;
            }
            return Ok(new
            {
                success,
                data = result,
                itemCount,
                status = (int)HttpStatusCode.OK
            });
        }
    }

    public abstract class BaseController<TController> : ControllerBase
    {
        protected ILogger<TController> Logger { get; }
        // private IUser qlUser;
        // protected IUser QLUser => qlUser ??= GetUser().Result;

        public BaseController(ILogger<TController> logger)
        {
            Logger = logger;
        }

        public OkObjectResult OkResult(object data)
        {

            return Ok(new
            {
                payload = data,
                result_code = 200,
                result_message = "success"
            });
        }

        // protected async Task<IUser> GetUser()
        // {
        //     var claimsIdentity = User.Identity as ClaimsIdentity;
        //     var userEmail = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        //     var userRepository = HttpContext.RequestServices.GetService<IUserRepository>();

        //     return await userRepository.FindById(userEmail);
        // }
    }
}