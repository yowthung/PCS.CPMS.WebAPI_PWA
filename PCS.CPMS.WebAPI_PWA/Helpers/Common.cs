using Microsoft.EntityFrameworkCore;
using PCS.CPMS.BL.Models;
using PCS.CPMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCS.CPMS.WebAPI_PWA.Helpers
{
    public class Common
    {
        private readonly tng_cpmsContext _context;
        public Common()
        {
        }


        public Common(tng_cpmsContext context)
        {
            _context = context;
        }


        internal async Task<User> GetUserLogin(string useremail, string pwd)
        {
           
            return await _context.User.FirstOrDefaultAsync(u => u.Email == useremail && u.Pwd == pwd);
        }


    }
}
