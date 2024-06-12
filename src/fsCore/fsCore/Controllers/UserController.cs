using System.Net;
using Common.Models;
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
        [HttpGet("ChangeUsername")]
        public async Task<IActionResult> ChangeUserName(string newUsername)
        {
            var user = GetCurrentUser();
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
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("SelfWithGroupPermissions")]
        public async Task<IActionResult> GetUserWithPermissions()
        {
            return Ok(RawUserPermission.FromUserWithPermissions(GetCurrentUserWithPermissions()));
        }
    }
}