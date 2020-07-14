using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class PayoutsController : Controller
    {

        private readonly tng_cpmsContext _context;
        [Obsolete]
        private readonly IHostingEnvironment hostingenvironment;
        public PayoutsController(tng_cpmsContext context)
        {
            _context = context;
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("create")]
        [ActionName("create")]
        [Obsolete]
        public ActionResult Create([FromForm]Payout payout, [FromForm] IFormFile receipt, [FromForm] IFormFile settlement, [FromForm] string batchlist)
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

            //var path1 = Path.Combine(
            //      hostingenvironment.WebRootPath, "receipts",
            //      receipt.FileName);

     
            //Directory.CreateDirectory(Path.GetDirectoryName(path1));

            //var path2 = Path.Combine(hostingenvironment.WebRootPath, "settlements", settlement.FileName);

            //Directory.CreateDirectory(Path.GetDirectoryName(path2));

            var filesPath = Directory.GetCurrentDirectory() + "/files";
            if (!System.IO.Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }
            var fileName = Path.GetFileName(receipt.FileName);
            var path1 = Path.Combine(filesPath, fileName);

            //var path2 = Path.Combine(
            //      hostingenvironment.WebRootPath, "settlements",
            //      settlement.FileName);
             fileName = Path.GetFileName(receipt.FileName);
            var path2 = Path.Combine(filesPath, fileName);

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
                payout.ReferenceNo = "REF-" + payout.CreatedDate?.ToString("yyyyMMdd")+"-"+ batch.BatchId;

            }

            if (_context.Payout.Any(p => p.ReferenceNo == payout.ReferenceNo))
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
                _context.Payout.Add(payout);

                foreach (BatchCreationLog batch in blist)
                {
                    var update_batch = _context.BatchCreationLog.Where(x => x.BatchId == batch.BatchId).FirstOrDefault();
                    update_batch.PayoutStatus = true;
                    update_batch.ReferenceNo = payout.ReferenceNo;

                }

                _context.SaveChanges();

                return Ok(new
                {
                    Message = "Success"
                });
            }
        }

        [HttpGet("{referenceNo}"), Route("viewpayout")]
        public IActionResult ViewPayout(string referenceNo)
        {
            dynamic model = new ExpandoObject();
            model.Batches = _context.BatchCreationLog.Where(b => b.ReferenceNo == referenceNo).ToList();
            model.Payout = _context.Payout.Where(p => p.ReferenceNo == referenceNo).FirstOrDefault();
            var host = Dns.GetHostName();
            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            model.ReceiptPath = baseUrl + "/receipts/" + model.Payout.Receipt;
            model.SettlementPath = baseUrl + "/settlements/" + model.Payout.Settlement;
            return PartialView("_ViewPayoutModal", model);
        }
    }

    }