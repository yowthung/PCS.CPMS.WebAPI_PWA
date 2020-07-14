using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;
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
    public class SitesController : Controller
    {

        private readonly tng_cpmsContext _context;

        public SitesController(tng_cpmsContext context)
        {
            _context = context;
        }

        [HttpGet("{site}/{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loaddata")]
        //POST: All sites (JSON) from table configuration
        public IActionResult LoadData(Site site, string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
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

                //Getting data 
                IQueryable<Site> model;

                //if (site.Name != null)
                //{
                model = (from m in _context.Site where (m.Id == site.Id) select m);

                //}
                //else
                //{
                //    model = (from m in _context.Site select m);
                //}



                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    model = model.OrderBy(x => sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    model = model.Where(m => m.Name.ToString().IndexOf(searchValue) > -1 || m.LocationId.IndexOf(searchValue) > -1);
                }

                //total number of rows count   
                recordsTotal = model.Count();
                //Paging   
                var data = model.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, Route("deletesite")]
        public IActionResult DeleteSite([FromBody] Site site)
        {
            var selectedsite = _context.Site.Where(s => s.Id == site.Id).FirstOrDefault();

            _context.Site.Remove(selectedsite);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost, Route("editsite")]
        public IActionResult EditSite([FromBody] Site site)
        {
            var selectedsite = _context.Site.Where(s => s.Id == site.Id).FirstOrDefault();

            //_context.Site.Remove(selectedsite);
            //_context.SaveChanges();

            return PartialView("_EditSiteModal", selectedsite);
        }

        [HttpPost, Route("saveeditchanges")]
        public IActionResult SaveEditChanges(Site site)
        {
            var selectedsite = _context.Site.Where(s => s.Id == site.Id).FirstOrDefault();

            selectedsite.Name = site.Name;
            selectedsite.LocationId = site.LocationId;
            selectedsite.Tngrate = site.Tngrate;
            selectedsite.Mbr = site.Mbr;
            selectedsite.Tngrate = site.Tngrate;
            selectedsite.TngrateMargin = site.TngrateMargin;
            selectedsite.TotalEntryLane = site.TotalEntryLane;
            selectedsite.TotalExitLane = site.TotalExitLane;
            selectedsite.ContractYear = site.ContractYear;
            selectedsite.SiteContactPersonName = site.SiteContactPersonName;
            selectedsite.SiteContactPersonTelNo = site.SiteContactPersonTelNo;
            selectedsite.SiteContactPersonEmail = site.SiteContactPersonEmail;
            selectedsite.MonthlyLaneCharge = site.MonthlyLaneCharge;
            selectedsite.SettlementDay = site.SettlementDay;
            selectedsite.CompanyBankName = site.CompanyBankName;
            selectedsite.CompanyBankAccountNo = site.CompanyBankAccountNo;
            selectedsite.CompanyBankAccountName = site.CompanyBankAccountName;
            selectedsite.SiteBankName = site.SiteBankName;
            selectedsite.SiteBankAccountNo = site.SiteBankAccountNo;
            selectedsite.SiteBankAccountName = site.SiteBankAccountName;
            selectedsite.LastUpdatedTime = site.LastUpdatedTime;
            selectedsite.LastUpdatedBy = site.LastUpdatedBy;

            //if (_context.Site.Any(x => x.LocationId.Equals(selectedsite.LocationId) && x.Name.Equals(selectedsite.Name)))
            //{
            //    return Ok(new
            //    {
            //        Message = "Fail"
            //    });
            //}
            //else{
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Success"
            });
            //}
        }

        //public IActionResult AddSite()
        //{

        //    return PartialView("_AddSiteModal");
        //}
        [HttpPost, Route("saveaddchanges")]
        public IActionResult SaveAddChanges(Site site)
        {
            //db checking if account existed
            if (_context.Site.Any(x => x.LocationId.Equals(site.LocationId) && x.Name.Equals(site.Name)))
            {
                return Ok(new
                {
                    Message = "Fail"
                });
            }
            else
            {
                _context.Site.Add(site);
                _context.SaveChanges();

                return Ok(new
                {
                    Message = "Success"
                });
            }

        }

        [HttpGet("{startDate}/{endDate}/{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loaddataindashboard")]
        public IActionResult LoadDataInDashboard(DateTime startDate, DateTime endDate, string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
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

                // Getting all site data  
                //var data = (from tempSite in _context.Site select tempSite, from tempTrans in _context.Transaction select tempTrans);

                var site = _context.Site.OrderBy(s => s.LocationId).ToList();
                List<DataResponse> listDataResp = new List<DataResponse>();
                foreach (var s in site)
                {
                    DataResponse dr = new DataResponse();
                    dr.SiteID = s.Id ;
                    dr.LocationID = s.LocationId;
                    dr.SiteName = s.Name;
                    dr.MBR= s.Mbr;
                    dr.Total = _context.Transaction.Where(t => t.ExitLocation == s.LocationId && t.SuccessFlag=="Y" && t.BatchNo !="" && t.TransactionTime >= startDate && t.TransactionTime <= endDate).Select(t => t.TransactionId).Count();
                    listDataResp.Add(dr);
                }
                ////Search  
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    siteData = siteData.Where(m => m.Name.ToString().IndexOf(searchValue) > -1 || m.LocationId.IndexOf(searchValue) > -1);
                //}

                //total number of rows count   
                recordsTotal = listDataResp.Count();
                ////Paging   
                //var data = siteData.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listDataResp });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet,Route("searchsite")]
        public IActionResult SearchSite(Site site)
        {
            IQueryable<Site> model;

            if (site.Name != null)
            {
                model = (from m in _context.Site where (m.Name == site.Name) select m);

                if(site.LocationId != null)
                {
                    model = (from m in _context.Site where (m.Name == site.Name && m.LocationId == site.LocationId) select m);

                }
                else
                {

                }

            }
            else
            {
                model = (from m in _context.Site select m);
                if (site.LocationId != null)
                {
                    model = (from m in _context.Site where (m.LocationId == site.LocationId) select m) ;
                }
                else
                {

                }
            }




            return Json(model);
        }

        [HttpGet, Route("viewsitedetails")]
        public IActionResult ViewSiteDetails(Site site)
        {
            var siteModel = _context.Site.Where(s => s.Id == site.Id ).FirstOrDefault();

            return Json(siteModel);
        }

        [HttpGet("{site}/{month}/{year}/{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loadsalessummary")]
        public IActionResult LoadSalesSummary(Site site, int month, int year, string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
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

                // Getting transaction list for specific month (for graph)
                var eachday = new List<DateTime>();

                for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
                {
                    eachday.Add(new DateTime(year, month, i)); //map each day for specific month
                }


                //var transactionData = (from tempS in _context.Site
                //                       join tempT in _context.Transaction on tempS.Id equals tempT.SiteID into table1
                //                       join tempST in _context.SeasonTransaction on tempS.Id equals tempST.Id into table2
                //                       from y1 in table1.DefaultIfEmpty() 
                //                       from y2 in table2.DefaultIfEmpty()
                //                       where (y1.SiteID == site.Id && y1.TransactionAmount > 0 && y1.SuccessFlag == "Y" && y1.CardTypeTag == "T")
                //                       select new
                //                       {
                //                           TNGCount = table1.Select(y1.TransactionAmount).Sum(),
                //                           y1.LocationId,
                //                           y2.CardNo,
                //                           y3.InvoiceNo,
                //                           UserName = y4.Name,
                //                           y5.SeasonRateName,
                //                           Price = y5.Price.ToString("C"),
                //                           tempT.Id ,
                //                           tempT.TransactionTime,
                //                           TransactionAmount = tempT.TransactionAmount.GetValueOrDefault().ToString("C"),
                //                           tempT.PaymentGateway,
                //                           tempT.MerchantCode
                //                       });

                var result_tng = _context.Transaction
                  .Where(x => x.TransactionTime.Value.Month == month && x.TransactionTime.Value.Year == year && x.SuccessFlag == "Y" && x.CardTypeTag == "T" && x.TransactionAmount>0 && x.SiteId == site.Id )
                  .GroupBy(x => x.TransactionTime.Value.Date)
                  .Select(x => new
                  {
                      Count = x.Select(p => p.TransactionAmount).Sum(),
                      Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                  })
                  .ToList();

                var result_visa = _context.Transaction
                    .Where(x => x.TransactionTime.Value.Month == month && x.TransactionTime.Value.Year == year && x.SuccessFlag == "Y" && x.CardTypeTag == "E" && x.TransactionAmount > 0 && x.SiteId == site.Id )
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                    })
                .ToList();

                var result_season = _context.SeasonTransaction
                    .Where(x => x.TransactionTime.Value.Month == month && x.TransactionTime.Value.Year == year && x.TransactionAmount > 0 && x.SiteId == site.Id )
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                    })
                .ToList();


                List<SummaryTransResponse> summarylist = new List<SummaryTransResponse>();

                foreach (var dt in eachday)
                {
                    decimal tng_price = 0;
                    decimal visa_price = 0;
                    decimal season_price = 0;
                    SummaryTransResponse summary = new SummaryTransResponse();

                    foreach (var item in result_tng)
                    {
                        if (dt == item.Date)
                        {
                            tng_price = Convert.ToDecimal(item.Count / 100);
                            summary.TNG = tng_price;
                            break;
                        }
                    }
                    foreach (var item in result_visa)
                    {
                        if (dt == item.Date)
                        {
                            visa_price = Convert.ToDecimal(item.Count / 100);
                            summary.VisaMaster = visa_price;
                            break;
                        }
                    }
                    foreach (var item in result_season)
                    {
                        if (dt == item.Date)
                        {
                            season_price = Convert.ToDecimal(item.Count.GetValueOrDefault());
                            summary.SeasonPass = season_price;
                            break;
                        }
                    }

                    summary.TotalPrice = summary.TNG + summary.VisaMaster + summary.SeasonPass;
                    summary.Date = dt.ToString("dd/MM/yyyy");
                    summarylist.Add(summary);
                    }

                //total number of rows count   
                recordsTotal = summarylist.Count();
                ////Paging   
                var data = summarylist.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }








        public class DataResponse
        {
            public int SiteID { get; set; }
            public string SiteName { get; set; }
            public string LocationID { get; set; }
            public int Total { get; set; }
            public double? MBR { get; set; }
        }

        public class SummaryTransResponse
        {
            public string Date { get; set; }
            public decimal TNG { get; set; }
            public decimal VisaMaster { get; set; }
            public decimal SeasonPass { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}