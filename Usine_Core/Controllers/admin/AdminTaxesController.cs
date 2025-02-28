using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Admin
{
    public class AdmTaxesTotal
    {
        public AdmTaxes tax { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }

    public class AdminTaxesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/taxes/GetTaxes")]
        public List<AdmTaxes> GetTaxes([FromBody] UserInfo usr)
        {
            return db.AdmTaxes.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/taxes/setTax")]
        public AdmTaxesTotal setTax([FromBody] AdmTaxesTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();

            try
            {
                if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
                {
                    var x = db.AdmTaxes.Where(a => a.TaxCode.ToUpper() == tot.tax.TaxCode.ToUpper() && a.TaxPer == tot.tax.TaxPer && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if (x == null)
                    {
                        tot.tax.BranchId = tot.usr.bCode;
                        tot.tax.CustomerCode = tot.usr.cCode;
                        db.AdmTaxes.Add(tot.tax);
                        db.SaveChanges();
                        msg = "OK";
                    }
                    else
                    {
                        msg = "These details are already existed";
                    }
                }
                else
                {
                    msg = "You are not authorised to create taxes";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }

    }
}
