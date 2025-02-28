using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;


namespace Usine_Core.Controllers.Inventory
{
    public class invItemGroupDetails
    {
        public int? recordID { get; set; }
        public string subGroup { get; set; }
        public string mainGroup { get; set; }
        public int? sno { get; set; }
        public int? chk { get; set; }
        public string statu { get; set; }
    }
    public class InvItemGroupsTotal
    {
        public InvGroups grp { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }


    public class InvItemGroupsTree
    {
        public int? recordId { get; set; }
        public string subGroup { get; set; }
        public String mainGroup { get; set; }
        public int? BuiltinType { get; set; }
        public string GroupTag { get; set; }
        public string groupcode { get; set; }
        public Boolean? opencheck { get; set; }

        public List<InvItemGroupsTree> children { get; set; }
    }

    public class invGroupsController : Controller
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Authorize]
        [Route("api/invgroups/GetInvItemGroups")]
        public IQueryable<InvGroups> GetInvItemGroups([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                return db.InvGroups.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.SGrp);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/invgroups/GetInvItemGroup")]
        public InvGroups GetInvItemGroup([FromBody] GeneralInformation inf)
        {
            try
            {
                return db.InvGroups.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }




        [HttpPost]
        [Route("api/invgroups/setInventoryGroup")]
        public TransactionResult setInventoryGroup([FromBody] InvItemGroupsTotal tot)
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
                    if (ac.screenCheck(tot.usr, 3, 1, 3, 1))
                    {
                        if (dupCheck(inf))
                        {

                            tot.grp.Chk = 1;
                            UsineContext dd = new UsineContext();
                            tot.grp.GroupCode = dd.InvGroups.Where(a => a.SGrp == tot.grp.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GroupCode).FirstOrDefault();
                            tot.grp.Statu = 1;
                            tot.grp.Branchid = tot.usr.bCode;
                            tot.grp.CustomerCode = tot.usr.cCode;
                            db.InvGroups.Add(tot.grp);
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
                    if (ac.screenCheck(tot.usr, 3, 1, 3, 2))
                    {
                        if (dupCheck(inf))
                        {
                            var grp = db.InvGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
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
                    if (ac.screenCheck(tot.usr, 3, 1, 3, 3))
                    {
                        if (dupCheck(inf))
                        {
                            var grp = db.InvGroups.Where(a => a.RecordId == tot.grp.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            db.InvGroups.Remove(grp);
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

            TransactionResult res = new TransactionResult();
            res.result = msg;
            return res;
        }












        private Boolean dupCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.InvGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.CustomerCode == inf.usr.cCode);
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
                    var y = db.InvGroups.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.RecordId != inf.recordId && a.CustomerCode == inf.usr.cCode);
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
                    var z = db.InvMaterials.Where(a => a.Grp == inf.recordId && a.CustomerCode == inf.usr.cCode);
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
        [Authorize]
        [Route("api/invgroups/GetInvGroupsTreeView")]
        public List<InvItemGroupsTree> GetInvGroupsTreeView([FromBody] UserInfo usr)
        {
            List<InvItemGroupsTree> ll = new List<InvItemGroupsTree>();

            var groups = db.InvGroups.Where(a => a.MGrp == "MATERIALS" && a.CustomerCode == usr.cCode).OrderBy(b => b.GrpTag);
            foreach (InvGroups group in groups)
            {
                InvItemGroupsTree l = new InvItemGroupsTree();
                l.children = new List<InvItemGroupsTree>();
                l.mainGroup = group.MGrp;
                l.subGroup = group.SGrp;
                l.BuiltinType = group.Chk;
                l.groupcode = group.GroupCode;
                l.GroupTag = group.GrpTag;
                l.recordId = group.RecordId;
                geInventoryBranchView(l.subGroup, l.recordId, usr, l.children, 1);
                if (l != null)
                {
                    ll.Add(l);
                }
            }
            return ll;
        }
        private void geInventoryBranchView(String str, int? recordId, UserInfo usr, List<InvItemGroupsTree> ll, int x)
        {
            UsineContext db = new UsineContext();
            var dr1 = db.InvGroups.Where(a => a.MGrp == str && a.CustomerCode == usr.cCode).OrderBy(c => c.SGrp);
            if (x == 2)
            {
                var accounts = db.InvMaterials.Where(a => a.Grp == recordId && a.CustomerCode == usr.cCode).OrderBy(b => b.ItemName).ToList();
                foreach (InvMaterials account in accounts)
                {
                    InvItemGroupsTree l = new InvItemGroupsTree();
                    l.children = new List<InvItemGroupsTree>();
                    l.mainGroup = str;
                    l.subGroup = account.ItemName;
                    l.BuiltinType = 1;
                    l.groupcode = "SPECIAL";
                    l.GroupTag = " ";
                    l.recordId = account.RecordId;


                    if (l != null)
                    {
                        ll.Add(l);
                    }
                }
            }
            foreach (InvGroups dr in dr1)
            {
                InvItemGroupsTree l = new InvItemGroupsTree();
                l.children = new List<InvItemGroupsTree>();
                l.mainGroup = dr.MGrp;
                l.subGroup = dr.SGrp;
                l.BuiltinType = dr.Chk;
                l.groupcode = dr.GroupCode;
                l.GroupTag = dr.GrpTag;
                l.recordId = dr.RecordId;

                geInventoryBranchView(l.subGroup, l.recordId, usr, l.children, x);
                if (l != null)
                {
                    ll.Add(l);
                }
            }

        }



        [HttpPost]
        [Authorize]
        [Route("api/invgroups/GetInventoryTypeWiseTreeView")]
        public List<InvItemGroupsTree> GetInventoryTypeWiseTreeView([FromBody] GeneralInformation inf)
        {
            List<InvItemGroupsTree> ll = new List<InvItemGroupsTree>();

            var groups = db.InvGroups.Where(a => a.MGrp == inf.detail && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.GrpTag);
            foreach (InvGroups group in groups)
            {
                InvItemGroupsTree l = new InvItemGroupsTree();
                l.children = new List<InvItemGroupsTree>();
                l.mainGroup = group.MGrp;
                l.subGroup = group.SGrp;
                l.BuiltinType = group.Chk;
                l.groupcode = group.GroupCode;
                l.GroupTag = group.GrpTag;
                l.recordId = group.RecordId;
                geInventoryBranchView(l.subGroup, l.recordId, inf.usr, l.children, 2);
                if (l != null)
                {
                    ll.Add(l);
                }
            }
            return ll;
        }




    }
}
