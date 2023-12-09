using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PluralSight_CityAPI.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        // Replica of what should be a table in user DB.
        // Same logic. Keep a hash of the password in the DB.
        private class CityInfoUser
        {
            public string UserName { get; set; }
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(string userName, int userId, string firstName, string lastName, string city)
            {
                UserName = userName;
                UserId = userId;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        // Scoped class AuthenRequestBody
        public class AuthenRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        // Configuration injections
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenRequestBody req)
        {
            // 1. Validate the user
            var user = ValidateUserCred(req.Username, req.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            // 2. Generate token: Generate the key (which can be store directly in some key vault),
            // and sign it on the payloads/ user info (represented as Claims)

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretInfoKey"]));
            var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim("sub", user.UserId.ToString()));
            claims.Add(new Claim("given_name", user.FirstName));
            claims.Add(new Claim("family_name", user.LastName));
            claims.Add(new Claim("city", user.City));

            var jwt_token = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredential);

            var tokenReturn = new JwtSecurityTokenHandler().WriteToken(jwt_token);

            return Ok(tokenReturn);
        }

        private CityInfoUser ValidateUserCred(string username, string password)
        {
            // Replica of the UserDB. Should return actual record from DB,
            // but mocked up since used for token demonstration purpose
            return new CityInfoUser("Hehe", 1, ":D", "Smile", "Melbourne");
        }
    }
}
