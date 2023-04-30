//using PropertyManagerFL.Api.Filter;
using Microsoft.AspNetCore.Mvc;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    //[TypeFilter(typeof(AuthorizationFilterAttribute))]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}