using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
//using System.Web.Http;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using DinkToPdf;
using DinkToPdf.Contracts;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Serializable]
    public partial class trans
    {
        // public string Draw { get; set; }
        public string BatchNo { get; set; }
        public string Spid { get; set; }
        public int? SiteId { get; set; }
        public string EXITLOCATION { get; set; }
        public string EntryLane { get; set; }
        public string ExitLane { get; set; }
        //   public DateTime? STARTDATE { get; set; }
        //   public DateTime? ENDDATE { get; set; }
        public trans()
        {



        }


    }

  

    // [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    // [EnableCors(origins: "localhost:8081", headers: "*", methods: "*")]
    public class TransactionsController : ControllerBase
    {
        private readonly tng_cpmsContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConverter _converter;
  
        public TransactionsController(tng_cpmsContext context, IHttpContextAccessor httpContextAccessor, IConverter converter)
        {
            _db = context;
            _httpContext = httpContextAccessor;
            _httpContext.HttpContext.Request.EnableBuffering();
            _converter = converter;
        }


        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("loaddata")]
        [ActionName("loaddata")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //POST: All Transactions (JSON) from table configuration    
        public async Task<IActionResult> LoadData([FromForm] trans trans, [FromForm] DateTime startDate, [FromForm] DateTime endDate)
        {
            try
            { 
                //validate object trans
                if (trans != null)
                {
                    if (trans.EXITLOCATION == null)
                    {
                        trans.EXITLOCATION = string.Empty;
                    }
                    if (trans.BatchNo == null)
                    {
                        trans.BatchNo = string.Empty;
                    }

                }
            
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

                //// Getting all Customer data  
                //var transactionData = (from tempTransaction in _db.Transaction where tempTransaction.SuccessFlag == "Y" select tempTransaction);
                IQueryable<Transaction> model;

                //batchno exists
                if (trans.BatchNo != null)
                {
                    model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                    //if (batch.Spid.Equals("ALL"))
                    //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    if (trans.EXITLOCATION.Equals("ALL"))
                    {
                        if (trans.EntryLane.Equals("ALL"))
                        {
                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                        else
                        {
                            model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                    }
                    else
                    {
                        model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                        if (trans.EntryLane.Equals("ALL"))
                        {
                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                        else
                        {
                            model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                    }
                }

                //if batchno = null
                else
                {
                    model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);


                    if (trans.EXITLOCATION.Equals("ALL"))
                    {
                        if (trans.EntryLane.Equals("ALL"))
                        {
                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                        else
                        {
                            model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                    }
                    else
                    {
                        model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                        if (trans.EntryLane.Equals("ALL"))
                        {
                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                        else
                        {
                            model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                            if (trans.ExitLane.Equals("ALL"))
                            { }
                            else
                            {
                                model = (from m in _db.Transaction where (m.SPID == trans.Spid && m.ExitLocation == trans.EXITLOCATION && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                            }
                        }
                    }
                }



                //}
                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    model = model.OrderBy(x => sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    model = model.Where(m => m.TransactionId.ToString().IndexOf(searchValue) > -1 || m.BatchNo.IndexOf(searchValue) > -1);
                }

                //total number of rows count   
                recordsTotal = model.Count();
                //Paging   
                var data = model.Skip(skip).Take(pageSize).ToList();

                foreach (var item in data)
                {
                    if (item.ExitLocation == "" || item.ExitLocation == "$?I" || item.ExitLocation == "1\u0090U") { }
                    else
                    {
                        var site = _db.Site.Where(s => s.LocationId == item.ExitLocation).FirstOrDefault();
                        item.ExitLocation = item.ExitLocation + " (" + site.Name + ")";
                    }


                }
                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("loadviewtransactiondata")]
        [ActionName("loadviewtransactiondata")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //POST: All Transactions (JSON) from table configuration   
        public IActionResult LoadViewTransactionData(string batchno, string locationID, string spid, string terminalID)
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

                var batch_no = "FN" + String.Format("{0:D5}", Convert.ToInt32(batchno));

                // Getting all Customer data  
                var transactionData = (from tempTransaction in _db.Transaction where tempTransaction.BatchNo == batch_no && tempTransaction.SuccessFlag == "Y" && tempTransaction.CardTypeTag == "T" && tempTransaction.SPID == spid && tempTransaction.ExitLocation == locationID && tempTransaction.ExitLane == terminalID select tempTransaction);

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    transactionData = transactionData.OrderBy(x => sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    transactionData = transactionData.Where(m => m.TransactionId.ToString().IndexOf(searchValue) > -1 || m.BatchNo.IndexOf(searchValue) > -1);
                }

                //total number of rows count   
                recordsTotal = transactionData.Count();
                //Paging   
                var data = transactionData.Skip(skip).Take(pageSize).ToList();

                foreach (var item in data)
                {
                    var site = _db.Site.Where(s => s.LocationId == item.ExitLocation).FirstOrDefault();
                    item.ExitLocation = item.ExitLocation + " (" + site.Name + ")";
                }

                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, batch_no = batch_no });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("gettransactionamount")]
        [ActionName("gettransactionamount")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult GetTransactionAmount(string batchlist)
        {
            Dictionary<int, string> Trans = new Dictionary<int, string>();
            //List<int> totalprice = new List<int>();

            var blist = JsonConvert.DeserializeObject<IEnumerable<BatchCreationLog>>(batchlist);
            var price = 0;
            var subtotal = "";
            var batchid = 0;
            //var location = "";
            var siteid = 0;
            //var sitename = ""; 

            foreach (BatchCreationLog batch in blist)
            {
                var batch_no = "FN" + batch.BatchNumber.PadLeft(5, '0');
                var sum = _db.Transaction.
                    Where(t => t.BatchNo == batch_no && t.SiteId == batch.SiteId && t.SPID == batch.Spid && t.ExitLane == batch.TerminalId && t.TransactionAmount > 0).Select(t => t.TransactionAmount).Sum();

                Trans.Add(batch.BatchId, (sum * 0.01)?.ToString("C"));

                price += Convert.ToInt32(sum * 0.01);
                batchid = batch.BatchId;
                //location = batch.LocationID;
                siteid = batch.SiteId;
                //sitename = _db.Site.Where(s => s.Name == batch.LocationID).Select(s => s.Name).FirstOrDefault();
            }

            var sitembr = 100 - (_db.Site.Where(s => s.Id == siteid).Select(s => s.Mdr).FirstOrDefault());

            var total = ((decimal)(price * sitembr / 100)).ToString("C");
            DateTime datetime = DateTime.Now;

            //var refno = "REF-"+ datetime.ToString("yyyyMMdd");

            subtotal = price.ToString("C");
            var finaltotal = (decimal)(price * sitembr / 100);

            //get site details 
            var sitedetails = _db.Site.Where(s => s.Id == siteid).FirstOrDefault();

            return new JsonResult(new { Trans, price, subtotal, total, datetime, batchid, finaltotal, sitedetails });
        }


        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("calculatetransactionfordaymonthyear")]
        [ActionName("calculatetransactionfordaymonthyear")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //api for main dasboard upper part : total trans for daily, thismonth, and thisyear
        public IActionResult CalculateTransactionForDayMonthYear(int dayinput, int monthinput, int yearinput)
        {

            // Getting transaction number for specific day, month, year
            var recordsDay = _db.Transaction.Where(m => m.TransactionTime.Value.Day == dayinput && m.TransactionTime.Value.Month == monthinput && m.TransactionTime.Value.Year == yearinput && m.SuccessFlag == "Y").Count();
            var recordsMonth = _db.Transaction.Where(m => m.TransactionTime.Value.Month == monthinput && m.TransactionTime.Value.Year == yearinput && m.SuccessFlag == "Y").Count();
            var recordsYear = _db.Transaction.Where(m => m.TransactionTime.Value.Year == yearinput && m.SuccessFlag == "Y").Count();

            var model = _db.Transaction.Where(m => m.TransactionTime.Value.Month == monthinput && m.TransactionTime.Value.Year == yearinput && m.SuccessFlag == "Y").ToList();

            return new JsonResult(new { recordsDay = recordsDay, recordsMonth = recordsMonth, recordsYear = recordsYear, data = model });
        }

        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("calculatetransactionformonth")]
        [ActionName("calculatetransactionformonth")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //api for main dashboard: main graph 
        public IActionResult CalculateTransactionForMonth(int monthinput, int yearinput)
        {

            // Getting transaction list for specific month (for graph)
            var eachday = new List<DateTime>();

            for (int i = 1; i <= DateTime.DaysInMonth(yearinput, monthinput); i++)
            {
                eachday.Add(new DateTime(yearinput, monthinput, i)); //map each day for specific month
            }

            var results = _db.Transaction
              .Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput && x.SuccessFlag == "Y")
              .GroupBy(x => x.TransactionTime.Value.Date)
              .Select(x => new
              {
                      //  Count = x.Select(p=>p.TransactionAmount).Sum(),
                      Count = x.Sum(p => p.TransactionAmount),

                  Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                  })
              .ToList();

            Dictionary<string, decimal> dataDict = new Dictionary<string, decimal>();
            foreach (var dt in eachday)
            {
                decimal count = 0;

                foreach (var item in results)
                {
                    if (dt == item.Date)
                    {
                        count = Convert.ToDecimal((item.Count) / 100);
                        break;
                    }
                }

                dataDict.Add(dt.ToString("yyyy-MM-dd"), count);

            }

            //return Json(dataDict);
            return new JsonResult(new { dataDict = dataDict });

        }

        //public IActionResult CalculateTransactionForYear(int yearinput)
        //{

        //    // Getting transaction list for specific year (for graph)
        //    var results = _db.Transaction
        //      .Where(x => x.TransactionTime.Value.Year == yearinput && x.SuccessFlag == "Y")
        //      .GroupBy(x => x.TransactionTime.Value.Month)
        //      .Select(x => new
        //      {
        //          Count = x.Count(),
        //          Month = x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
        //      })
        //      .ToList();

        //    return Json(new { results = results});

        //}




        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("getinvoiceprinting")]
        [ActionName("getinvoiceprinting")]
        //   [Consumes("application/json")]
        //[Consumes("application/x-www-form-urlencoded")]
        //api for main dashboard: main graph 
        public async Task<ActionResult> GetInvoicePrinting([FromForm]string batchlist)
        {

           

            var obj = new UserListPdfExporter(_db, _converter);
            //  obj.ConvertUserListToHtmlTable(Trans, price, subtotal, total, datetime, batchid, finaltotal, sitedetails);
            var html =   await obj.ConvertUserListToHtmlTable(batchlist);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = html
                    }
                }
            };
             
           // var file = new FileDto("UserList.pdf", _converter.Convert(doc));

            byte[] pdf = _converter.Convert(doc);
            using (FileStream stream = new FileStream(@"Files\" + DateTime.UtcNow.Ticks.ToString() + ".pdf", FileMode.Create))
            {
                stream.Write(pdf, 0, pdf.Length);
            }
            return new FileContentResult(pdf, "application/pdf");
            //  HttpContext.Response.ContentType = "application/pdf";

            //string filename = @"report.pdf";
            //HttpContext.Response.Headers.Add("x-filename", file.FileName);
            //HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            //HttpContext.Response.Body.Write(file.FileBytes);
            //return new ContentResult();
           // return  File(file.FileBytes, "application/pdf", file.FileName);
        }


















        }



    }








