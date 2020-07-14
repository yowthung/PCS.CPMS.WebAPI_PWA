using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PCS.CPMS.DAL;
using PCS.CPMS.BL.Models;
using Microsoft.AspNetCore.Authorization;

namespace PCS.CPMS.WebAPI_PWA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // [EnableCors(origins: "http://mywebclient.azurewebsites.net", headers: "*", methods: "*")]
    public class ContactsController : ControllerBase
    {

        private readonly tng_cpmsContext _context;

        public ContactsController(tng_cpmsContext context)
        {
            _context = context;
        }
        //GET: Contacts <List>
        public ActionResult Index()
        {
            List<Contact> model = _context.Contact.ToList();
            return new JsonResult(model);
        }

        [HttpPost("{draw}/{start}/{length}/{sortColumn}/{sortColumnDirection}/{searchValue}"), Route("loaddata")]

        //POST: All Contacts(JSON) from table
        public IActionResult LoadData(string draw, string start, string length, string sortColumn, string sortColumnDirection, string searchValue)
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

                // Getting all Customer data  
                var contactData = (from tempContact in _context.Contact select tempContact);

                //Sorting  
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    contactData = contactData.OrderBy(x => sortColumn + " " + sortColumnDirection);
                }
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    contactData = contactData.Where(m => m.ContactName.IndexOf(searchValue) > -1);
                }

                //total number of rows count   
                recordsTotal = contactData.Count();
                //Paging   
                var data = contactData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("{id}"), Route("viewcontacttomodal")]
       //POST: find id and return partial view
public IActionResult ViewContactToModal(int id)
            {
            Contact contact = _context.Contact.Find(id);
            if (contact == null)
            {
                return NotFound();
            }
            return new JsonResult(contact);
        }

        //POST: update contact 
        public IActionResult UpdateContact(int id, string name, string phone, string title, string companyname, string companyaddress)
        {
            Contact contact = _context.Contact.Where(i => i.Id.Equals(id)).FirstOrDefault();
            contact.ContactName = name;
            contact.ContactPhone = phone;
            contact.ContactTitle = title;
            contact.ContactCompany = companyname;
            contact.ContactCompanyAddress = companyaddress;

            _context.SaveChanges();

            return Ok();
        }

        private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id);
        }
    }
}