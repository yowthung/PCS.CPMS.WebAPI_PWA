using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class SeasonsController : Controller
    {
        private readonly tng_cpmsContext _context;

        public SeasonsController(tng_cpmsContext context)
        {
            _context = context;
        }

        [HttpPost("{trans}/{startDate}/{endDate}/{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loaddata")]
        public IActionResult LoadSeasonTransactionData([FromBody]SeasonTransaction trans, DateTime startDate, DateTime endDate, string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
        {
            try
            {
                //var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                //// Skiping number of Rows count  
                //var start = Request.Form["start"].FirstOrDefault();
                //// Paging Length 10,20  
                //var length = Request.Form["length"].FirstOrDefault();
                //// Sort Column Name  
                //var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                //// Sort Column Direction ( asc ,desc)  
                //var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                //// Search Value from (Search box)  
                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var transactionData = (from tempT in _context.SeasonTransaction
                             join tempS in _context.Site on tempT.SiteId equals tempS.Id into table1
                             join tempSC in _context.SeasonCard on tempT.SeasonCardId equals tempSC.Id into table2
                             join tempSI in _context.SeasonInvoice on tempT.InvoiceId equals tempSI.Id into table3
                             join tempSU in _context.SeasonUser on tempT.SeasonUserId equals tempSU.Id into table4
                             join tempSR in _context.SeasonRate on tempT.SeasonRateId equals tempSR.Id into table5
                             from y1 in table1.DefaultIfEmpty()
                             from y2 in table2.DefaultIfEmpty()
                             from y3 in table3.DefaultIfEmpty()
                             from y4 in table4.DefaultIfEmpty()
                             from y5 in table5.DefaultIfEmpty()
                             where (tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime <= endDate)
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
                    transactionData = transactionData.OrderBy(x => sortColumn + " " + sortColumnDirection);
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