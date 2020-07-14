using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Dynamic;
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

    // [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class SitesController : Controller
    {   
        private readonly tng_cpmsContext _db;

        public SitesController(tng_cpmsContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/LoadData")]
        [ActionName("api/Sites/LoadData")]
        //POST: load site data after search apply
        public IActionResult LoadData(Site site)
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

                //IQueryable<Site> model;

                //model = (from m in _db.Site where (m.Id == site.Id) select m);

                var siteData = (from tempS in _db.Site
                                       join tempC in _db.Companies on tempS.CompanyId equals tempC.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempS.Id == site.Id)
                                       select new
                                       {
                                           y1.CompanyName,
                                           y1.CompanyRegistrationNo,
                                           y1.CompanyAddress,
                                           y1.ContactNo,
                                           y1.CompanyBankName, 
                                           y1.CompanyBankAccountNo,
                                           y1.CompanyBankAccountName,
                                           tempS.Id,
                                           tempS.Name,
                                           tempS.LocationId,
                                           tempS.TotalEntryLane,
                                           tempS.TotalExitLane,
                                           tempS.TotalValidator,
                                           tempS.Podate,
                                           tempS.EnrollDate,
                                           tempS.ContractYear,
                                           tempS.SiteContactPersonName,
                                           tempS.SiteContactPersonTelNo,
                                           tempS.SiteContactPersonEmail,
                                           tempS.Mdr,
                                           tempS.Tngrate,
                                           tempS.TngrateMargin,
                                           tempS.MonthlyLaneCharge,
                                           tempS.SettlementDay,
                                           tempS.SiteBankName,
                                           tempS.SiteBankAccountNo,
                                           tempS.SiteBankAccountName
                                       });

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    siteData = siteData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    siteData = siteData.Where(m => m.Name.ToString().IndexOf(searchValue) > -1 || m.LocationId.IndexOf(searchValue) > -1);
                }

                //total number of rows count   
                recordsTotal = siteData.Count();

                //Paging   
                var data = siteData.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/DeleteSite")]
        [ActionName("api/Sites/DeleteSite")]
        public IActionResult DeleteSite([FromForm]Site site)
        {
            try
            {
                var selectedsite = _db.Site.Where(s => s.Id == site.Id).FirstOrDefault();

                _db.Site.Remove(selectedsite);
                _db.SaveChanges();

                return Ok(new
                {
                    Message = "Success"
                });
            }
            catch
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/EditSite")]
        [ActionName("api/Sites/EditSite")]
        public IActionResult EditSite([FromForm]Site site)
        {
            try
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Company = new List<Company>();

                var selected_site = _db.Site.Where(s => s.Id == site.Id).FirstOrDefault();
                mymodel.Company.Add(_db.Companies.Where(c => c.Id == selected_site.CompanyId).FirstOrDefault());

                var othercompanies = _db.Companies.Where(c => c.Id != selected_site.CompanyId).ToList();

                foreach (var item in othercompanies)
                {
                    mymodel.Company.Add(item);
                }

                mymodel.Site = selected_site;

                return PartialView("_EditSiteModal", mymodel);
            }
            catch
            {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/SaveEditChanges")]
        [ActionName("api/Sites/SaveEditChanges")]
        public IActionResult SaveEditChanges([FromForm]Site site)
        {
            var selectedsite = _db.Site.Where(s => s.Id == site.Id).FirstOrDefault();

            try
            {
                selectedsite.Name = site.Name;
                selectedsite.LocationId = site.LocationId;
                selectedsite.Tngrate = site.Tngrate;
                selectedsite.Mdr = site.Mdr;
                selectedsite.Tngrate = site.Tngrate;
                selectedsite.TngrateMargin = site.TngrateMargin;
                selectedsite.TotalEntryLane = site.TotalEntryLane;
                selectedsite.TotalExitLane = site.TotalExitLane;
                selectedsite.TotalValidator = site.TotalValidator;
                selectedsite.Podate = site.Podate;
                selectedsite.EnrollDate = site.EnrollDate;
                selectedsite.ContractYear = site.ContractYear;
                selectedsite.SiteContactPersonName = site.SiteContactPersonName;
                selectedsite.SiteContactPersonTelNo = site.SiteContactPersonTelNo;
                selectedsite.SiteContactPersonEmail = site.SiteContactPersonEmail;
                selectedsite.MonthlyLaneCharge = site.MonthlyLaneCharge;
                selectedsite.SettlementDay = site.SettlementDay;
                selectedsite.CompanyId = site.CompanyId;
                selectedsite.SiteBankName = site.SiteBankName;
                selectedsite.SiteBankAccountNo = site.SiteBankAccountNo;
                selectedsite.SiteBankAccountName = site.SiteBankAccountName;
                selectedsite.LastUpdatedTime = site.LastUpdatedTime;
                selectedsite.LastUpdatedBy = site.LastUpdatedBy;

                _db.SaveChanges();

                return Ok(new
                {
                    Message = "Success"
                });
            }
            catch
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/AddSite")]
        [ActionName("api/Sites/AddSite")]
        public IActionResult AddSite()
        {
            dynamic mymodel = new ExpandoObject();

            mymodel.Company = _db.Companies.ToList();
            return PartialView("_AddSiteModal", mymodel);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/SaveAddChanges")]
        [ActionName("api/Sites/SaveAddChanges")]
        public IActionResult SaveAddChanges(Site site)
        {
            try
            {
                if (site.Name == null || site.LocationId == null)
                {
                    return Ok(new
                    {
                        Message = "Please fill out the blank!"
                    });
                }
                else
                {
                    //db checking if account existed
                    if (_db.Site.Any(x => x.LocationId.Equals(site.LocationId) && x.Name.Equals(site.Name)))
                    {
                        return Ok(new
                        {
                            Message = "The location ID already appear in this site name! Enter new site name or location ID."
                        });
                    }
                    else
                    {
                        _db.Site.Add(site);
                        _db.SaveChanges();

                        return Ok(new
                        {
                            Message = "Success"
                        });
                    }
                }
            }
            catch
            {
                throw; 
            }

        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/LoadDataInDashboard")]
        [ActionName("api/Sites/LoadDataInDashboard")]
        public IActionResult LoadDataInDashboard()
        {
            try
            {

                var siteList = new List<int>();
                siteList.Add(4);
                siteList.Add(6);

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

                var site = new List<Site>();

                foreach (var item in siteList)
                {
                    site.Add(_db.Site.Where(s => s.Id == item).FirstOrDefault());
                }

                List<DataResponse> listDataResp = new List<DataResponse>();
                foreach (var s in site)
                {
                    DataResponse dr = new DataResponse();
                    dr.SiteID = s.Id;
                    dr.LocationId = s.LocationId;
                    dr.SiteName = s.Name;
                    dr.TNGRate = s.Tngrate;
                    dr.MDR = s.Mdr;
                    dr.TotalEntryLane = s.TotalEntryLane;
                    dr.TotalExitLane = s.TotalExitLane;
                    dr.TotalValidator = s.TotalValidator;

                    var current_date = DateTime.Now;

                    var amount_tng_visa_master = _db.Transaction
                        .Where(x => x.TransactionTime < current_date.AddDays(1) &&
                               x.SuccessFlag == "Y" && x.SiteId == s.Id)
                        .Select(x => x.TransactionAmount).Sum();

                    var amount_tng_visa_master_yesterday = _db.Transaction
                        .Where(x => x.TransactionTime >= current_date.AddDays(-1) && x.TransactionTime < current_date.AddDays(1) &&
                               x.SuccessFlag == "Y" && x.SiteId == s.Id)
                        .Select(x => x.TransactionAmount).Sum();

                    var amount_tng_visa_master_today = _db.Transaction
                        .Where(x => x.TransactionTime >= current_date && x.TransactionTime < current_date.AddDays(1) &&
                               x.SuccessFlag == "Y" && x.SiteId == s.Id)
                        .Select(x => x.TransactionAmount).Sum();


                    var amount_seasoncard = _db.SeasonTransaction
                        .Where(x => x.TransactionTime < current_date.AddDays(1) &&
                               x.Status == 1 && x.SiteId == s.Id)
                        .Select(x => x.TransactionAmount).Sum();

                    var amount_seasoncard_yesterday = _db.SeasonTransaction
                    .Where(x => x.TransactionTime >= current_date.AddDays(-1) &&  x.TransactionTime < current_date.AddDays(1) &&
                        x.Status == 1 && x.SiteId == s.Id)
                    .Select(x => x.TransactionAmount).Sum();

                    var amount_seasoncard_today = _db.SeasonTransaction
                    .Where(x => x.TransactionTime >= current_date && x.TransactionTime < current_date.AddDays(1) &&
                    x.Status == 1 && x.SiteId == s.Id)
                    .Select(x => x.TransactionAmount).Sum();

                    dr.TotalAmount = (Convert.ToDecimal(amount_tng_visa_master) / 100) + amount_seasoncard;

                    dr.TotalAmountYesterday = (Convert.ToDecimal(amount_tng_visa_master_yesterday) / 100) + amount_seasoncard_yesterday;

                    dr.TotalAmountToday = (Convert.ToDecimal(amount_tng_visa_master_today) / 100) + amount_seasoncard_today;

                    dr.TotalPendingBatch = _db.BatchCreationLog.Where(b=> b.BatchType == "FN" && b.PayoutStatus == null && b.SiteId == s.Id).Select(x=> x.BatchId).Count();

                    listDataResp.Add(dr);
                }

                //total number of rows count   
                recordsTotal = listDataResp.Count();

                //Paging
                //var data = listDataResp.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listDataResp });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/SearchSite")]
        [ActionName("api/Sites/SearchSite")]
        public IActionResult SearchSite([FromForm]Site site)
        {
            IQueryable<Site> model;

            if (site.Name != null)
            {
                model = (from m in _db.Site where (m.Name == site.Name) select m);

                if(site.LocationId != null)
                {
                    model = (from m in _db.Site where (m.Name == site.Name && m.LocationId == site.LocationId) select m);

                }
                else
                {

                }

            }
            else
            {
                model = (from m in _db.Site select m);
                if (site.LocationId != null)
                {
                    model = (from m in _db.Site where (m.LocationId == site.LocationId) select m) ;
                }
                else
                {

                }
            }

            return Json(model);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/ViewSiteDetails")]
        [ActionName("api/Sites/ViewSiteDetails")]
        public IActionResult ViewSiteDetails([FromForm]Site site)
        {
            try
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Company = new List<Company>();

                var siteModel = _db.Site.Where(s => s.Id == site.Id).FirstOrDefault();
                mymodel.Company = _db.Companies.Where(c => c.Id == siteModel.CompanyId).FirstOrDefault();
                mymodel.Site = siteModel;

                return Json(mymodel);
            }
            catch
            {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Sites/LoadSalesSummary")]
        [ActionName("api/Sites/LoadSalesSummary")]
        public IActionResult LoadSalesSummary([FromForm]Site site, [FromForm] DateTime startDate, [FromForm] DateTime endDate)
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

                // Getting transaction list for specific month (for graph)
                var eachday = new List<DateTime>();

                for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    eachday.Add(dt);
                }

                var result_tng = _db.Transaction
                  .Where(x => x.TransactionTime >= startDate && x.TransactionTime < endDate.AddDays(1) && x.SuccessFlag == "Y" && x.CardTypeTag == "T" && x.TransactionAmount>0 && x.SiteId == site.Id)
                  .GroupBy(x => x.TransactionTime.Value.Date)
                  .Select(x => new
                  {
                      Count = x.Select(p => p.TransactionAmount).Sum(),
                      Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                  })
                  .ToList();

                var result_visa = _db.Transaction
                    .Where(x => x.TransactionTime >= startDate && x.TransactionTime < endDate.AddDays(1) && x.SuccessFlag == "Y" && x.CardTypeTag == "E" && x.TransactionAmount > 0 && x.SiteId == site.Id)
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                    })
                .ToList();

                var result_season = _db.SeasonTransaction
                    .Where(x => x.TransactionTime >= startDate && x.TransactionTime < endDate.AddDays(1) && x.TransactionAmount > 0 && x.SiteId == site.Id && x.Status == 1)
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                    })
                .ToList();


                List<SummaryTransResponse> summarylist = new List<SummaryTransResponse>();

                var selected_site = _db.Site.Where(s => s.Id == site.Id).FirstOrDefault();

                foreach (var dt in eachday)
                {
                    decimal tng_price = 0;
                    decimal visa_price = 0;
                    decimal season_price = 0;
                    SummaryTransResponse summary = new SummaryTransResponse();

                    summary.Site = selected_site.LocationId + " (" + selected_site.Name + ")";

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
                            season_price = item.Count.GetValueOrDefault();
                            summary.SeasonPass = season_price;
                            break;
                        }
                    }

                    summary.TotalPrice = summary.TNG + summary.VisaMaster + summary.SeasonPass;

                    summary.Date = dt.ToString("dd/MM/yyyy");
                    if (summary.TotalPrice != 0)
                    {
                        summarylist.Add(summary);
                    }
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
            public string LocationId { get; set; }
            public int? TotalEntryLane { get; set; }
            public int? TotalExitLane { get; set; }
            public int? TotalValidator { get; set; }
            public double? MDR { get; set; }
            public double? TNGRate { get; set; }
            public decimal? TotalAmount { get; set; }
            public decimal? TotalAmountYesterday { get; set; }
            public decimal? TotalAmountToday { get; set; }
            public int TotalPendingBatch { get; set; }
        }

        public class SummaryTransResponse
        {
            public string Site { get; set; }
            public string Date { get; set; }
            public decimal TNG { get; set; }
            public decimal VisaMaster { get; set; }
            public decimal SeasonPass { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}