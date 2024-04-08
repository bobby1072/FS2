using System.Net;
using Common.Models;
using Common.Permissions;
using fsCore.Controllers.Attributes;
using fsCore.Controllers.ControllerModels;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser(true)]
        [HttpGet("Self")]
        public async Task<IActionResult> GetSelf()
        {
            return Ok(_getCurrentUser());
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser(true)]
        [HttpGet("ChangeUsername")]
        public async Task<IActionResult> ChangeUserName(string newUsername)
        {
            var user = _getCurrentUser();
            user.Username = newUsername;
            return Ok((await _userService.SaveUser(new User(user.Email, user.EmailVerified, user.Name, user.Username, user.Id))).Id);
        }
        [ProducesDefaultResponseType(typeof(UserWithoutEmail[]))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser]
        [HttpGet("SearchUsers")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            return Ok(await _userService.SearchUsers(searchTerm));
        }
        [ProducesDefaultResponseType(typeof(RawUserPermission))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser]
        [RequiredUserWithPermissions(true)]
        [HttpGet("SelfWithGroupPermissions")]
        public async Task<IActionResult> GetUserWithPermissions()
        {
            return Ok(RawUserPermission.FromUserWithPermissions(_getCurrentUserWithPermissions()));
        }
    }
}