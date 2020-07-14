using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System;
using System.Dynamic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using PCS.CPMS.BL.Models;
//using System.Web.Http;
using Newtonsoft.Json;
//using System.Web.Http.Cors;
using PCS.CPMS.DAL;
using Microsoft.AspNetCore.Cors;
//using System.Web.Http;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using System.Web.Http;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{

    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TngPagesController : Controller
    {
        //add database
        private readonly tng_cpmsContext _db;
        private static readonly HttpClient client = new HttpClient();

        public TngPagesController(tng_cpmsContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/TngPages/BatchTable")]
        [ActionName("api/TngPages/BatchTable")]
        public IActionResult BatchTable([FromForm]int roleID)
        {           
            var siteList = new List<int>();
            siteList.Add(4);
            siteList.Add(6);

            dynamic mymodel = new ExpandoObject();
            mymodel.TerminalID = new List<String>();
            mymodel.SPID = new List<String>();
            mymodel.Site = new List<Site>();

            mymodel.TerminalID = _db.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.TerminalId).Distinct().ToList();
            mymodel.SPID = _db.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.Spid).Distinct().ToList();

            foreach (var item in siteList)
            {
                mymodel.Site.Add(_db.Site.Where(s => s.ID == item).FirstOrDefault());
            }

            return View(mymodel);
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/TngPages/TngTransactionTable")]
        [ActionName("api/TngPages/TngTransactionTable")]
        public IActionResult TngTransactionTable([FromForm]int roleID)
        {

            var siteList = new List<int>();
            siteList.Add(4);
            siteList.Add(6);

            //using (var httpClient = new HttpClient())
            //{

            //    using (var response = await httpClient.GetAsync("http://192.168.0.176:5555/api/Tables/transactiontable"))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        dynamic lol = Newtonsoft.Json.Linq.JObject.Parse(apiResponse);

            //        mymodel.BatchNumber = lol.BatchNumber.ToObject<List<String>>();
            //        mymodel.SPID = lol.SPID.ToObject<List<String>>();
            //        mymodel.Location = lol.Location.ToObject<List<String>>();
            //        mymodel.EntryLane = lol.EntryLane.ToObject<List<String>>();
            //        mymodel.ExitLane = lol.ExitLane.ToObject<List<String>>();
            //        mymodel.Site = lol.Site.ToObject<List<Site>>();
            //    }
            //}
            dynamic mymodel = new ExpandoObject();
            mymodel.BatchNumber = new List<String>();
            mymodel.SPID = new List<String>();
            mymodel.Location = new List<String>();
            mymodel.EntryLane = new List<String>();
            mymodel.ExitLane = new List<String>();
            mymodel.Site = new List<Site>();


            List<BatchCreationLog> batchlist = new List<BatchCreationLog>();

            mymodel.BatchNo = _db.Transaction.Where(p => p.SuccessFlag == "Y").OrderBy(p => p.BatchNo).Select(p => p.BatchNo).Distinct().ToList();
            mymodel.SPID = _db.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.Spid).Distinct().ToList();
            var location = _db.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.ExitLocation).Distinct().ToList();
            mymodel.EntryLane = _db.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.EntryLane).Distinct().ToList();
            mymodel.ExitLane = _db.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.ExitLane).Distinct().ToList();

            foreach (var item in siteList)
            {
                mymodel.Site.Add(_db.Site.Where(s=> s.ID == item).FirstOrDefault());
            }

            return View(mymodel);
        }

        //public IActionResult ViewTngTransaction()
        //{
        //    return View();
        //}

    }
}