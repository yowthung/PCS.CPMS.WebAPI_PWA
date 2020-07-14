using System;
using System.Linq;
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
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Dynamic;
using System.Net;
using Abp.Extensions;
using System.Data.Entity.SqlServer;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class TransactionsController : Controller
    {
        private readonly tng_cpmsContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConverter _converter;
        [Obsolete]
        private readonly IHostingEnvironment _env;

        public TransactionsController(tng_cpmsContext context, IHttpContextAccessor httpContextAccessor, IConverter converter, IHostingEnvironment env)
        {
            _db = context;
            _httpContext = httpContextAccessor;
            _httpContext.HttpContext.Request.EnableBuffering();
            _converter = converter;
            _env = env;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/LoadTngData")]
        [ActionName("api/Transactions/LoadTngData")]
        //POST: All 
        public IActionResult LoadTngData(Transaction trans, DateTime startDate, DateTime endDate, string roleid )
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

                //var transactionData = (from tempTransaction in _db.Transaction where tempTransaction.SuccessFlag == "Y" select tempTransaction);
                //IQueryable<Transaction> model;


                var batch_no = "FN" + String.Format("{0:D5}", Convert.ToInt32(trans.BatchNo));
             var  allow_site = (from c in _db.RolesAccess
                                 select int.Parse(c.SiteId.ToString())
                                ).ToList();
                                 

                
                var transactionData = (from tempT in _db.Transaction.Where(c => allow_site.Contains((int)c.SiteId))
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1) )
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.ManufacturerNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                       });

                //batchno exists
                if (trans.BatchNo != null)
                {
                    transactionData = (from tempT in _db.Transaction
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.ManufacturerNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                       });
                    //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                    //if (batch.Spid.Equals("ALL"))
                    //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    //if (trans.ExitLocation.Equals("ALL"))
                    //{      
                    if (trans.EntryLane.Equals("ALL"))
                    {
                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.ManufacturerNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                            //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                        }
                    }
                    else
                    {
                        transactionData = (from tempT in _db.Transaction
                                           join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                           from y1 in newtemp.DefaultIfEmpty()
                                           where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                           select new
                                           {
                                               y1.Name,
                                               y1.LocationId,
                                               tempT.BatchNo,
                                               tempT.ManufacturerNo,
                                               tempT.TransactionAmount,
                                               tempT.CardTypeTag,
                                               tempT.Spid,
                                               tempT.EntryLane,
                                               tempT.EntryTime,
                                               tempT.ExitLane,
                                               tempT.ExitTime,
                                               tempT.TransactionId,
                                               tempT.SiteId,
                                           });
                        //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.ManufacturerNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                            //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                        }
                    }
                  
                }

                //if batchno = null
                else
                {
                    transactionData = (from tempT in _db.Transaction
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.ManufacturerNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                       });
                    //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                    if (trans.EntryLane.Equals("ALL"))
                    {
                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.ManufacturerNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                            //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                        }
                    }
                    else
                    {
                        transactionData = (from tempT in _db.Transaction
                                           join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                           from y1 in newtemp.DefaultIfEmpty()
                                           where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                           select new
                                           {
                                               y1.Name,
                                               y1.LocationId,
                                               tempT.BatchNo,
                                               tempT.ManufacturerNo,
                                               tempT.TransactionAmount,
                                               tempT.CardTypeTag,
                                               tempT.Spid,
                                               tempT.EntryLane,
                                               tempT.EntryTime,
                                               tempT.ExitLane,
                                               tempT.ExitTime,
                                               tempT.TransactionId,
                                               tempT.SiteId,
                                           });
                        //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.EntryLane == trans.EntryLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.ManufacturerNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                            //model = (from m in _db.Transaction where (m.BatchNo.Contains(trans.BatchNo) && m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.EntryLane == trans.EntryLane && m.ExitLane == trans.ExitLane && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);
                        }
                    }
                    //model = (from m in _db.Transaction where (m.SiteId == trans.SiteId && m.Spid == trans.Spid && m.SuccessFlag == "Y" && m.TransactionTime >= startDate && m.TransactionTime <= endDate) select m);

                }

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

                //List<DataResponse> listDR = new List<DataResponse>();
                //foreach (var item in data)
                //{
                //    var site = _db.Site.Where(s => s.Id == item.SiteId).FirstOrDefault();
                //    var dataresponse = new DataResponse();
                //    dataresponse.SiteName = site.Name;
                //    dataresponse.LocationId = site.LocationId;
                //    dataresponse.BatchNo = item.BatchNo;
                //    dataresponse.ManufacturerNo = item.ManufacturerNo;
                //    dataresponse.TransactionAmount = item.TransactionAmount;
                //    dataresponse.CardTypeTag = item.CardTypeTag;
                //    dataresponse.Spid = item.Spid;
                //    dataresponse.EntryLane = item.EntryLane;
                //    dataresponse.EntryTime = item.EntryTime;
                //    dataresponse.ExitLane = item.ExitLane;
                //    dataresponse.ExitTime = item.ExitTime;
                //    listDR.Add(dataresponse);
                //}
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/LoadVisaMasterTransactionData")]
        [ActionName("api/Transactions/LoadVisaMasterTransactionData")]
        public IActionResult LoadVisaMasterTransactionData([FromForm]Transaction trans,[FromForm] DateTime startDate,[FromForm] DateTime endDate)
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

                //var batch_no = "FN" + String.Format("{0:D5}", Convert.ToInt32(trans.BatchNo));

                var transactionData = (from tempT in _db.Transaction
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.CardNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                       });

                //if batchno exists
                //if (trans.BatchNo != null)
                //{
                //    transactionData = (from tempT in _db.Transaction
                //                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                //                       from y1 in newtemp.DefaultIfEmpty()
                //                       where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                //                       select new
                //                       {
                //                           y1.Name,
                //                           y1.LocationId,
                //                           tempT.BatchNo,
                //                           tempT.CardNo,
                //                           tempT.TransactionAmount,
                //                           tempT.CardTypeTag,
                //                           tempT.Spid,
                //                           tempT.EntryLane,
                //                           tempT.EntryTime,
                //                           tempT.ExitLane,
                //                           tempT.ExitTime,
                //                           tempT.TransactionId,
                //                           tempT.SiteId,
                //                       });

                //    if (trans.EntryLane.Equals("ALL"))
                //    {
                //        if (trans.ExitLane.Equals("ALL"))
                //        { }
                //        else
                //        {
                //            transactionData = (from tempT in _db.Transaction
                //                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                //                               from y1 in newtemp.DefaultIfEmpty()
                //                               where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                //                               select new
                //                               {
                //                                   y1.Name,
                //                                   y1.LocationId,
                //                                   tempT.BatchNo,
                //                                   tempT.CardNo,
                //                                   tempT.TransactionAmount,
                //                                   tempT.CardTypeTag,
                //                                   tempT.Spid,
                //                                   tempT.EntryLane,
                //                                   tempT.EntryTime,
                //                                   tempT.ExitLane,
                //                                   tempT.ExitTime,
                //                                   tempT.TransactionId,
                //                                   tempT.SiteId,
                //                               });
                //        }
                //    }
                //    else
                //    {
                //        transactionData = (from tempT in _db.Transaction
                //                           join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                //                           from y1 in newtemp.DefaultIfEmpty()
                //                           where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                //                           select new
                //                           {
                //                               y1.Name,
                //                               y1.LocationId,
                //                               tempT.BatchNo,
                //                               tempT.CardNo,
                //                               tempT.TransactionAmount,
                //                               tempT.CardTypeTag,
                //                               tempT.Spid,
                //                               tempT.EntryLane,
                //                               tempT.EntryTime,
                //                               tempT.ExitLane,
                //                               tempT.ExitTime,
                //                               tempT.TransactionId,
                //                               tempT.SiteId,
                //                           });

                //        if (trans.ExitLane.Equals("ALL"))
                //        { }
                //        else
                //        {
                //            transactionData = (from tempT in _db.Transaction
                //                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                //                               from y1 in newtemp.DefaultIfEmpty()
                //                               where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                //                               select new
                //                               {
                //                                   y1.Name,
                //                                   y1.LocationId,
                //                                   tempT.BatchNo,
                //                                   tempT.CardNo,
                //                                   tempT.TransactionAmount,
                //                                   tempT.CardTypeTag,
                //                                   tempT.Spid,
                //                                   tempT.EntryLane,
                //                                   tempT.EntryTime,
                //                                   tempT.ExitLane,
                //                                   tempT.ExitTime,
                //                                   tempT.TransactionId,
                //                                   tempT.SiteId,
                //                               });
                //        }
                //    }
                //}

                //if batchno = no entered value 
                //else
                //{
                    transactionData = (from tempT in _db.Transaction
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.CardNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                       });

                    if (trans.EntryLane.Equals("ALL"))
                    {
                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.CardNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                        }
                    }
                    else
                    {
                        transactionData = (from tempT in _db.Transaction
                                           join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                           from y1 in newtemp.DefaultIfEmpty()
                                           where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                           select new
                                           {
                                               y1.Name,
                                               y1.LocationId,
                                               tempT.BatchNo,
                                               tempT.CardNo,
                                               tempT.TransactionAmount,
                                               tempT.CardTypeTag,
                                               tempT.Spid,
                                               tempT.EntryLane,
                                               tempT.EntryTime,
                                               tempT.ExitLane,
                                               tempT.ExitTime,
                                               tempT.TransactionId,
                                               tempT.SiteId,
                                           });

                        if (trans.ExitLane.Equals("ALL"))
                        { }
                        else
                        {
                            transactionData = (from tempT in _db.Transaction
                                               join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                               from y1 in newtemp.DefaultIfEmpty()
                                               where (tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "E" && tempT.Spid == trans.Spid && tempT.SiteId == trans.SiteId && tempT.EntryLane == trans.EntryLane && tempT.ExitLane == trans.ExitLane && tempT.TransactionTime >= startDate && tempT.TransactionTime < endDate.AddDays(1))
                                               select new
                                               {
                                                   y1.Name,
                                                   y1.LocationId,
                                                   tempT.BatchNo,
                                                   tempT.CardNo,
                                                   tempT.TransactionAmount,
                                                   tempT.CardTypeTag,
                                                   tempT.Spid,
                                                   tempT.EntryLane,
                                                   tempT.EntryTime,
                                                   tempT.ExitLane,
                                                   tempT.ExitTime,
                                                   tempT.TransactionId,
                                                   tempT.SiteId,
                                               });
                        }
                    }

                //}

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

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("/Transactions/LoadViewTngTransactionData")]
        [ActionName("api/Transactions/LoadViewTngTransactionData")]
        public IActionResult LoadViewTngTransactionData(string batchno, int siteID, string spid, string terminalID)
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

                //Getting all transaction data
                var transactionData = (from tempT in _db.Transaction
                                       join tempS in _db.Site on tempT.SiteId equals tempS.Id into newtemp
                                       from y1 in newtemp.DefaultIfEmpty()
                                       where (tempT.BatchNo == batch_no && tempT.SuccessFlag == "Y" && tempT.TransactionAmount > 0 && tempT.CardTypeTag == "T" && tempT.Spid == spid && tempT.SiteId == siteID && tempT.ExitLane == terminalID)
                                       select new
                                       {
                                           y1.Name,
                                           y1.LocationId,
                                           tempT.BatchNo,
                                           tempT.ManufacturerNo,
                                           tempT.TransactionAmount,
                                           tempT.CardTypeTag,
                                           tempT.Spid,
                                           tempT.EntryLane,
                                           tempT.EntryTime,
                                           tempT.ExitLane,
                                           tempT.ExitTime,
                                           tempT.TransactionId,
                                           tempT.SiteId,
                                           //tempT.SuccessFlag
                                       });

                //var transactionData = _db.Transaction.FromSql("SELECT s.Name, s.LocationId, t.BatchNo, t.ManufacturerNo, t.TransactionAmount, t.CardTypeTag, t.Spid, t.EntryLane, t.EntryTime, t.ExitLane, t.ExitTime " +
                //    "FROM Transaction t  left join Site s on t.SiteId = s.Id where t.BatchNo == batch_no && t.SuccessFlag == "Y" && t.CardTypeTag == "T" && t.Spid == spid && s.Id == siteID && t.ExitLane == terminalID");
                //transactionData.Where(t => t.BatchNo == batch_no && t.SuccessFlag == "Y" && t.CardTypeTag == "T" && t.Spid == spid && t.SiteId == siteID && t.ExitLane == terminalID).ToList();

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    transactionData = transactionData.OrderBy(x=>sortColumn + " " + sortColumnDirection);
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

                //List<DataResponse> listDR = new List<DataResponse>();
                //foreach (var item in data)
                //{
                //    var site = _db.Site.Where(s => s.Id == item.SiteId).FirstOrDefault();
                //    var dataresponse = new DataResponse();
                //    dataresponse.SiteName = site.Name;
                //    dataresponse.LocationId = site.LocationId;
                //    dataresponse.BatchNo = item.BatchNo;
                //    dataresponse.ManufacturerNo = item.ManufacturerNo;
                //    dataresponse.TransactionAmount = item.TransactionAmount;
                //    dataresponse.CardTypeTag = item.CardTypeTag;
                //    dataresponse.Spid = item.Spid;
                //    dataresponse.EntryLane = item.EntryLane;
                //    dataresponse.EntryTime = item.EntryTime;
                //    dataresponse.ExitLane = item.ExitLane;
                //    dataresponse.ExitTime = item.ExitTime;
                //    listDR.Add(dataresponse);
                //}

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data, batch_no = batch_no });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/GetTransactionAmount")]
        [ActionName("api/Transactions/GetTransactionAmount")]
        public IActionResult GetTransactionAmount([FromForm]string batchlist)
        {
            try
            {
                Dictionary<int, string> Trans = new Dictionary<int, string>();

                var blist = JsonConvert.DeserializeObject<IEnumerable<BatchCreationLog>>(batchlist);
                var price = 0;
                var subtotal = "";
                var batchid = 0;
                var siteid = 0;

                foreach (BatchCreationLog batch in blist)
                {
                    var batch_no = "FN" + batch.BatchNumber.PadLeft(5, '0');
                    var sum = _db.Transaction.
                        Where(t => t.BatchNo == batch_no && t.SiteId == batch.SiteId && t.Spid == batch.Spid && t.ExitLane == batch.TerminalId && t.TransactionAmount > 0).Select(t => t.TransactionAmount).Sum();

                    Trans.Add(batch.BatchId, (sum * 0.01)?.ToString("C"));

                    price += Convert.ToInt32(sum * 0.01);
                    batchid = batch.BatchId;
                    siteid = batch.SiteId;
                }

                var sitemdr = 100 - (_db.Site.Where(s => s.Id == siteid).Select(s => s.Mdr).FirstOrDefault());

                var total = ((decimal)(price * sitemdr / 100)).ToString("C");
                DateTime datetime = DateTime.Now;

                subtotal = price.ToString("C");
                var finaltotal = (decimal)(price * sitemdr / 100);

                //get site details 
                var sitedetails = _db.Site.Where(s => s.Id == siteid).FirstOrDefault();

                return Json(new { Trans, price, subtotal, total, datetime, batchid, finaltotal, sitedetails });
            }
            catch
            {
                throw;
            }
        }



        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/CalculateTransactionForTodayMonthOverall")]
        [ActionName("api/Transactions/CalculateTransactionForTodayMonthOverall")]
        //api for main dasboard upper part : total trans for daily, thismonth, and thisyear
        public IActionResult CalculateTransactionForTodayMonthOverall(int dayinput, int monthinput, int yearinput, List<int> sitelist)
        {
            decimal? total_amt_for_today = 0;
            decimal? total_amt_for_month = 0;
            decimal? total_amt_for_overall = 0;
            int? total_num_for_today = 0;
            int? total_num_for_month = 0;
            int? total_num_for_overall = 0;
            Dictionary<int, decimal?> day_Dict = new Dictionary<int, decimal?>();
            Dictionary<int, decimal?> month_Dict = new Dictionary<int, decimal?>();
            Dictionary<int, decimal?> year_Dict = new Dictionary<int, decimal?>();

            try
            {
                var site = new List<Site>();
                foreach (var item in sitelist)
                {
                    site.Add(_db.Site.Where(s => s.Id == item).FirstOrDefault());

                }

                foreach (var item in site)
                {
                    // Getting transaction number for today 
                    //total amount of transaction for tng and visa master
                    var today_amt_tng_visa_master = _db.Transaction.Where(x => x.TransactionTime.Value.Day == dayinput && x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var today_num_visa_master = _db.Transaction.Where(x => x.TransactionTime.Value.Day == dayinput && x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionId).Count();

                    //total amount of transaction for season card
                    var today_amt_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Day == dayinput && x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.Status == 1 && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var today_num_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Day == dayinput && x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.Status == 1 && x.SiteId == item.Id).Select(x => x.Id).Count();

                    total_amt_for_today += (today_amt_tng_visa_master / 100) + today_amt_seasoncard;
                    total_num_for_today += today_num_visa_master + today_num_seasoncard;
                    //day_Dict.Add(item.Id, day_tng_visa_master + day_seasoncard);

                    // Getting transaction amount for current month
                    //total amount of transaction for tng and visa master
                    var month_amt_tng_visa_master = _db.Transaction.Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var month_num_tng_visa_master = _db.Transaction.Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionId).Count();

                    //total amount of transaction for season card
                    var month_amt_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.Status == 1 && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var month_num_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.Status == 1 && x.SiteId == item.Id).Select(x => x.Id).Count();

                    total_amt_for_month += (month_amt_tng_visa_master / 100) + month_amt_seasoncard;
                    total_num_for_month += month_num_tng_visa_master + month_num_seasoncard;
                    //month_Dict.Add(item.Id, month_tng_visa_master + month_seasoncard);

                    //// Getting transaction amount for current year
                    ////total amount of transaction for tng and visa master
                    //var year_tng_visa_master = _db.Transaction.Where(x => x.TransactionTime.Value.Year == yearinput &&
                    //  x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    ////total amount of transaction for season card
                    //var year_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Year == yearinput &&
                    //  x.Status == 1 && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    //total_for_year += year_tng_visa_master + year_seasoncard;
                    //year_Dict.Add(item.Id, year_tng_visa_master + year_seasoncard);
                    var overall_amt_tng_visa_master = _db.Transaction.Where(x => x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var overall_num_tng_visa_master = _db.Transaction.Where(x => x.SuccessFlag == "Y" && x.SiteId == item.Id).Select(x => x.TransactionId).Count();

                    //total amount of transaction for season card
                    var overall_amt_seasoncard = _db.SeasonTransaction.Where(x => x.Status == 1 && x.SiteId == item.Id).Select(x => x.TransactionAmount).Sum();

                    var overall_num_seasoncard = _db.SeasonTransaction.Where(x => x.Status == 1 && x.SiteId == item.Id).Select(x => x.Id).Count();

                    total_amt_for_overall += (overall_amt_tng_visa_master / 100) + overall_amt_seasoncard;
                    total_num_for_overall += overall_num_tng_visa_master + overall_num_seasoncard;

                }

                return Json(
                    new { total_amt_for_today, total_amt_for_month, total_amt_for_overall, total_num_for_today, total_num_for_month, total_num_for_overall });

            }
            catch
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/CalculateTransactionForMonth")]
        [ActionName("api/Transactions/CalculateTransactionForMonth")]
        //api for main dashboard: main graph 
        public IActionResult CalculateTransactionForMonth(Site site, int monthinput, int yearinput)
        {
            try
            {
                // Getting transaction list for specific month (for graph)
                var eachday = new List<DateTime>();

                for (int i = 1; i <= DateTime.DaysInMonth(yearinput, monthinput); i++)
                {
                    eachday.Add(new DateTime(yearinput, monthinput, i)); //map each day for specific month
                }

                //Decimal? total_amount;

                var amount_tng_visa_master = _db.Transaction
                    .Where(x => x.TransactionTime.Value.Month == monthinput && x.TransactionTime.Value.Year == yearinput &&
                      x.SuccessFlag == "Y" && x.SiteId == site.Id)
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                })
                  .ToList();

                //total no of transaction for tng and visa master
                var amount_seasoncard = _db.SeasonTransaction.Where(x => x.TransactionTime.Value.Month == monthinput
                && x.TransactionTime.Value.Year == yearinput && x.Status == 1 && x.SiteId == site.Id)
                    .GroupBy(x => x.TransactionTime.Value.Date)
                    .Select(x => new
                    {
                        Count = x.Select(p => p.TransactionAmount).Sum(),
                        Date = (DateTime)x.Key // or x.Key.Date (excluding time info) or x.Key.Date.ToString() (give only Date in string format) 
                })
                  .ToList();

                Dictionary<string, decimal?> dataDict = new Dictionary<string, decimal?>();
                foreach (var dt in eachday)
                {
                    decimal count_tng = 0;
                    decimal? count_season = 0;

                    foreach (var item in amount_tng_visa_master)
                    {
                        if (dt == item.Date)
                        {
                            count_tng = Convert.ToDecimal((item.Count) / 100);
                            break;
                        }
                    }

                    foreach (var item in amount_seasoncard)
                    {
                        if (dt == item.Date)
                        {
                            count_season = item.Count;
                            break;
                        }
                    }

                    dataDict.Add(dt.ToString("yyyy-MM-dd"), count_tng + count_season);

                }

                //return Json(dataDict);
                return Json(new { dataDict = dataDict });
            }
            catch
            {
                throw;
            }

        }





        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("api/Transactions/getinvoiceprinting")]
        [ActionName("api/Transactions/getinvoiceprinting")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //api for main dashboard: main graph 
        public async Task<ActionResult> GetInvoicePrinting([FromForm] string batchlist)
        {

            try
            {
                var obj = new UserListPdfExporter(_db, _converter);
                //  obj.ConvertUserListToHtmlTable(Trans, price, subtotal, total, datetime, batchid, finaltotal, sitedetails);
                var html = await obj.ConvertUserListToHtmlTable(batchlist);


                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                    Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = html,
                        PagesCount = true,
                        //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine (Directory.GetCurrentDirectory (), "assets", "styles.css") }

                    }
                }
                };

                var file = new FileDto("Invoice.pdf", _converter.Convert(doc));

                var webRoot = _env.ContentRootPath;
                var filepath = System.IO.Path.Combine(webRoot, @"Files/Invoice_Payout_" + DateTime.UtcNow.Ticks.ToString() + ".pdf");
                //string filepath = _env(@"~/Files/Invoice_Payout_" + DateTime.UtcNow.Ticks.ToString() + ".pdf");// @"Files\Invoice_Payout_" + DateTime.UtcNow.Ticks.ToString() + ".pdf";
                // return new FileDto("UserList.pdf", _converter.Convert(doc));
                //  byte[] pdf = _converter.Convert(doc);

                byte[] pdf_byte = _converter.Convert(doc);

                using (FileStream stream = new FileStream(filepath, FileMode.Create))
                {
                    stream.Write(pdf_byte, 0, pdf_byte.Length);
                }

                return File(pdf_byte, "application/pdf");
                // return new FileContentResult(pdf, "application/pdf");
                //  HttpContext.Response.ContentType = "application/pdf";

                //string filename = @"report.pdf";
                //HttpContext.Response.Headers.Add("x-filename", file.FileName);
                //HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
                //HttpContext.Response.Body.Write(file.FileBytes);
                //return new ContentResult();
                //return new FileContentResult(file.FileBytes, "application/pdf");
                // return File(file, "application.pdf", "Invoice.pdf");

                // return File(file.FileBytes, "application/pdf", filepath);



                //byte[] myBytes = System.IO.File.ReadAllBytes(filepath);
                //return File(myBytes, "application/pdf");
            }
            catch (Exception ex)
            { throw ex; }
        }
      
    }

    public class DataResponse
    {

        public string SiteName { get; set; }
        public string LocationID { get; set; }
        public string BatchNo { get; set; }
        public string ManufacturerNo { get; set; }
        public Int64? TransactionAmount { get; set; }
        public char? CardTypeTag { get; set; }
        public string Spid { get; set; }
        public string EntryLane { get; set; }
        public DateTime? EntryTime { get; set; }
        public string ExitLane { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}







