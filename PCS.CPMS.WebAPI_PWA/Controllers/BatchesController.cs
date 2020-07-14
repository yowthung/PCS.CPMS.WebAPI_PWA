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
using System;
//using System.Web.Http.Cors;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{

    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class BatchesController : Controller
    {
        private readonly tng_cpmsContext _db;

        public BatchesController(tng_cpmsContext db)
        {
            _db = db;
        }


        [Microsoft.AspNetCore.Mvc.HttpPost("api/Batches/LoadData")]
        [ActionName("api/Batches/LoadData")]
        //after apply search, show data
        public IActionResult LoadData(BatchCreationLog batch, string status, DateTime startDate, DateTime endDate)
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

                // Getting all batch data required
                //var batchData = (from tempBatch in _db.BatchCreationLog where tempBatch.BatchFileName.Contains("FN") select tempBatch);
                //IQueryable <BatchCreationLog> model;

                //var sitelist = new List<BatchCreationLog>();

                var batchData = (from tempB in _db.BatchCreationLog
                                 join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                 from y1 in newtemp.DefaultIfEmpty()
                                 where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                 select new
                                 {
                                     y1.Name,
                                     y1.Mdr,
                                     y1.Tngrate,
                                     tempB.BatchId,
                                     tempB.BatchNumber,
                                     tempB.Spid,
                                     tempB.LocationId,
                                     tempB.TerminalId,
                                     tempB.RecordCount,
                                     tempB.TotalDebit,
                                     tempB.BatchDate,
                                     tempB.CreationDate,
                                     tempB.ReferenceNo,
                                     tempB.PayoutStatus,
                                     tempB.SiteId
                                 });

                if (batch.BatchNumber != null)
                {
                    //var batchData = (from tempBatch in _db.BatchCreationLog where tempBatch.BatchFileName.Contains("FN") select tempBatch);
                    //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    batchData = (from tempB in _db.BatchCreationLog
                                 join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                 from y1 in newtemp.DefaultIfEmpty()
                                 where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                 select new
                                 {
                                     y1.Name,
                                     y1.Mdr,
                                     y1.Tngrate,
                                     tempB.BatchId,
                                     tempB.BatchNumber,
                                     tempB.Spid,
                                     tempB.LocationId,
                                     tempB.TerminalId,
                                     tempB.RecordCount,
                                     tempB.TotalDebit,
                                     tempB.BatchDate,
                                     tempB.CreationDate,
                                     tempB.ReferenceNo,
                                     tempB.PayoutStatus,
                                     tempB.SiteId
                                 });

                    if (batch.TerminalId.Equals("ALL"))
                    {
                        if (status.Equals("ALL"))
                        {
                        }
                        else if (status.Equals("Pending"))
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true) select m);
                        }
                    }
                    else
                    {
                        batchData = (from tempB in _db.BatchCreationLog
                                     join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                     from y1 in newtemp.DefaultIfEmpty()
                                     where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId)
                                     select new
                                     {
                                         y1.Name,
                                         y1.Mdr,
                                         y1.Tngrate,
                                         tempB.BatchId,
                                         tempB.BatchNumber,
                                         tempB.Spid,
                                         tempB.LocationId,
                                         tempB.TerminalId,
                                         tempB.RecordCount,
                                         tempB.TotalDebit,
                                         tempB.BatchDate,
                                         tempB.CreationDate,
                                         tempB.ReferenceNo,
                                         tempB.PayoutStatus,
                                         tempB.SiteId
                                     });
                        //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId) select m);

                        if (status.Equals("ALL"))
                        {
                        }
                        else if (status.Equals("Pending"))
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);

                        }
                    }

                }
                //if batchno = null
                else
                {
                    batchData = (from tempB in _db.BatchCreationLog
                                 join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                 from y1 in newtemp.DefaultIfEmpty()
                                 where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                 select new
                                 {
                                     y1.Name,
                                     y1.Mdr,
                                     y1.Tngrate,
                                     tempB.BatchId,
                                     tempB.BatchNumber,
                                     tempB.Spid,
                                     tempB.LocationId,
                                     tempB.TerminalId,
                                     tempB.RecordCount,
                                     tempB.TotalDebit,
                                     tempB.BatchDate,
                                     tempB.CreationDate,
                                     tempB.ReferenceNo,
                                     tempB.PayoutStatus,
                                     tempB.SiteId
                                 });

                    if (batch.TerminalId.Equals("ALL"))
                    {
                        if (status.Equals("ALL"))
                        {
                        }
                        else if (status.Equals("Pending"))
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true) select m);
                        }
                    }
                    else
                    {
                        batchData = (from tempB in _db.BatchCreationLog
                                     join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                     from y1 in newtemp.DefaultIfEmpty()
                                     where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId)
                                     select new
                                     {
                                         y1.Name,
                                         y1.Mdr,
                                         y1.Tngrate,
                                         tempB.BatchId,
                                         tempB.BatchNumber,
                                         tempB.Spid,
                                         tempB.LocationId,
                                         tempB.TerminalId,
                                         tempB.RecordCount,
                                         tempB.TotalDebit,
                                         tempB.BatchDate,
                                         tempB.CreationDate,
                                         tempB.ReferenceNo,
                                         tempB.PayoutStatus,
                                         tempB.SiteId
                                     });
                        //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId) select m);

                        if (status.Equals("ALL"))
                        {
                        }
                        else if (status.Equals("Pending"))
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _db.BatchCreationLog
                                         join tempS in _db.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mdr,
                                             y1.Tngrate,
                                             tempB.BatchId,
                                             tempB.BatchNumber,
                                             tempB.Spid,
                                             tempB.LocationId,
                                             tempB.TerminalId,
                                             tempB.RecordCount,
                                             tempB.TotalDebit,
                                             tempB.BatchDate,
                                             tempB.CreationDate,
                                             tempB.ReferenceNo,
                                             tempB.PayoutStatus,
                                             tempB.SiteId
                                         });
                            //model = (from m in _db.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);

                        }
                    }
                    
                }

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    batchData = batchData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    batchData = batchData.Where(m => m.BatchDate.ToString().IndexOf(searchValue) > -1 || m.BatchNumber.ToString().IndexOf(searchValue) > -1);
                }

                ////List<DataResponse> listData = new List<DataResponse>();
                //foreach (var b in batchData)
                //{
                //    b.MBR = b.TotalDebit * (100 - b.MBR) / 100 / 100;
                //    b.Tngrate = Convert.ToSingle(b.TotalDebit * (100 - b.Tngrate) / 100 / 100);
                //}
                //    DataResponse dr = new DataResponse();

                //    dr.BatchId = b.BatchId;                    
                //    dr.BatchNumber = b.BatchNumber;
                //    dr.BatchDate = b.BatchDate;
                //    dr.CreationDate = b.CreationDate;
                //    dr.SiteId = b.SiteId;
                //    dr.RecordCount = b.RecordCount;
                //    dr.TotalDebit = b.TotalDebit;
                //    dr.Spid = b.Spid;
                //    dr.LocationId = b.LocationId;
                //    dr.TerminalId = b.TerminalId;
                //    dr.PayoutStatus = b.PayoutStatus;
                //    dr.ReferenceNo = b.ReferenceNo;
                //    var mbr = _db.Site.Where(s => s.Id == b.SiteId).Select(s => s.MBR).FirstOrDefault();
                //    var tngrate = _db.Site.Where(s => s.Id == b.SiteId).Select(s => s.Tngrate).FirstOrDefault();

                //    dr.MBR = (Convert.ToSingle(b.TotalDebit * (100 - mbr) / 100 / 100)).ToString("C");
                //    dr.Tngrate = (Convert.ToSingle(b.TotalDebit * (100 - tngrate) / 100 /100)).ToString("C");
                //    listData.Add(dr);
                //}


                //total number of rows count   
                recordsTotal = batchData.Count();
                //Paging   
                var data = batchData.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/Batches/BatchDetails")]
        [ActionName("api/Batches/BatchDetails")]
        //POST: find id and return partial view
        public IActionResult BatchDetails(int[] arrId)
        {
            try
            {
                dynamic mymodel = new ExpandoObject();
                mymodel.Batches = new List<BatchCreationLog>();
                var locationid = "";
                var siteid = 0;
                foreach (var item in arrId)
                {
                    var batch_no = "FN" + String.Format("{0:D5}", item);
                    mymodel.Batches.Add(_db.BatchCreationLog.Where(m => m.BatchId == item).FirstOrDefault());
                }

                foreach (var item in mymodel.Batches)
                {
                    locationid = item.LocationId;
                    siteid = item.SiteId;

                }

                var selected_site = _db.Site.Where(s => s.Id == siteid).FirstOrDefault();
                mymodel.Company = _db.Companies.Where(c => c.Id == selected_site.CompanyId).FirstOrDefault();
                mymodel.Mdr = _db.Site.Where(s => s.Id == siteid).Select(s => s.Mdr).FirstOrDefault();
                mymodel.Site = selected_site;

                return PartialView("_PayoutSubmitModal", mymodel);
            }
            catch
            {
                throw;
            }
        }
     
        //public class DataResponse
        //{
        //    public int BatchId { get; set; }
        //    public string BatchNumber { get; set; }
        //    public DateTime? BatchDate { get; set; }
        //    public DateTime? CreationDate { get; set; }
        //    public Int64 RecordCount { get; set; }
        //    public Int64? TotalDebit { get; set; }
        //    public string SPID { get; set; }

        //    public int SiteID { get; set; }
        //    public string LocationId { get; set; }

        //    public string TerminalId { get; set; }

        //    public Boolean? PayoutStatus { get; set; }

        //    public string ReferenceNo { get; set; }

        //    public string MBR { get; set; }
        //    public string Tngrate { get; set; }
        //}

    }
}