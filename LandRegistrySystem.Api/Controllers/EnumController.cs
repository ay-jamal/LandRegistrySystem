using LandRegistrySystem_Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LandRegistrySystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumController : ControllerBase
    {

        [HttpGet("branch-names")]
        public ActionResult<IEnumerable<string>> GetBranchNames()
        {
            var names = Enum.GetNames(typeof(BranchNames)).ToList();
            return Ok(names);
        }
    }
}
