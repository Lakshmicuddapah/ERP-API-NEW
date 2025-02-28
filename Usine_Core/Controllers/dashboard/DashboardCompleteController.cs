using System;
using System.Collections.Generic;
 using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;

namespace Usine_Core.Controllers.Dashboard
{
     
    public class DashboardCompleteController : ControllerBase
    {
        UsineContext db = new UsineContext();
        StringConversions sc = new StringConversions();

        [HttpPost]
        [Authorize]
        [Route("api/DashboardComplete/GetCompleteUsersList")]
        public List<UsrAut> GetCompleteUsersList([FromBody] UserInfo usr)
        {
            
            return (from a in db.UsrAut.Where(a => a.CustomerCode == usr.cCode)
                       select new UsrAut
                       {
                           UsrName=a.UsrName,
                           RoleName=sc.makeAsciitoString(a.UsrName),
                       }).OrderBy(b => b.UsrName).ToList();
        }
    }
}
