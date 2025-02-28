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
 
namespace Usine_Core.Controllers.CRM
{
     
    public class CRMCustomerGroupsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/CRMCustomerGroups/GetCustomerGroups")]
        public List<SalcustomerGroups> GetCustomerGroups([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(usr, 7, 1, 3, 0))
                {
                    return db.SalcustomerGroups.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMCustomerGroups/GetSalCustomerGroup")]
        public SalcustomerGroups GetSalCustomerGroup([FromBody] GeneralInformation inf)
        {
            try
            {
                return db.SalcustomerGroups.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }




        private Boolean dupCustomerGroupCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.SalcustomerGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.CustomerCode == inf.usr.cCode);
                    if (x == null)
                    {
                        b = true;
                    }
                    else
                    {
                        if (x.Count() == 0)
                        {
                            b = true;
                        }
                        else
                        {
                            b = false;
                        }
                    }
                    break;
                case 2:
                    var y = db.SalcustomerGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.RecordId != inf.recordId && a.CustomerCode == inf.usr.cCode);
                    if (y == null)
                    {
                        b = true;
                    }
                    else
                    {
                        if (y.Count() == 0)
                        {
                            b = true;
                        }
                        else
                        {
                            b = false;
                        }
                    }
                    break;
                case 3:
                    var z = db.PartyDetails.Where(a => a.PartyGroup == inf.recordId && a.PartyType == inf.detail && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (z == null)
                    {
                        b = true;
                    }

                    break;
            }
            return b;
        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMCustomerGroups/setCustomerGroup")]
        public salCustomerGroupsTotal setCustomerGroup([FromBody] salCustomerGroupsTotal tot)
        {
            String msg = "";
            GeneralInformation inf = new GeneralInformation();
            inf.recordId = tot.grp.RecordId;
            inf.traCheck = tot.traCheck;
            inf.detail = tot.grp.SGrp;
            inf.usr = tot.usr;
            AdminControl ac = new AdminControl();
            switch (tot.traCheck)
            {
                case 1:
                    if (ac.screenCheck(tot.usr, 7, 1, 3, 1))
                    {
                        if (dupCustomerGroupCheck(inf))
                        {

                            tot.grp.Chk = 1;
                            UsineContext dd = new UsineContext();
                            tot.grp.GroupCode = dd.SalcustomerGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GroupCode).FirstOrDefault();
                            tot.grp.GrpTag = dd.SalcustomerGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GrpTag).FirstOrDefault();
                            tot.grp.Statu = 1;
                            tot.grp.Branchid = tot.usr.bCode;
                            tot.grp.CustomerCode = tot.usr.cCode;
                            db.SalcustomerGroups.Add(tot.grp);
                            db.SaveChanges();
                            msg = "OK";
                        }
                        else
                        {
                            msg = "This Group name is already existed";
                        }
                    }
                    else
                    {
                        msg = "You are not authorised to create groups";
                    }
                    break;
                case 2:
                    if (ac.screenCheck(tot.usr, 7, 1, 3, 2))
                    {
                        if (dupCustomerGroupCheck(inf))
                        {
                            var grp = db.SalcustomerGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if (grp != null)
                            {
                                grp.SGrp = tot.grp.SGrp;
                            }
                            db.SaveChanges();
                            msg = "OK";
                        }
                        else
                        {
                            msg = "This Group name is already existed";
                        }
                    }
                    else
                    {
                        msg = "You are not authorised to modify groups";
                    }
                    break;
                case 3:
                    if (ac.screenCheck(tot.usr, 7, 1, 3, 3))
                    {
                        if (dupCustomerGroupCheck(inf))
                        {
                            var grp = db.SalcustomerGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            db.SalcustomerGroups.Remove(grp);
                            db.SaveChanges();
                            msg = "OK";
                        }
                        else
                        {
                            msg = "This Group name is in use";
                        }
                    }
                    else
                    {
                        msg = "You are not authorised to delete groups";
                    }
                    break;
            }


            tot.result = msg;
            return tot;
        }
        private int? findId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            try
            {
                x = db.PurSupplierGroups.Max(a => a.RecordId);
            }
            catch
            {

            }
            x++;
            return x;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMCustomerGroups/GetCustomerGroupsTreeView")]
        public List<PurSupplierGroupsTree> GetCustomerGroupsTreeView([FromBody] UserInfo usr)
        {
            List<PurSupplierGroupsTree> ll = new List<PurSupplierGroupsTree>();

            var groups = db.SalcustomerGroups.Where(a => a.MGrp == "MAIN" && a.CustomerCode == usr.cCode).OrderBy(b => b.GrpTag);
            foreach (SalcustomerGroups group in groups)
            {
                PurSupplierGroupsTree l = new PurSupplierGroupsTree();
                l.children = new List<PurSupplierGroupsTree>();
                l.mainGroup = group.MGrp;
                l.subGroup = group.SGrp;
                l.BuiltinType = group.Chk;
                l.groupcode = group.GroupCode;
                l.GroupTag = group.GrpTag;
                l.recordId = group.RecordId;
                getSuplierBranchView(l.subGroup, l.recordId, usr, l.children, 1);
                if (l != null)
                {
                    ll.Add(l);
                }
            }
            return ll;
        }
        private void getSuplierBranchView(String str, int? recordId, UserInfo usr, List<PurSupplierGroupsTree> ll, int x)
        {
            UsineContext db = new UsineContext();
            var dr1 = db.SalcustomerGroups.Where(a => a.MGrp == str && a.CustomerCode == usr.cCode);
            if (x == 2)
            {
                var accounts = db.FinAccounts.Where(a => a.Accgroup == recordId && a.CustomerCode == usr.cCode).OrderBy(b => b.Accname).ToList();
                foreach (FinAccounts account in accounts)
                {
                    PurSupplierGroupsTree l = new PurSupplierGroupsTree();
                    l.children = new List<PurSupplierGroupsTree>();
                    l.mainGroup = str;
                    l.subGroup = account.Accname;
                    l.BuiltinType = 1;
                    l.groupcode = "SPECIAL";
                    l.GroupTag = " ";
                    l.recordId = account.Recordid;


                    if (l != null)
                    {
                        ll.Add(l);
                    }
                }
            }
            foreach (SalcustomerGroups dr in dr1)
            {
                PurSupplierGroupsTree l = new PurSupplierGroupsTree();
                l.children = new List<PurSupplierGroupsTree>();
                l.mainGroup = dr.MGrp;
                l.subGroup = dr.SGrp;
                l.BuiltinType = dr.Chk;
                l.groupcode = dr.GroupCode;
                l.GroupTag = dr.GrpTag;
                l.recordId = dr.RecordId;

                getSuplierBranchView(l.subGroup, l.recordId, usr, l.children, x);
                if (l != null)
                {
                    ll.Add(l);
                }
            }

        }


    }
}
