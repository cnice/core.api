using System; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using core.api.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace core.api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        #region Private Variables
        private IConfiguration _config;
        #endregion

        #region Constructors
        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Wecome!" };
        }

        // POST api/values
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                user.Token = BuildToken(user);
                response = Ok(new { user });
            }

            return response;
        }

        [HttpGet, Authorize]
        [Route("GetUserContactInfo")]
        public IActionResult GetUserContactInfo()
        {
            IActionResult response = Unauthorized();

            var t = HttpContext.Request.Headers;

            ContactInfo contactInfo = new ContactInfo{
                Address = "188 Simple Ave",
                State = "North Carolina",
                ZipCode = 21822
            };

            return response = Ok(new { contactInfo });
        }

        #region Private Methods
        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            if (login.Username == "username" && login.Password == "password")
            {
                user = new UserModel { Name = "Username", Email = "username@gmail.com" };
            }
            return user;
        }

        private string BuildToken(UserModel user)
        {

            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(10),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
       
    }
}
