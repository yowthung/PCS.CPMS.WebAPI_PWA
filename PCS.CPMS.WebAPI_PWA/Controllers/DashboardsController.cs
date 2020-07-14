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
    public class DashboardsController : Controller
    {
        //add database
        private readonly tng_cpmsContext _db;

        public DashboardsController(tng_cpmsContext db)
        {
            _db = db;
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("api/Dashboards/Dashboard")]
        [ActionName("api/Dashboards/Dashboard")]
        //generate Dashboard page fixed details
        public IActionResult Dashboard()
        {

            var siteList = new List<int>();
            siteList.Add(6);
            siteList.Add(4);

            var pending_batches = 0;
            var completed_batches = 0;
            var total_site = 0;

            //total pending & completed amount is only for tng batches
            long? total_pending_amount = 0;
            long? total_completed_amount = 0;
        
            double? new_total_pending_amount = 0;
            double? new_total_completed_amount = 0;

            int? total_entry_lane = 0;
            int? total_exit_lane = 0;
            int? total_validator = 0;

            dynamic mymodel = new ExpandoObject();
            mymodel.Site = new List<Site>();

            foreach (var item in siteList)
            {
                pending_batches += _db.BatchCreationLog.Where(b => b.BatchType == "FN" && b.PayoutStatus == null && b.SiteId == item).Select(b => b.BatchId).Count();
                completed_batches += _db.BatchCreationLog.Where(b => b.BatchType == "FN" && b.PayoutStatus == true && b.SiteId == item).Select(b => b.BatchId).Count();

                total_site += 1;

                total_entry_lane += _db.Site.Where(s => s.ID == item).Select(s => s.TotalEntryLane).Sum();
                total_exit_lane += _db.Site.Where(s => s.ID == item).Select(s => s.TotalExitLane).Sum();
                total_validator += _db.Site.Where(s => s.ID == item).Select(s => s.TotalValidator).Sum();

                total_pending_amount = _db.BatchCreationLog.Where(b => b.BatchType == "FN" && b.PayoutStatus == null && b.SiteId == item).Select(b => b.TotalDebit).Sum();
                total_completed_amount = _db.BatchCreationLog.Where(b => b.BatchType == "FN" && b.PayoutStatus == true && b.SiteId == item).Select(b => b.TotalDebit).Sum();

                var mdr = _db.Site.Where(s => s.ID == item).Select(s => s.Mdr).FirstOrDefault();

                new_total_pending_amount += (total_pending_amount / 100) * ((100 - mdr) / 100);
                new_total_completed_amount += (total_completed_amount / 100) * ((100 - mdr) / 100);

                mymodel.Site.Add(_db.Site.Where(s => s.ID == item).FirstOrDefault());
            }

            mymodel.SiteIdList = siteList;
            mymodel.PendingBatch = pending_batches;
            mymodel.CompletedBatch = completed_batches;
            mymodel.TotalSite = total_site;
            mymodel.TotalEntryLane = total_entry_lane;
            mymodel.TotalExitLane = total_exit_lane;
            mymodel.TotalValidator = total_validator;
            mymodel.TotalPendingAmount = new_total_pending_amount?.ToString("C");
            mymodel.TotalCompletedAmount = new_total_completed_amount?.ToString("C");

            return new  JsonResult(mymodel);
        }

    }
}