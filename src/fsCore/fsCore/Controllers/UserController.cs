using System.Net;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Controllers
{
    [RequiredUser]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("Self")]
        public async Task<IActionResult> GetSelf()
        {
            return Ok(_getCurrentUser());
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("ChangeUsername")]
        public async Task<IActionResult> ChangeUserName(string newUsername)
        {
            var user = _getCurrentUser();
            user.Username = newUsername;
            return Ok(await _userService.SaveUser(user));
        }
    }
}