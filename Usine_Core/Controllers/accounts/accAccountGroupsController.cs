using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Accounts
{
    public class accAccountGroupDetails
    {
        public int? recordID { get; set; }
        public string subGroup { get; set; }
        public string mainGroup { get; set; }
        public int? sno { get; set; }
        public int? chk { get; set; }
        public string statu { get; set; }
    }
    public class FinAccountGroupsTotal
    {
        public FinAccGroups grp { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }


    public class AccAccountGroupsTree
    {
        public int? recordId { get; set; }
        public string subGroup { get; set; }
        public String mainGroup { get; set; }
        public int? BuiltinType { get; set; }
        public string GroupTag { get; set; }
        public string groupcode { get; set; }
        public Boolean? opencheck { get; set; }

        public List<AccAccountGroupsTree> children { get; set; }
    }

    public class accAccountGroupsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Authorize]
        [Route("api/accAccountGroups/GetAccountGroups")]
        public IQueryable<FinAccGroups> GetAccountGroups([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(usr, 1, 1, 1, 0))
                {
                    return db.FinAccGroups.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.SGrp);
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
        [Route("api/accAccountGroups/GetAccTypeWiseAccountGroups")]
        public IQueryable<FinAccGroups> GetAccTypeWiseAccountGroups([FromBody] GeneralInformation inf)
        {
            try
            {
                return db.FinAccGroups.Where(a => a.GroupCode == inf.detail && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.SGrp);
            }
            catch
            {
                return null;
            }
        }
        [HttpPost]
        [Route("api/accAccountGroups/GetAccountGroup")]
        public FinAccGroups GetFinaccgroup([FromBody] GeneralInformation inf)
        {
            try
            {
                return db.FinAccGroups.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }




        private Boolean dupAccountGroupCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.FinAccGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.CustomerCode == inf.usr.cCode);
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
                    var y = db.FinAccGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.RecordId != inf.recordId && a.CustomerCode == inf.usr.cCode);
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
                    var z = db.FinAccounts.Where(a => a.Accgroup == inf.recordId && a.CustomerCode == inf.usr.cCode);
                    if (z == null)
                    {
                        b = true;
                    }
                    else
                    {
                        if (z.Count() == 0)
                        {
                            b = true;
                        }
                        else
                        {
                            b = false;
                        }
                    }
                    break;
            }
            return b;
        }


        [HttpPost]
        [Route("api/accAccountGroups/setAccountGroup")]
        public FinAccountGroupsTotal setAccountGroup([FromBody] FinAccountGroupsTotal tot)
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
                    if (ac.screenCheck(tot.usr, 1, 1, 1, 1))
                    {
                        if (dupAccountGroupCheck(inf))
                        {

                            tot.grp.Chk = 1;
                            UsineContext dd = new UsineContext();
                            tot.grp.GroupCode = dd.FinAccGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GroupCode).FirstOrDefault();
                            tot.grp.GrpTag = dd.FinAccGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GrpTag).FirstOrDefault();
                            tot.grp.Statu = 1;
                            tot.grp.Branchid = tot.usr.bCode;
                            tot.grp.CustomerCode = tot.usr.cCode;
                            db.FinAccGroups.Add(tot.grp);
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
                    if (ac.screenCheck(tot.usr, 1, 1, 1, 2))
                    {
                        if (dupAccountGroupCheck(inf))
                        {
                            var grp = db.FinAccGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
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
                    if (ac.screenCheck(tot.usr, 1, 1, 1, 3))
                    {
                        if (dupAccountGroupCheck(inf))
                        {
                            var grp = db.FinAccGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            db.FinAccGroups.Remove(grp);
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
                x = db.FinAccGroups.Max(a => a.RecordId);
            }
            catch
            {

            }
            x++;
            return x;
        }

        [HttpPost]
        [Route("api/accAccountGroups/GetAccountGroupsTreeView")]
        public List<AccAccountGroupsTree> GetAccountGroupsTreeView([FromBody] UserInfo usr)
        {
            List<AccAccountGroupsTree> ll = new List<AccAccountGroupsTree>();

            var groups = db.FinAccGroups.Where(a => a.MGrp == "MAIN" && a.CustomerCode == usr.cCode).OrderBy(b => b.GrpTag);
            foreach (FinAccGroups group in groups)
            {
                AccAccountGroupsTree l = new AccAccountGroupsTree();
                l.children = new List<AccAccountGroupsTree>();
                l.mainGroup = group.MGrp;
                l.subGroup = group.SGrp;
                l.BuiltinType = group.Chk;
                l.groupcode = group.GroupCode;
                l.GroupTag = group.GrpTag;
                l.recordId = group.RecordId;
                geAccountBranchView(l.subGroup, l.recordId, usr, l.children, 1);
                if (l != null)
                {
                    ll.Add(l);
                }
            }
            return ll;
        }
        private void geAccountBranchView(String str, int? recordId, UserInfo usr, List<AccAccountGroupsTree> ll, int x)
        {
            UsineContext db = new UsineContext();
            var dr1 = db.FinAccGroups.Where(a => a.MGrp == str && a.CustomerCode == usr.cCode);
            if (x == 2)
            {
                var accounts = db.FinAccounts.Where(a => a.Accgroup == recordId && a.CustomerCode == usr.cCode).OrderBy(b => b.Accname).ToList();
                foreach (FinAccounts account in accounts)
                {
                    AccAccountGroupsTree l = new AccAccountGroupsTree();
                    l.children = new List<AccAccountGroupsTree>();
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
            foreach (FinAccGroups dr in dr1)
            {
                AccAccountGroupsTree l = new AccAccountGroupsTree();
                l.children = new List<AccAccountGroupsTree>();
                l.mainGroup = dr.MGrp;
                l.subGroup = dr.SGrp;
                l.BuiltinType = dr.Chk;
                l.groupcode = dr.GroupCode;
                l.GroupTag = dr.GrpTag;
                l.recordId = dr.RecordId;

                geAccountBranchView(l.subGroup, l.recordId, usr, l.children, x);
                if (l != null)
                {
                    ll.Add(l);
                }
            }

        }

        [HttpPost]
        [Authorize]
        [Route("api/accAccountGroups/GetAccountTypeWiseTreeView")]
        public List<AccAccountGroupsTree> GetAccountTypeWiseTreeView([FromBody] GeneralInformation inf)
        {
            List<AccAccountGroupsTree> ll = new List<AccAccountGroupsTree>();

            var group = db.FinAccGroups.Where(a => a.SGrp == inf.detail && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            AccAccountGroupsTree l = new AccAccountGroupsTree();
            l.children = new List<AccAccountGroupsTree>();
            l.mainGroup = group.MGrp;
            l.subGroup = group.SGrp;
            l.BuiltinType = group.Chk;
            l.groupcode = group.GroupCode;
            l.GroupTag = group.GrpTag;
            l.recordId = group.RecordId;
            geAccountBranchView(l.subGroup, l.recordId, inf.usr, l.children, 2);
            if (l != null)
            {
                ll.Add(l);
            }


            /* foreach (Finaccgroups group in groups)
             {
                 AccAccountGroupsTree l = new AccAccountGroupsTree();
                 l.children = new List<AccAccountGroupsTree>();
                 l.mainGroup = group.MGrp;
                 l.subGroup = group.SGrp;
                 l.BuiltinType = group.Chk;
                 l.groupcode = group.GroupCode;
                 l.GroupTag = group.GrpTag;
                 l.recordId = group.RecordId;
                 geAccountBranchView(l.subGroup,l.recordId, inf.usr, l.children,2);
                 if (l != null)
                 {
                     ll.Add(l);
                 }
             }*/
            return ll;
        }
    }
}
