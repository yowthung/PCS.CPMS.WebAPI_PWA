using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using PCS.CPMS.DAL;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System;
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
    public class PayoutsController : Controller
    {
        private readonly tng_cpmsContext _db;
        private readonly IHostingEnvironment hostingenvironment;

        public PayoutsController(tng_cpmsContext db, IHostingEnvironment _hostenvironment)
        {
            _db = db;
            hostingenvironment = _hostenvironment;
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Payouts/Create")]
        [ActionName("api/Payouts/Create")]
        public ActionResult Create([FromForm]Payout payout, [FromForm] IFormFile receipt, [FromForm] IFormFile settlement, [FromForm] string batchlist)
        {
            try
            {
                if (receipt == null || receipt.Length == 0)
                    return Ok(new
                    {
                        Message = "Please upload receipt."
                    });

                if (settlement == null || settlement.Length == 0)
                    return Ok(new
                    {
                        Message = "Please upload settlement."
                    });

                var path1 = Path.Combine(hostingenvironment.WebRootPath, "receipts", receipt.FileName);

                var path2 = Path.Combine(hostingenvironment.WebRootPath, "settlements", settlement.FileName);


                using (var stream = new FileStream(path1, FileMode.Create))
                {
                    receipt.CopyTo(stream);
                }
                using (var stream2 = new FileStream(path2, FileMode.Create))
                {
                    settlement.CopyTo(stream2);
                }

                var blist = JsonConvert.DeserializeObject<List<BatchCreationLog>>(batchlist);

                foreach (BatchCreationLog batch in blist)
                {
                    payout.ReferenceNo = "REF-" + payout.CreatedDate?.ToString("yyyyMMdd") + "-" + batch.BatchId;

                }

                if (_db.Payout.Any(p => p.ReferenceNo == payout.ReferenceNo))
                {

                    return Ok(new
                    {
                        Message = "Current payout existed.Please try again"
                    });
                }
                else
                {

                    payout.Receipt = receipt.FileName;
                    payout.Settlement = settlement.FileName;
                    _db.Payout.Add(payout);

                    foreach (BatchCreationLog batch in blist)
                    {
                        var update_batch = _db.BatchCreationLog.Where(x => x.BatchId == batch.BatchId).FirstOrDefault();
                        update_batch.PayoutStatus = true;
                        update_batch.ReferenceNo = payout.ReferenceNo;

                    }

                    _db.SaveChanges();

                    return Ok(new
                    {
                        Message = "Success"
                    });
                }
            }
            catch {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Payouts/ViewPayout")]
        [ActionName("api/Payouts/ViewPayout")]
        public IActionResult ViewPayout([FromForm]string referenceNo)
        {
            dynamic model = new ExpandoObject();
            model.Batches = _db.BatchCreationLog.Where(b => b.ReferenceNo == referenceNo).ToList();
            model.Payout = _db.Payout.Where(p => p.ReferenceNo == referenceNo).FirstOrDefault();
            var host = Dns.GetHostName();
            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            model.ReceiptPath = baseUrl + "/receipts/" + model.Payout.Receipt;
            model.SettlementPath = baseUrl + "/settlements/" + model.Payout.Settlement;
            return PartialView("_ViewPayoutModal", model);
        }
    }

    }