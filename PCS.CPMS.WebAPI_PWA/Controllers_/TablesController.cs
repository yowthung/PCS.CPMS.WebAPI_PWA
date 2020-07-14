using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Dynamic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class TablesController : ControllerBase
    {

        private readonly tng_cpmsContext _context;

        public TablesController(tng_cpmsContext context)
        {
            _context = context;
        }

        [HttpGet, Route("batchtable")]
        public IActionResult BatchTable()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.BatchNumber = new List<String>();
            mymodel.LocationID = new List<String>();
            mymodel.TerminalID = new List<String>();
            mymodel.SPID = new List<String>();
            mymodel.Site = new List<Site>();
            //List<BatchCreationLog> batchlist = new List<BatchCreationLog>();

            mymodel.BatchNumber = _context.BatchCreationLog.Where(p => p.BatchType == "FN").OrderBy(p => int.Parse(p.BatchNumber)).Select(p => p.BatchNumber).Distinct().ToList();
            //var location = _context.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.LocationID).Distinct().ToList();
            mymodel.TerminalID = _context.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.TerminalId).Distinct().ToList();
            mymodel.SPID = _context.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.Spid).Distinct().ToList();

            //var siteid = _context.BatchCreationLog.Where(p => p.BatchType == "FN").Select(p => p.SiteID).Distinct().ToList();
            //var sitename = new List<String>();
            //var location1 = new List<String>();

            //foreach (var item in siteid)
            //{
                //if (item == "" || item == "$?I" || item == "1\u0090U") { }
                //else
                //{
                //    var site = _context.Site.Where(s => s.LocationID == item).FirstOrDefault();
                //    location1.Add(item + " (" + site.Name + ")");
                //}
                mymodel.Site = _context.Site.OrderBy(s=> s.LocationId).ToList();
            //}

            //mymodel.LocationID = location1;
            return new JsonResult(mymodel);
        }

        //transaction table 
        [HttpGet, Route("transactiontable")]
        //transaction table 
        public async Task<ActionResult> TransactionTable([FromForm] int roleid)
        {

            dynamic mymodel = new ExpandoObject();
            mymodel.BatchNumber = new List<String>();
            mymodel.SPID = new List<String>();
            mymodel.Location = new List<String>();
            mymodel.EntryLane = new List<String>();
            mymodel.ExitLane = new List<String>();
            //List<BatchCreationLog> batchlist = new List<BatchCreationLog>();
            var allow_site = _context.RolesAccess.OrderBy(s => s.RoleId == roleid).ToList();
            mymodel.BatchNo = _context.Transaction.Where(p => p.SuccessFlag == "Y").OrderBy(p => p.BatchNo).Select(p => p.BatchNo).Distinct().ToList();
            mymodel.SPID = _context.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.SPID).Distinct().ToList();
            var location = _context.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.ExitLocation).Distinct().ToList();
            mymodel.EntryLane = _context.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.EntryLane).Distinct().ToList();
            mymodel.ExitLane = _context.Transaction.Where(p => p.SuccessFlag == "Y").Select(p => p.ExitLane).Distinct().ToList();

            //var location1 = new List<String>();

            //foreach (var item in location)
            //{
            //    if (item == "" || item == "$?I" || item == "1\u0090U") { }
            //    else
            //    {
            //        var site = _context.Site.Where(s => s.LocationID == item).FirstOrDefault();
            //        location1.Add(item + " (" + site.Name + ")");
            //    }
            //}

            //mymodel.Location = location1;
            mymodel.Site = _context.Site.OrderBy(s => s.LocationId).ToList();
            return new JsonResult(mymodel);
        }

        [HttpGet, Route("visamastertransactiontable")]
        public IActionResult VisaMasterTransactionTable()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.SPID = _context.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount >0).Select(p => p.SPID).Distinct().ToList();           
            mymodel.EntryLane = _context.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount > 0).Select(p => p.EntryLane).Distinct().ToList();
            mymodel.ExitLane = _context.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount > 0).Select(p => p.ExitLane).Distinct().ToList();
            mymodel.Site = _context.Site.OrderBy(s => s.LocationId).ToList();
            return new JsonResult(mymodel);
        }

        [HttpGet, Route("seasonpasstransactiontable")]
        public IActionResult SeasonPassTransactionTable()
        {
            dynamic mymodel = new ExpandoObject();      
            mymodel.Site = _context.Site.OrderBy(s => s.LocationId).ToList();
            return new JsonResult(mymodel);
        }

        [HttpGet, Route("salessummary")]
        public IActionResult SalesSummary()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Site = _context.Site.OrderBy(s => s.LocationId).ToList();
            return new JsonResult(mymodel);
        }

        //public IActionResult ViewTransaction()
        //{
        //    return View();
        //}


        //get contact list
        //public IActionResult DataTables()
        //{ 
        //    return View();
        //}

    }
}