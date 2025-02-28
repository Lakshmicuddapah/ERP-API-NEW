using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Purchases
{
    public class purSupplierGroupDetails
    {
        public int? recordID { get; set; }
        public string subGroup { get; set; }
        public string mainGroup { get; set; }
        public int? sno { get; set; }
        public int? chk { get; set; }
        public string statu { get; set; }
    }
    public class purSupplierGroupsTotal
    {
        public PurSupplierGroups grp { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class salCustomerGroupsTotal
    {
      public SalcustomerGroups grp { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurSupplierGroupsTree
    {
        public int? recordId { get; set; }
        public string subGroup { get; set; }
        public String mainGroup { get; set; }
        public int? BuiltinType { get; set; }
        public string GroupTag { get; set; }
        public string groupcode { get; set; }
        public Boolean? opencheck { get; set; }

        public List<PurSupplierGroupsTree> children { get; set; }
    }

    public class PurSupplierGroupsController : ControllerBase
    {

        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/PurSupplierGroups/GetSupplierGroups")]
        public IQueryable<PurSupplierGroups> GetSupplierGroups([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(usr, 2, 1, 1, 0))
                {
                    return db.PurSupplierGroups.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.SGrp);
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
        [Route("api/PurSupplierGroups/GetPurSupplierGroup")]
        public PurSupplierGroups GetPurSupplierGroup([FromBody] GeneralInformation inf)
        {
            try
            {
                return db.PurSupplierGroups.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }




        private Boolean dupSupplierGroupCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.PurSupplierGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.CustomerCode == inf.usr.cCode);
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
                    var y = db.PurSupplierGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.RecordId != inf.recordId && a.CustomerCode == inf.usr.cCode);
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
                     var z = db.PartyDetails.Where(a => a.PartyGroup == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    var zz = db.PurSupplierGroups.Where(a => a.MGrp == inf.detail && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (z == null && zz==null)
                    {
                        b = true;
                    }
                     
                    break;
            }
            return b;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurSupplierGroups/setSupplierGroup")]
        public purSupplierGroupsTotal setSupplierGroup([FromBody] purSupplierGroupsTotal tot)
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
                    if (ac.screenCheck(tot.usr, 2, 1, 1, 1))
                    {
                        if (dupSupplierGroupCheck(inf))
                        {

                            tot.grp.Chk = 1;
                            UsineContext dd = new UsineContext();
                            tot.grp.GroupCode = dd.PurSupplierGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GroupCode).FirstOrDefault();
                            tot.grp.GrpTag = dd.PurSupplierGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GrpTag).FirstOrDefault();
                            tot.grp.Statu = 1;
                            tot.grp.Branchid = tot.usr.bCode;
                            tot.grp.CustomerCode = tot.usr.cCode;
                            db.PurSupplierGroups.Add(tot.grp);
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
                    if (ac.screenCheck(tot.usr, 2, 1, 1, 2))
                    {
                        string grpname = "";
                        var grp = db.PurSupplierGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                        if (grp != null)
                        {
                            grpname = grp.SGrp;
                            grp.SGrp = tot.grp.SGrp;
                        }
                        inf.detail = grpname;
                        if (dupSupplierGroupCheck(inf))
                        {
                           
                           
                            var grps = db.PurSupplierGroups.Where(a => a.MGrp == grpname && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                            foreach(var gr in grps)
                            {
                                gr.MGrp = tot.grp.SGrp;
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
                    if (ac.screenCheck(tot.usr, 2, 1, 1, 3))
                    {
                        if (dupSupplierGroupCheck(inf))
                        {
                            var grp = db.PurSupplierGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            db.PurSupplierGroups.Remove(grp);
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
     

        [HttpPost]
        [Authorize]
        [Route("api/PurSupplierGroups/GetSupplierGroupsTreeView")]
        public List<PurSupplierGroupsTree> GetSupplierGroupsTreeView([FromBody] UserInfo usr)
        {
            List<PurSupplierGroupsTree> ll = new List<PurSupplierGroupsTree>();

            var groups = db.PurSupplierGroups.Where(a => a.MGrp == "MAIN" && a.CustomerCode == usr.cCode).OrderBy(b => b.GrpTag);
            foreach (PurSupplierGroups group in groups)
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
            var dr1 = db.PurSupplierGroups.Where(a => a.MGrp == str && a.CustomerCode == usr.cCode);
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
            foreach (PurSupplierGroups dr in dr1)
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