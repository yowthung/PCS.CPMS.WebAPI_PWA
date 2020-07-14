using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PCS.CPMS.BL.Models;
using PCS.CPMS.DAL;
using PCS.CPMS.WebAPI_PWA.Services;
using PCS.CPMS.WebAPI_PWA.Helpers;
using PCS.CPMS.BL.OthersModel;
using Microsoft.AspNetCore.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    [ApiController]
    // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        //public UserController(IConfiguration config)
        //{
        //    _config = config;
        //}

        private readonly tng_cpmsContext _context;

        private IUserService _userService;


        public UserController(tng_cpmsContext context, IConfiguration config, IUserService userService)
        {
            _context = context;
            _config = config;
            _userService = userService;
        }


        [HttpGet, Route("index")]
        //GET
        public async Task<IActionResult> Index()
        {
            return new JsonResult(await _context.User.ToListAsync());
        }


        //POST: api/btnLogin    
        // [HttpPost]
        [AllowAnonymous]
        [HttpPost, Route("login")]
        public async  Task<ActionResult> Login([FromForm]string useremail, [FromForm] string password)
        {
            if (useremail == null || password == null)
                return Ok(new { Message = "Please fill out the blank field before proceed." });

            var pwd =  Encrypt.EncryptFunction(password);
            //db checking

            Common cm_fn = new Common(_context);

            User user_ = await cm_fn.GetUserLogin(useremail, pwd);

            if (user_ != null)
            {                          
                var token = await _userService.Authenticate(user_);


                Roles roles_ = await _context.Roles.FirstOrDefaultAsync(u => u.Id == token.RoleID);
                token.Role = roles_.RoleName;
                token.Pwd = string.Empty;
                return Ok(token);

                //return Ok(new
                //{
                //    Message = "Success",
                //    Name = user.Name,
                //    Email = user.Email,
                //    Role = user.Role

                //});
            }
            else
            {
                return Ok(new
                {
                    Message = "Invalid email or password. Please try again."
                });
            }
        }

       

        private object AuthenticateResponse(User user, string token)
        {
            throw new NotImplementedException();
        }

        //POST: api/btnRegister
        [HttpPost,Route("createuser")]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {

            if (user.Name == null || user.Email == null || user.Pwd == null)
            {
                return Ok(new { Message = "Please fill out the blank field." });
            }
            else
            {
                //db checking if account existed
                if (_context.User.Any(x => x.Email.Equals(user.Email)))
                {
                    return Ok(new
                    {
                        Message = "Account already exists. Please try another email."
                    });
                }
                else
                {
                    user.Pwd = Encrypt.EncryptFunction(user.Pwd);
                    _context.User.Add(user);
                    _context.SaveChanges();

                    return Ok(new
                    {
                        Message = "Success"
                    });
                }
            }
        }


        //POST: api/btnDeleteUser
        [HttpPost("{id}") , Route("deleteuser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid user id");

            using (_context)
                {
                    var user = _context.User
                        .Where(u => u.Id == id)
                        .FirstOrDefault();

                    _context.Entry(user).State = EntityState.Deleted;
                    _context.SaveChanges();
                }

                return Ok("Successfully delete user");

            }

        //POST: api/btnResetPasssword
        [HttpPost("{user}/{newpassword}"), Route("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody]User user, string newpassword)
        {
            if (user.Email == null || newpassword == null || user.Pwd == null)
            {
                return Ok(new { Message = "Please fill out the blank field." });
            }
            else
            {
                //db checking email 
                if (_context.User.Any(x => x.Email.Equals(user.Email)))
                {

                    if (!user.Pwd.Equals(newpassword))
                    {
                        return Ok(new { Message = "ERROR: Your password and confirmation password do not match." });
                    }
                    else
                    {
                        User user1 = _context.User.Where(i => i.Email.Equals(user.Email)).FirstOrDefault();

                        user1.Pwd = Encrypt.EncryptFunction(newpassword);
                        _context.SaveChanges();

                        return Ok(new
                        {
                            Message = "Success"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        Message = "Account not exists. Please re-enter email"
                    });
                }
                }
            }

        }
    }

