using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;

namespace Usine_Core.Controllers.HRD
{

    public class hrdDepartmentGroupDetails
    {
        public int? recordID { get; set; }
        public string subGroup { get; set; }
        public string mainGroup { get; set; }
        public int? sno { get; set; }
        public int? chk { get; set; }
        public string statu { get; set; }
    }
    public class hrdDepartmentGroupsTotal
    {
        public HrdDepartments dept { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class Employees
    {
        public int? empid { get; set; }
        public String empname { get; set; }
        public String department { get; set; }
    }


    public class HrdDepartmentGroupsTree
    {
        public int? recordId { get; set; }
        public string subGroup { get; set; }
        public String mainGroup { get; set; }
        public int? BuiltinType { get; set; }
        public string GroupTag { get; set; }
        public string groupcode { get; set; }
        public Boolean? opencheck { get; set; }

        public List<HrdDepartmentGroupsTree> children { get; set; }
    }
    public class hrdDepartmentsController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Route("api/hrdDepartments/GetDepartments")]
        [Authorize]
        public List<HrdDepartments> GetHrdDepartments([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 8, 1, 1, 0))
                {
                return db.HrdDepartments.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Sno).ToList();
            }
            else
            {
                return null;
            }
        }
       

        private Boolean dupHRDDepartmentCheck(GeneralInformation inf)
        {
            Boolean b = false;
            switch (inf.traCheck)
            {
                case 1:
                    var x = db.HrdDepartments.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.CustomerCode == inf.usr.cCode);
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
                    var y = db.HrdDepartments.Where(a => a.SGrp.ToUpper() == inf.detail.ToUpper() && a.RecordId != inf.recordId && a.CustomerCode == inf.usr.cCode);
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
                     
                    b = true;
                    break;
            }
            return b;
        }


        [HttpPost]
        [Route("api/hrdDepartments/setHrdDepartment")]
        [Authorize]
        public hrdDepartmentGroupsTotal setHrdDepartment([FromBody] hrdDepartmentGroupsTotal tot)
        {
            String msg = "";
            GeneralInformation inf = new GeneralInformation();
            inf.recordId = tot.dept.RecordId;
            inf.traCheck = tot.traCheck;
            inf.detail = tot.dept.SGrp;
            inf.usr = tot.usr;
            AdminControl ac = new AdminControl();
            switch (tot.traCheck)
            {
                case 1:
                    if (ac.screenCheck(tot.usr, 8, 1, 1, 1))
                    {
                        if (dupHRDDepartmentCheck(inf))
                        {

                            tot.dept.Chk = 1;
                            UsineContext dd = new UsineContext();
                            tot.dept.GroupCode = dd.HrdDepartments.Where(a => a.SGrp == tot.dept.MGrp && a.CustomerCode == tot.usr.cCode).Select(b => b.GroupCode).FirstOrDefault();
                            tot.dept.Statu = 1;
                            tot.dept.Branchid = tot.usr.bCode;
                            tot.dept.CustomerCode = tot.usr.cCode;
                            db.HrdDepartments.Add(tot.dept);
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
                    if (ac.screenCheck(tot.usr, 8, 1, 1, 2))
                    {
                        if (dupHRDDepartmentCheck(inf))
                        {
                            var grp = db.HrdDepartments.Where(a => a.RecordId == tot.dept.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if (grp != null)
                            {
                                grp.SGrp = tot.dept.SGrp;
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
                    if (ac.screenCheck(tot.usr, 8, 1, 1, 3))
                    {
                        if (dupHRDDepartmentCheck(inf))
                        {
                            var grp = db.HrdDepartments.Where(a => a.RecordId == tot.dept.RecordId && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if (grp != null)
                            {
                                db.HrdDepartments.Remove(grp);
                            }
                            db.SaveChanges();
                            msg = "OK";
                        }
                        else
                        {
                            msg = "This Group is in useralready existed";
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
        [Route("api/hrdDepartments/GetHrdDepartmentsTreeView")]
        [Authorize]
        public List<HrdDepartmentGroupsTree> GetHrdDepartmentsTreeView([FromBody] UserInfo usr)
        {
            List<HrdDepartmentGroupsTree> ll = new List<HrdDepartmentGroupsTree>();

            var groups = db.HrdDepartments.Where(a => a.MGrp == "Departments" && a.CustomerCode == usr.cCode).OrderBy(b => b.Sno);
            foreach (HrdDepartments group in groups)
            {
                HrdDepartmentGroupsTree l = new HrdDepartmentGroupsTree();
                l.children = new List<HrdDepartmentGroupsTree>();
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
        private void geAccountBranchView(String str, int? recordId, UserInfo usr, List<HrdDepartmentGroupsTree> ll, int x)
        {
            UsineContext db = new UsineContext();
            var dr1 = db.HrdDepartments.Where(a => a.MGrp == str && a.CustomerCode == usr.cCode);
           
            foreach (HrdDepartments dr in dr1)
            {
                HrdDepartmentGroupsTree l = new HrdDepartmentGroupsTree();
                l.children = new List<HrdDepartmentGroupsTree>();
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

      
        public List<Employees> getEmployeesByDepartment(GeneralInformation inf)
        {
            return null;
        }

    }
}
