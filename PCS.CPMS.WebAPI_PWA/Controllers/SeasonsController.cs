using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.BL.Models;
//using System.Web.Http;
using System.Collections.Generic;
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
    public class SeasonsController : Controller
    {
        private readonly tng_cpmsContext _db;

        public SeasonsController(tng_cpmsContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Seasons/apiLoadSeasonTransactionData")]
        [ActionName("api/Seasons/LoadSeasonTransactionData")]
        public IActionResult LoadSeasonTransactionData([FromForm]SeasonTransaction trans, [FromForm] DateTime startDate, [FromForm] DateTime endDate)
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var transactionData = (from tempT in _db.SeasonTransaction
                             join tempS in _db.Site on tempT.SiteId equals tempS.Id into table1
                             join tempSC in _db.SeasonCard on tempT.SeasonCardId equals tempSC.Id into table2
                             join tempSI in _db.SeasonInvoice on tempT.InvoiceId equals tempSI.Id into table3
                             join tempSU in _db.SeasonUser on tempT.SeasonUserId equals tempSU.Id into table4
                             join tempSR in _db.SeasonRate on tempT.SeasonRateId equals tempSR.Id into table5
                             from y1 in table1.DefaultIfEmpty()
                             from y2 in table2.DefaultIfEmpty()
                             from y3 in table3.DefaultIfEmpty()
                             from y4 in table4.DefaultIfEmpty()
                             from y5 in table5.DefaultIfEmpty()
                             where (tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1) && tempT.Status == 1)
                             select new
                             {
                                 SiteName = y1.Name,
                                 y1.LocationId,
                                 y2.CardNo,
                                 y3.InvoiceNo,
                                 UserName = y4.Name,
                                 y5.SeasonRateName,
                                 Price = y5.Price.ToString("C"),
                                 tempT.Id,
                                 tempT.TransactionTime,
                                 TransactionAmount = tempT.TransactionAmount.GetValueOrDefault().ToString("C"),
                                 tempT.PaymentGateway,
                                 tempT.MerchantCode                     
                             });



                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    transactionData = transactionData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    transactionData = transactionData.Where(m => m.CardNo.IndexOf(searchValue) > -1 || m.MerchantCode.IndexOf(searchValue) > -1 || m.InvoiceNo.IndexOf(searchValue) > -1|| m.UserName.IndexOf(searchValue) > -1);
                }



                //total number of rows count   
                recordsTotal = transactionData.Count();
                //Paging   
                var data = transactionData.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}