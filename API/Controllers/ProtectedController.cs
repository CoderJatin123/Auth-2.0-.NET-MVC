using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProtectedController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProtectedData()
    {
        var username = User.Identity?.Name ?? "Unknown User";
        var userId = "Unknown";
        
        foreach (var userClaim in User.Claims)
        {
            if (userClaim.Type == "Userid")
            {
                Claim claim = userClaim;
                userId = claim.Value;
            }
        }
        
        return Ok(new { message = "This is a protected resource!", user = userId });
    }
}