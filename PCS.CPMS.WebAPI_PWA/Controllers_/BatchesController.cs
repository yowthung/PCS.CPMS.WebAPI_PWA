using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class BatchesController : ControllerBase
    {
        private readonly tng_cpmsContext _context;

        public BatchesController(tng_cpmsContext context)
        {
            _context = context;
        }

        [HttpGet("{batch}/{status}/{startDate}/{endDate}/{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loaddata")]
        public IActionResult LoadData([FromBody] BatchCreationLog batch, string status, DateTime startDate, DateTime endDate, string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
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

                // Getting all batch data required
                //var batchData = (from tempBatch in _context.BatchCreationLog where tempBatch.BatchFileName.Contains("FN") select tempBatch);
                //IQueryable <BatchCreationLog> model;

                //var sitelist = new List<BatchCreationLog>();

                var batchData = (from tempB in _context.BatchCreationLog
                                 join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                 from y1 in newtemp.DefaultIfEmpty()
                                 where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                 select new
                                 {
                                     y1.Name,
                                     y1.Mbr,
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
                    //var batchData = (from tempBatch in _context.BatchCreationLog where tempBatch.BatchFileName.Contains("FN") select tempBatch);
                    //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    batchData = (from tempB in _context.BatchCreationLog
                                           join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                           from y1 in newtemp.DefaultIfEmpty()
                                           where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                           select new
                                           {
                                               y1.Name,
                                               y1.Mbr,
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
                            batchData = (from tempB in _context.BatchCreationLog
                                             join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                             from y1 in newtemp.DefaultIfEmpty()
                                             where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == null)
                                             select new
                                             {
                                                 y1.Name,
                                                 y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null) select m);

                            }
                            else
                            {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true) select m);
                            }
                        }
                        else
                        {
                        batchData = (from tempB in _context.BatchCreationLog
                                     join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                     from y1 in newtemp.DefaultIfEmpty()
                                     where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId ==batch.TerminalId)
                                     select new
                                     {
                                         y1.Name,
                                         y1.Mbr,
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
                        //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId) select m);

                            if (status.Equals("ALL"))
                            {
                            }
                            else if (status.Equals("Pending"))
                            {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == null) 
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);

                            }
                            else
                            {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.BatchNumber == batch.BatchNumber && tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);

                            }
                        }
                    
                }
                //if batchno = null
                else
                {
                    batchData = (from tempB in _context.BatchCreationLog
                                     join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                     from y1 in newtemp.DefaultIfEmpty()
                                     where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate)
                                     select new
                                     {
                                         y1.Name,
                                         y1.Mbr,
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
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true) select m);
                        }
                    }
                    else
                    {
                        batchData = (from tempB in _context.BatchCreationLog
                                     join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                     from y1 in newtemp.DefaultIfEmpty()
                                     where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId)
                                     select new
                                     {
                                         y1.Name,
                                         y1.Mbr,
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
                        //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId) select m);

                        if (status.Equals("ALL"))
                        {
                        }
                        else if (status.Equals("Pending"))
                        {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == null)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);

                        }
                        else
                        {
                            batchData = (from tempB in _context.BatchCreationLog
                                         join tempS in _context.Site on tempB.SiteId equals tempS.Id into newtemp
                                         from y1 in newtemp.DefaultIfEmpty()
                                         where (tempB.Spid == batch.Spid && tempB.SiteId == batch.SiteId && tempB.BatchType == "FN" && tempB.BatchDate >= startDate && tempB.BatchDate <= endDate && tempB.TerminalId == batch.TerminalId && tempB.PayoutStatus == true)
                                         select new
                                         {
                                             y1.Name,
                                             y1.Mbr,
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
                            //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);

                        }
                    }
                    //var batchData = (from tempBatch in _context.BatchCreationLog where tempBatch.BatchFileName.Contains("FN") select tempBatch);
                    //model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.SiteId == batch.SiteId && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    ////if (batch.Spid.Equals("ALL"))
                    ////{
                    ////model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    ////if (batch.LocationId.Equals("ALL"))
                    ////{
                    //    //model = (from m in _context.BatchCreationLog where (m.BatchNumber == batch.BatchNumber && m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate) select m);

                    //    if (batch.TerminalId.Equals("ALL"))
                    //    {
                    //        if (status.Equals("ALL"))
                    //        {
                    //        }
                    //        else if (status.Equals("Pending"))
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.SiteId == batch.SiteId && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null) select m);

                    //        }
                    //        else
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.SiteId == batch.SiteId && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true) select m);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.SiteId == batch.SiteId && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId) select m);

                    //        if (status.Equals("ALL"))
                    //        {
                    //        }
                    //        else if (status.Equals("Pending"))
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.SiteId == batch.SiteId && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);

                    //        }
                    //        else
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.SiteId == batch.SiteId && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);

                    //        }
                    //    }
                }
                    //else
                    //{
                    //    model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId) select m);

                    //    if (batch.TerminalId.Equals("ALL"))
                    //    {
                    //        if (status.Equals("ALL"))
                    //        {
                    //        }
                    //        else if (status.Equals("Pending"))
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == null) select m);
                    //        }
                    //        else
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == true) select m);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId) select m);

                    //        if (status.Equals("ALL"))
                    //        {
                    //        }
                    //        else if (status.Equals("Pending"))
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == null) select m);
                    //        }
                    //        else
                    //        {
                    //            model = (from m in _context.BatchCreationLog where (m.Spid == batch.Spid && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == true) select m);
                    //        }
                    //    }
                    //}
                //}


                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    batchData = batchData.OrderBy(x => sortColumn + " " + sortColumnDirection);
                }

                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    batchData = batchData.Where(m => m.BatchDate.ToString().IndexOf(searchValue) > -1 || m.BatchNumber.ToString().IndexOf(searchValue) > -1);
                }

                ////List<DataResponse> listData = new List<DataResponse>();
                //foreach (var b in batchData)
                //{
                //    b.Mbr = b.TotalDebit * (100 - b.Mbr) / 100 / 100;
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
                //    var mbr = _context.Site.Where(s => s.Id == b.SiteId).Select(s => s.Mbr).FirstOrDefault();
                //    var tngrate = _context.Site.Where(s => s.Id == b.SiteId).Select(s => s.Tngrate).FirstOrDefault();

                //    dr.Mbr = (Convert.ToSingle(b.TotalDebit * (100 - mbr) / 100 / 100)).ToString("C");
                //    dr.Tngrate = (Convert.ToSingle(b.TotalDebit * (100 - tngrate) / 100 /100)).ToString("C");
                //    listData.Add(dr);
                //}


                //total number of rows count   
                recordsTotal = batchData.Count();
                //Paging   
                var data = batchData.Skip(skip).Take(pageSize).ToList();

                //foreach(var item in data)
                //{
                //    var site = _context.Site.Where(s => s.LocationId == item.LocationId).FirstOrDefault();
                //    item.LocationId = item.LocationId + " (" + site.Name + ")";
                //}
            
                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data});

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("{arrid}"), Route("batchdetails")]
        //POST: find id and return partial view
        public IActionResult BatchDetails(int[] arrId)
        {

            dynamic mymodel = new ExpandoObject();
            mymodel.Batches = new List<BatchCreationLog>();
            var locationid = "";
            var siteid = 0;
            foreach (var item in arrId)
            {
                var batch_no = "FN" + String.Format("{0:D5}", item);
                mymodel.Batches.Add(_context.BatchCreationLog.Where(m => m.BatchId == item).FirstOrDefault());
            }

            foreach(var item in mymodel.Batches)
            {
                locationid = item.LocationId;
                siteid = item.SiteId; 
   
            }

            mymodel.Site = _context.Site.Where(s => s.Id == siteid).FirstOrDefault();
            mymodel.Mbr = _context.Site.Where(s => s.Id == siteid).Select(s=> s.Mbr).FirstOrDefault();
            //mymodel = _context.Site.Where(s => s.Id == siteid).FirstOrDefault();

            return  new JsonResult(mymodel);
        }

        //public IActionResult SearchBatch(BatchCreationLog batch, string status, DateTime startDate, DateTime endDate)
        //{
        //    var model = new List<BatchCreationLog>();

        //    //
        //    if(!batch.BatchNumber.Equals("")) {
        //        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate).ToList();

        //        if (batch.Spid.Equals("ALL"))
        //        {
        //            if (batch.LocationId.Equals("ALL"))
        //            {
        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId).ToList();

        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid).ToList();

        //            if (batch.LocationId.Equals("ALL"))
        //            {
        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId).ToList();

        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchNumber == batch.BatchNumber && m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //        }
        //    }


        //    //if batchno not keyin
        //    else
        //    {
        //        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate).ToList();

        //        if (batch.Spid.Equals("ALL"))
        //        {
        //            if (batch.LocationId.Equals("ALL"))
        //            {
        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId).ToList();

        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid).ToList();

        //            if (batch.LocationId.Equals("ALL"))
        //            {
        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId).ToList();

        //                if (batch.TerminalId.Equals("ALL"))
        //                {
        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //                else
        //                {
        //                    model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId).ToList();

        //                    if (status.Equals("ALL"))
        //                    {
        //                    }
        //                    else if (status.Equals("Pending"))
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == null).ToList();
        //                    }
        //                    else
        //                    {
        //                        model = _context.BatchCreationLog.Where(m => m.BatchType == "FN" && m.BatchDate >= startDate && m.BatchDate <= endDate && m.Spid == batch.Spid && m.LocationId == batch.LocationId && m.TerminalId == batch.TerminalId && m.PayoutStatus == true).ToList();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Json(model);
        //}



        public class DataResponse
        {
            public int BatchId { get; set; }
            public string BatchNumber { get; set; }
            public DateTime? BatchDate { get; set; }
                    public DateTime? CreationDate { get; set; }
            public Int64 RecordCount { get; set; }
            public Int64? TotalDebit { get; set; }
            public string Spid { get; set; }

            public int SiteId { get; set; }
            public string LocationID { get; set; }

            public string TerminalID { get; set; }

            public Boolean? PayoutStatus { get; set; }

            public string ReferenceNo { get; set; }
             
            public string MBR { get; set; }
            public string TNGRate { get; set; }
        }





    }
}