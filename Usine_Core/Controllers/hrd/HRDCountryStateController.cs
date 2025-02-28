using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.HRD;
using Usine_Core.Models;

namespace Usine_Core.Controllers.hrd
{
    public class HRDCountryStateRequirements
    {
        public List<MisCountryMaster> countries { get; set; }
        public List<MisStateMaster> states { get; set; }
    }
    public class HRDCountryStateController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [Route("api/HRDCountryState/HRDCountryStateRequirements")]
        public HRDCountryStateRequirements HRDCountryStateRequirements([FromBody] UserInfo usr)
        {
            hrdDepartmentsController hr = new hrdDepartmentsController();
            HRDCountryStateRequirements tot = new HRDCountryStateRequirements();

            tot.countries = db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode).ToList();
            tot.states = db.MisStateMaster.Select(x => new MisStateMaster
            {
                StateName = x.StateName,
                CustomerCode = x.CustomerCode,
                Cntname = x.Cntname,
                RecordId = x.RecordId

            }).Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.StateName).ToList();
            return tot;
        }
    }
}
