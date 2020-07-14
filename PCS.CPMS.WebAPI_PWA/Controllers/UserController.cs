using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
//using System.Web.Http;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using PCS.CPMS.BL.OthersModel;
using PCS.CPMS.WebAPI_PWA.Helpers;
using PCS.CPMS.WebAPI_PWA.Services;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class UserController : Controller
    {
        private readonly tng_cpmsContext _db;
        public IConfiguration _configuration;
        private IUserService _userService;

        public UserController(tng_cpmsContext context, IConfiguration config, IUserService userService)
        {
            _db = context;
            _configuration = config;
            _userService = userService;
        }

        //GET
        [Microsoft.AspNetCore.Mvc.HttpGet("api/User/Index")]
        [ActionName("api/User/Index")]
        public async Task<IActionResult> Index()
        { 
            return View(await _db.User.ToListAsync());
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/User/Login")]
        [ActionName("api/User/Login")]
        //POST: api/btnLogin    
        // [HttpPost]
        [AllowAnonymous]
       // [HttpPost, Route("login")]
        public async Task<ActionResult> Login([FromForm] string useremail, [FromForm] string password)
        {
            if (useremail == null || password == null)
                return Ok(new { Message = "Please fill out the blank field before proceed." });

            var pwd = Encrypt.EncryptFunction(password);
            //db checking

            Common cm_fn = new Common(_db);

            User user_ = await cm_fn.GetUserLogin(useremail, pwd);

            if (user_ != null)
            {
                var token = await _userService.Authenticate(user_);


                Roles roles_ = await _db.Roles.FirstOrDefaultAsync(u => u.Id == token.RoleID);
                token.roleName = roles_.RoleName;
                token.Pwd = string.Empty;
                //var Message = "Success";
                //return Ok(token, Message);

                return Ok(new
                {
                    Message = "Success",
                    Token = token
                   
                }); 
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

        ////POST: api/btnLogin
        //public ActionResult Login([FromForm] string useremail, [FromForm] string password)
        //{
        //    if (useremail == null || password == null)
        //        return Ok(new { Message = "Please fill out the blank field before proceed." });

        //    var pwd = Encrypt.EncryptFunction(password);
        //    //db checking
        //    if (_db.User.Any(x => x.Email.Equals(useremail) && x.Pwd.Equals(pwd)))
        //    {
        //        User user =_db.User.FirstOrDefault(u => u.Email == useremail && u.Pwd == pwd);
        //        Roles role = _db.Roles.FirstOrDefault(x => x.Id == user.RoleId);
        //        return Ok(new
        //        {
        //            Message = "Success",
        //            Name = user.Name,
        //            //Email = user.Email,
        //            //Role = user.RoleID,
        //            RoleName = role.RoleName
        //        }); 
        //    }
        //    else
        //    {
        //        return Ok(new
        //        {
        //            Message = "Invalid email or password. Please try again."
        //        });
        //    }
        //}


        [Microsoft.AspNetCore.Mvc.HttpPost("api/User/CreateUser")]
        [ActionName("api/User/CreateUser")]
        //POST: api/btnRegister
        public IActionResult CreateUser([FromForm] User user)
        {

            if (user.Name == null || user.Email == null || user.Pwd == null)
            {
                return Ok(new { Message = "Please fill out the blank field." });
            }
            else
            {
                //db checking if account existed
                if (_db.User.Any(x => x.Email.Equals(user.Email)))
                {
                    return Ok(new
                    {
                        Message = "Account already exists. Please try another email."
                    });
                }
                else
                {
                    user.Pwd = Encrypt.EncryptFunction(user.Pwd);
                    _db.User.Add(user);
                    _db.SaveChanges();

                    return Ok(new
                    {
                        Message = "Success"
                    });
                }
            }
        }

        //POST: api/btnDeleteUser
      
        [Microsoft.AspNetCore.Mvc.HttpPost("api/User/DeleteUser")]
        [ActionName("api/User/DeleteUser")]
        public IActionResult DeleteUser([FromForm] int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid user id");

            using (_db)
                {
                    var user = _db.User
                        .Where(u => u.Id == id)
                        .FirstOrDefault();

                    _db.Entry(user).State = EntityState.Deleted;
                    _db.SaveChanges();
                }

                return Ok("Successfully delete user");

            }

        //POST: api/btnResetPasssword
        [Microsoft.AspNetCore.Mvc.HttpPost("api/User/ResetPassword")]
        [ActionName("api/User/ResetPassword")]
        public IActionResult ResetPassword([FromForm]User user, [FromForm] string newpassword)
        {
            if (user.Email == null || newpassword == null || user.Pwd == null)
            {
                return Ok(new { Message = "Please fill out the blank field." });
            }
            else
            {
                //db checking email 
                if (_db.User.Any(x => x.Email.Equals(user.Email)))
                {

                    if (!user.Pwd.Equals(newpassword))
                    {
                        return Ok(new { Message = "Your password and confirmation password do not match." });
                    }
                    else
                    {
                        User user1 = _db.User.Where(i => i.Email.Equals(user.Email)).FirstOrDefault();

                        user1.Pwd = Encrypt.EncryptFunction(newpassword);
                        _db.SaveChanges();

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

