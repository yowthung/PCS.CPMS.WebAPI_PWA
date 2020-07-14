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
    public class OthersController : Controller
    {
        //add database
        private readonly tng_cpmsContext _db;
        private static readonly HttpClient client = new HttpClient();

        public OthersController(tng_cpmsContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Payouts/Site")]
        [ActionName("api/Payouts/Site")]
        public IActionResult Site()
        {

            var siteList = new List<int>();
            siteList.Add(4);
            siteList.Add(6);

            dynamic mymodel = new ExpandoObject();
            mymodel.Site = new List<Site>();

            foreach (var item in siteList) {
                mymodel.Site.Add(_db.Site.Where(s => s.ID == item).FirstOrDefault());
            }

            return View(mymodel);
        }
    }
}