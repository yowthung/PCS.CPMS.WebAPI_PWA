using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Hosting;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
   // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class DashboardsController : ControllerBase
    {


        private readonly tng_cpmsContext _context;

        public DashboardsController(tng_cpmsContext context)
        {
            _context = context;
        }

      
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("Dashboard")]
        [ActionName("Dashboard")]
        //   [Consumes("application/json")]
        [Consumes("application/x-www-form-urlencoded")]
        //POST: All Transactions (JSON) from table configuration  
        public IActionResult Dashboard([FromForm] int roleid)
        {

            dynamic mymodel = new ExpandoObject();
            var allow_site = _context.RolesAccess.OrderBy(s => s.RoleId == roleid).ToList();
            var site = _context.Site.Where(data => allow_site.Any(x => data.Id == x.Id));
            mymodel.Total = new List<System.Numerics.BigInteger>();

            foreach (var s in site)
            {
                mymodel.Total.Add(_context.BatchCreationLog.Where(t => t.LocationId == s.LocationId).Select(t => t.RecordCount).Sum());
            }

            mymodel.Site = site;
            mymodel.Batch = _context.BatchCreationLog.Where(b => b.BatchType == "FN" && b.PayoutStatus == null).OrderByDescending(b=> b.BatchDate).ToList();

            mymodel.BatchNumber = new List<String>();

            foreach (var item in mymodel.Batch)
            {
                var batch_no = "FN" + item.BatchNumber.ToString().PadLeft(5,'0');
                mymodel.BatchNumber.Add(batch_no);
            }
            
            return new JsonResult(mymodel);
        }

    }
}