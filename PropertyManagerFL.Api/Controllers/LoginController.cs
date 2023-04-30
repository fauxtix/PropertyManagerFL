using MediaOrganizerApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.ViewModels.Users;
using PropertyManagerFL.Infrastructure.Services.SecurityServices;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IUsersRepository repoUsers;

        public LoginController(ITokenGenerator _tokenGenerator, IPasswordHasher _passwordHasher, IUsersRepository _repoUsers)
        {
            tokenGenerator = _tokenGenerator;
            passwordHasher = _passwordHasher;
            repoUsers = _repoUsers;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(LoginUser userParam)
        {
            // Default to bad username/password message
            IActionResult result = Unauthorized(new { message = "Username or password is incorrect" });

            // Get the user by username
            Users user = await repoUsers.GetUser(userParam.Username);

            // If we found a user and the password hash matches
            if (user != null && passwordHasher.VerifyHashMatch(user.PasswordHash, userParam.Password, user.Salt))
            {
                // Create an authentication token
                string token = tokenGenerator.GenerateToken(user.UserId, user.Username, user.Role);

                // Create a ReturnUser object to return to the client
                LoginResponse retUser = new LoginResponse() { User = new ReturnUser() { UserId = user.UserId, Username = user.Username, Role = user.Role }, Token = token };

                // Switch to 200 OK
                result = Ok(retUser);
            }

            return result;
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterUser userParam)
        {
            IActionResult result;

            Users existingUser = await repoUsers.GetUser(userParam.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already taken. Please choose a different username." });
            }

            Users user = await repoUsers.AddUser(userParam.Username, userParam.Password, userParam.Role);
            if (user != null)
            {
                result = Created(user.Username, null); //values aren't read on client
            }
            else
            {
                result = BadRequest(new { message = "An error occurred and user was not created." });
            }

            return result;
        }

        [HttpGet] //if works try authenticated
        [Authorize]
        public async Task<IActionResult> GetUserRole()
        {
            //returns user with xxxx for salt and password, requires id derived through token
            string IdTokenString = User.FindFirst("sub").Value;
            int IdTokenInt = Convert.ToInt32(IdTokenString);
            Users censoredUser = await repoUsers.GetUserRoleById(IdTokenInt);
            return Ok(censoredUser);
        }
    }
}
