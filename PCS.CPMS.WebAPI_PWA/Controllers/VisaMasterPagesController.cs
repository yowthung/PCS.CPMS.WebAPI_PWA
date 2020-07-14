using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
//using System.Web.Http;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
 
    [Authorize]
    [Microsoft.AspNetCore.Components.Route("api/{controller}")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class VisaMasterPagesController : Controller
    {
        //add database
        private readonly tng_cpmsContext _db;
        private static readonly HttpClient client = new HttpClient();

        public VisaMasterPagesController(tng_cpmsContext db)
        {
            _db = db;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("api/VisaMasterPages/VisaMasterTransactionTable")]
        [ActionName("api/VisaMasterPages/VisaMasterTransactionTable")]
        public IActionResult VisaMasterTransactionTable()
        {
            var siteList = new List<int>();
            siteList.Add(4);
            siteList.Add(6);

            dynamic mymodel = new ExpandoObject();
            mymodel.SPID = _db.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount > 0).Select(p => p.Spid).Distinct().ToList();
            mymodel.EntryLane = _db.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount > 0).Select(p => p.EntryLane).Distinct().ToList();
            mymodel.ExitLane = _db.Transaction.Where(p => p.SuccessFlag == "Y" && p.CardTypeTag == "E" && p.TransactionAmount > 0).Select(p => p.ExitLane).Distinct().ToList();
            mymodel.Site = new List<Site>();

            foreach (var item in siteList)
            {
                mymodel.Site.Add(_db.Site.Where(s => s.ID == item).FirstOrDefault());
            }

            return View(mymodel);
        }


    }
}