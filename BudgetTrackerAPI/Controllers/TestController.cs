using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;

namespace BudgetTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("secured")]
        [Authorize] // **[Authorize] Attribute - Secures this endpoint**
        public IActionResult GetSecuredData()
        {
            return Ok(new { message = "This is secured data, only accessible with a valid JWT." });
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok(new { message = "This is public data, accessible without JWT." });
        }
    }
}