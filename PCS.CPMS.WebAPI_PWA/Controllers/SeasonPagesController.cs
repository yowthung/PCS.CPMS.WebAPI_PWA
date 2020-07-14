using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Dynamic;
using System.Net.Http;
using System;
using PCS.CPMS.BL.Models;
using Newtonsoft.Json;
//using System.Web.Http.Cors;
using PCS.CPMS.DAL;
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
    public class SeasonPagesController : Controller
    {
        //add database
        private readonly tng_cpmsContext _db;
        private static readonly HttpClient client = new HttpClient();



    
        public SeasonPagesController(tng_cpmsContext db)
        {
            _db = db;
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/SeasonPages/SeasonPassTransactionTable")]
        [ActionName("api/SeasonPages/SeasonPassTransactionTable")]
        public IActionResult SeasonPassTransactionTable()
        {
            var siteList = new List<int>();
            siteList.Add(4);
            siteList.Add(6);

            dynamic mymodel = new ExpandoObject();
            mymodel.Site = new List<Site>();

            foreach (var item in siteList)
            {
                mymodel.Site.Add(_db.Site.Where(s => s.ID == item).FirstOrDefault());
            }

            return View(mymodel);
        }


    }
}