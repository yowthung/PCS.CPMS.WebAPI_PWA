using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Linq.Dynamic.Core;
using PCS.CPMS.BL.Models;
//using System.Web.Http.Cors;
using Microsoft.AspNetCore.Cors;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using System.Web.Http;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{

    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class PagesController : Controller
    {

        //public IActionResult Login()
        //{
        //    return View();
        //}

        //public IActionResult Register()
        //{
        //    return View();
        //}

        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //public IActionResult ViewSiteDetails()
        //{
        //    return View();
        //}
    }
}