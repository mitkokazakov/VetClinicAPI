using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Common;
using VetClinic.Data.Models;
using VetClinic.DTO.Users;

namespace VetClinic.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("user/{token}")]
        public async Task<ActionResult> GetUser(string token)
        {
            var secureKey = configuration["JWT:Secret"];

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secureKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            var finalToken = (JwtSecurityToken)validatedToken;

            var userId = finalToken.Issuer;

            var user = await userManager.FindByIdAsync(userId);

            var roles = await userManager.GetRolesAsync(user);

            var role = " ";

            if (roles.Any())
            {
                role = roles[0];
            }

            return Ok(new
            {
                UserId = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role,
                Phone = user.PhoneNumber,
                Address = user.Address,
                Town = user.Town,
                Email = user.Email
            });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterUserFormModel user)
        {
            var existingUser = await this.userManager.FindByNameAsync(user.Email);

            if (existingUser != null)
            {
                return Conflict("User with this email already exist");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.Email
            };

            var result = await userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(newUser,
                                    "User").Wait();
            }

            return Ok(new Response { Status = "Success", Message = "User has been created successfully" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginUserFormModel user)
        {
            var currentUser = await userManager.FindByNameAsync(user.Username);
            var passwordIsCorrect = await this.userManager.CheckPasswordAsync(currentUser, user.Password);

            if (currentUser != null && passwordIsCorrect)
            {
                //var userRoles = await this.userManager.GetRolesAsync(currentUser);

                //var authClaims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, currentUser.UserName),
                //    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                //};

                //foreach (var role in userRoles)
                //{
                //    authClaims.Add(new Claim(ClaimTypes.Role, role));
                //}

                //var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                //var token = new JwtSecurityToken
                //    (
                //        issuer: this.configuration["JWT:ValidIssuer"],
                //        audience: this.configuration["JWT:ValidAudience"],
                //        expires: DateTime.Now.AddDays(1),
                //        claims: authClaims,
                //        signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                //    );

                var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                var credentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                var header = new JwtHeader(credentials);

                var payLoad = new JwtPayload(currentUser.Id, null, null, null, DateTime.Now.AddDays(1));

                var securityToken = new JwtSecurityToken(header, payLoad);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(securityToken)
                });


            }

            return Unauthorized("User not found");
        }

        [HttpGet]
        [Route("AllUsers")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetAllUsers() 
        {
            var allUsers = await userManager.GetUsersInRoleAsync("User");

            var allUsersViewModel = allUsers.Select(u => new UserViewModel
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Town = u.Town,
                Address = u.Address
            }).ToList();

            return allUsersViewModel;
        }

        [HttpGet]
        [Route("FindUsersByName/{userName}")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> FindUsersByName(string userName)
        {
            var allUsers = await userManager.GetUsersInRoleAsync("User");

            var allUsersViewModel = allUsers.Where(u => u.FirstName.Contains(userName) || u.LastName.Contains(userName)).Select(u => new UserViewModel
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Town = u.Town,
                Address = u.Address
            }).ToList();

            return allUsersViewModel;
        }

        [HttpGet]
        [Route("GetUserById/{userId}")]
        public async Task<ActionResult<UserViewModel>> GetUserById(string userId)
        {
            var currentUser = await userManager.FindByIdAsync(userId);

            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            var userViewModel = new UserViewModel
            {
                UserId = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                Town = currentUser.Town,
                Address = currentUser.Address
            };

            return userViewModel;
        }

        [HttpPost]
        [Route("ChangeUser")]
        public async Task<ActionResult<UserViewModel>> ChangeUser(UserViewModel user)
        {
            var currentUser = await userManager.FindByIdAsync(user.UserId);

            if (currentUser == null)
            {
                return NotFound("User not found");
            }

            currentUser.FirstName = user.FirstName;
            currentUser.LastName = user.LastName;
            currentUser.Email = user.Email;
            currentUser.PhoneNumber = user.Phone;
            currentUser.Address = user.Address;
            currentUser.Town = user.Town;

            var result = await userManager.UpdateAsync(currentUser);

            if (result.Succeeded)
            {
                return Ok("User has been changed successfully");
            }

            return Unauthorized();
        }
    }
}
