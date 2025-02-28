
using System;
using System.Collections.Generic;
using Usine_Core.Controllers.Purchases;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;

namespace Usine_Core.Controllers.CRM
{
    
     public class CRMTargetSettingRequirements
    {
        public int? empno { get; set; }
        public List<HrdEmployees> employees { get; set; }       
        public int? detail { get; set; }
        public int? yea { get; set; }
        public int? targetType{ get; set; }
        public int? brandType { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMTargetSettingTotal
    {
        public List<CrmTargetSettings> targets { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CRMTargetSettingsController : ControllerBase
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/CRMTargetSettings/GetCRMTargetRequirements")]
        public CRMTargetSettingRequirements GetCRMTargetRequirements([FromBody] UserInfo usr)
        {
            StringConversions sc = new StringConversions();
            var dets = db.CrmSetup.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            General g = new General();
            int targettype = -100, brandtype = -100;
            foreach (var de in dets)
            {
                switch (de.SetupCode)
                {
                    case "crm_cyl":
                        targettype = g.valInt(de.Pos);
                        break;
                    case "crm_brd":
                        brandtype = g.valInt(de.Pos);
                        break;
                }
            }
            CRMTargetSettingRequirements result = new CRMTargetSettingRequirements();
            result.targetType = targettype;
            result.brandType = brandtype;
            var username = sc.makeStringToAscii(usr.uCode.ToLower());
            var det = db.UserCompleteProfile.Where(a => a.UsrName == username && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
            if (det != null)
            {
                var empno = det.EmployeeNo;
                if (empno != null)
                {
                    result.employees = (from a in db.HrdEmployees.Where(a => a.Mgr == empno && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                        join b in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.Designation equals b.RecordId
                                        select new HrdEmployees
                                        {
                                            RecordId = a.RecordId,
                                            Empname = a.Empname,
                                            Empno = a.Empno,
                                            City = b.Designation
                                        }).OrderBy(b => b.Empname).ToList();
                }
            }
            return result;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMTargetSettings/GetCRMTargetDetails")]
        public dynamic GetCRMTargetDetails([FromBody] CRMTargetSettingRequirements det)
        {
            List<int?> months = new List<int?>();
            var details = db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == det.usr.cCode).ToList();
            List<InvMaterialCompleteDetailsView> items = new List<InvMaterialCompleteDetailsView>();
            switch (det.brandType)
            {
                case 1:
                    items = details;
                    break;
                case 2:
                    items = (from a in details.Select(a => new { a.Grpid, a.Grpname }).Distinct()
                             select new InvMaterialCompleteDetailsView
                             {
                                 Grpid = a.Grpid,
                                 Grpname = a.Grpname,
                             }).ToList();
                    break;
                case 3:
                    items.Add(new InvMaterialCompleteDetailsView
                    {
                        Grpid = 0,
                        Grpname = "Total"
                    });
                    break;
            }
            var year = det.yea;

            switch (det.targetType)
            {
                case 1:
                    months.Add(det.detail);
                    break;
                case 2:
                    var num = (det.detail - 1) * 3;
                    months.Add(num + 1);
                    months.Add(num + 2);
                    months.Add(num + 3);
                    if(det.detail ==1)
                    {
                        year++;         
                    }
                    break;
                case 3:
                    var num1 = (det.detail - 1) * 6;
                    var i = 1;
                    while (i <= 6)
                    {
                        months.Add(num1 + i);
                        i++;
                    }
                    break;
                case 4:

                    var j = 1;
                    while (j <= 12)
                    {
                        months.Add(j);
                        j++;
                    }
                    break;

            }

            var targets = (from a in db.CrmTargetSettings.Where(a => a.Empno == det.empno && a.Yea == year && months.Contains(a.Mont)).GroupBy(a => new { a.Empno, a.CategoryId, a.ProductId })
                           select new CrmTargetSettings
                           {
                               Empno = a.Key.Empno,
                               CategoryId = a.Key.CategoryId,
                               ProductId = a.Key.ProductId,
                               Tgt = a.Sum(b => b.Tgt),
                               Calls = a.Sum(b => b.Calls)
                           }).ToList();
            if (det.brandType == 1)
            {


                return (from a in items join b in targets
                                    on new { grpid = a.Grpid, prodid = a.RecordId } equals new { grpid = b.CategoryId, prodid = b.ProductId } into gj
                        from subdet in gj.DefaultIfEmpty()
                        select new
                        {
                            categoryid = a.Grpid,
                            category = a.Grpname,
                            itemid = a.RecordId,
                            itemname = a.Itemname,
                            target = subdet == null ? 0 : subdet.Tgt,
                            calls = subdet == null ? 0 : subdet.Calls,
                            slno = subdet == null ? -1 : subdet.Slno

                        }) ;

            }
            else
            {
                return (from a in items
                        join b in targets
              on a.Grpid equals b.CategoryId into gj
                        from subdet in gj.DefaultIfEmpty()
                        select new
                        {
                            categoryid = a.Grpid,
                            category = a.Grpname,
                            target = subdet == null ? 0 : subdet.Tgt,
                            calls = subdet == null ? 0 : subdet.Calls,
                              slno = subdet == null ? -1 : subdet.Slno
                        });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMTargetSettings/SetCRMTarget")]
        public TransactionResult SetCRMTarget([FromBody] CRMTargetSettingTotal tot)
        {
            AdminControl ac = new AdminControl();
            string msg = "";
            if (ac.screenCheck(tot.usr, 7, 2, 10, 0))
            {

                try
                {
                    if(tot.targets.Count > 0)
                    {
                        /* var slnos=tot.targets.Select(a => a.Slno).ToList();
                         if(tot.targets[0].Slno > 0)
                         {
                             var details = db.CrmTargetSettings.Where(a => a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode && slnos.Contains(a.Slno)).ToList();
                             if(details.Count() > 0)
                             {
                                 db.CrmTargetSettings.RemoveRange(details);
                             }
                         }*/

                        var months = tot.targets.Select(b => b.Mont);
                        var years = tot.targets.Select(b => b.Yea);

                        var details = db.CrmTargetSettings.Where(a => a.Empno == tot.targets[0].Empno && months.Contains(a.Mont) && years.Contains(a.Yea) && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                        if (details.Count() > 0)
                        {
                            db.CrmTargetSettings.RemoveRange(details);
                        }
                    }
                    
                    foreach(var det in tot.targets)
                    {
                        det.Dat = ac.getPresentDateTime();
                        det.BranchId = tot.usr.bCode;
                        det.CustomerCode = tot.usr.cCode;
                        det.Slno = null;
                    }
                    db.CrmTargetSettings.AddRange(tot.targets);
                    db.SaveChanges();
                    msg = "OK";
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
    }
}
