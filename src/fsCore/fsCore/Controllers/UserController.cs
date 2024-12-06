using Common.Models;
using fsCore.ApiModels;
using fsCore.Attributes;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;
using System.Net;

namespace fsCore.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService, ICachingService cachingService) : base(logger, cachingService)
        {
            _userService = userService;
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser(true)]
        [HttpGet("ChangeUsername")]
        public async Task<IActionResult> ChangeUserName(string newUsername)
        {
            var user = await GetCurrentUserAsync();
            return Ok((await _userService.SaveUser(new User(user.Email, user.EmailVerified, user.Name, newUsername, user.Id))).Id);
        }
        [ProducesDefaultResponseType(typeof(User))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUser]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetOneUser(Guid userId)
        {
            return Ok(await _userService.GetUser(userId, await GetCurrentUserWithPermissionsAsync()));
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
        [RequiredUser(true)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("SelfWithGroupPermissions")]
        public async Task<IActionResult> GetUserWithPermissions()
        {
            return Ok((await GetCurrentUserWithPermissionsAsync()).ToRawUserPermission());
        }

    }
}