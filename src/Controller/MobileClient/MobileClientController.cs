using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.MobileClient
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class MobileClientController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly 
        
    }
}