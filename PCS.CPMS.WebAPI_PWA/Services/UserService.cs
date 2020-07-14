using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PCS.CPMS.BL.Models;
using PCS.CPMS.DAL;
using PCS.CPMS.WebAPI_PWA.Helpers;
using PCS.CPMS.BL.OthersModel;

namespace PCS.CPMS.WebAPI_PWA.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(User model);
       // IEnumerable<User> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{ 
        //    new User { Id = 1, Name = "Test", Pwd = "User", Email = "test", Role = "test" } 
        //};

        private readonly tng_cpmsContext _context;

        //public UserService(tng_cpmsContext context)
        //{
        //    _context = context;
        //}

        private readonly AppSettings _appSettings;
        private readonly IConfiguration Configuration;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(User user_)
        {
            // var user = _context.user.SingleOrDefault(x => x.Username == Username && x.Password == Password);

            // return null if user not found
            // if (user == null) return null;

            // authentication successful so generate jwt token

            var token = await generateJwtToken(user_);
            return new AuthenticateResponse(user_, token);
        }

        //public IEnumerable<User> GetAll()
        //{
        //    return _users;
        //}

        // helper methods

        private async Task<string> generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token =  tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}