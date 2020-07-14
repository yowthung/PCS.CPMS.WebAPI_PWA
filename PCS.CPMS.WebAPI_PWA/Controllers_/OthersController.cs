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
    public class OthersController : ControllerBase
    {

        private readonly tng_cpmsContext _context;

        public OthersController(tng_cpmsContext context)
        {
            _context = context;
        }

        [HttpGet, Route("Site")]
        public IActionResult Site()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Site = _context.Site.OrderBy(s => s.LocationId).ToList();

            return new JsonResult(mymodel);
        }
    }
}


