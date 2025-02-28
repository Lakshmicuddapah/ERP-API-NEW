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

namespace Usine_Core.Controllers.crm
{
    public class TeamDetails
    {
        public long? empno { get; set; }
    }
    public class CRMGeneral
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        
        public List<CrmSaleOrderUni> EmployeeDirectBusiness(GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            StringConversions sc = new StringConversions();
            string str = sc.makeStringToAscii(inf.usr.uCode.ToLower());
            var empno = db.UserCompleteProfile.Where(a => a.UsrName == str && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.EmployeeNo).FirstOrDefault();
            List<int?> parties = new List<int?>();
            if (empno != null)
            {
                parties = db.PartyDetails.Where(a => a.Employee == empno && a.PartyType == "CUS" && a.CustomerCode == inf.usr.cCode).Select(b => b.RecordId).ToList();
            }
            if(parties != null)
            {
                return (from a in db.CrmSaleOrderUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.Pos==2 && parties.Contains(a.PartyId) && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        join b in db.PartyDetails.Where(a => a.CustomerCode == inf.usr.cCode) on a.PartyId equals b.RecordId
                        join c in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Employee equals c.RecordId
                        select new CrmSaleOrderUni
                        {
                            RecordId=a.RecordId,
                            Seq=a.Seq,
                            Dat=a.Dat,
                            PartyId=a.PartyId,
                            PartyName=a.PartyName,
                            Mobile=a.Mobile,
                            Email=c.Empname,
                            Baseamt=a.Baseamt,
                            Discount=a.Discount,
                            Taxes=a.Taxes,
                            Others=a.Others,
                            TotalAmt=a.TotalAmt

                        }).OrderBy(b => b.Dat).ToList();
            }
            else
            {
                return null;
            }
        }
        public List<CrmSaleOrderUni> EmployeeTeamBusiness(GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            StringConversions sc = new StringConversions();
            string str = sc.makeStringToAscii(inf.usr.uCode.ToLower());
            var empno = db.UserCompleteProfile.Where(a => a.UsrName == str && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.EmployeeNo).FirstOrDefault();
            List<long?> team = new List<long?>();
            List<TeamDetails> teamemps = new List<TeamDetails>();
            findTeam(empno, inf.usr.bCode, inf.usr.cCode,teamemps);
            foreach (var emp in teamemps)
            {
                team.Add(emp.empno);
            }
            List<int?> parties = db.PartyDetails.Where(a => team.Contains(a.Employee) && a.PartyType == "CUS" && a.CustomerCode == inf.usr.cCode).Select(b => b.RecordId).ToList();
   
            if (parties.Count() > 0)
            {
                return (from a in db.CrmSaleOrderUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.Pos == 2 && parties.Contains(a.PartyId) && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                        join b in db.PartyDetails.Where(a => a.CustomerCode == inf.usr.cCode) on a.PartyId equals b.RecordId
                        join c in db.HrdEmployees.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on b.Employee equals c.RecordId
                        select new CrmSaleOrderUni
                        {
                            RecordId = a.RecordId,
                            Seq = a.Seq,
                            Dat = a.Dat,
                            PartyId = a.PartyId,
                            PartyName = a.PartyName,
                            Mobile = a.Mobile,
                            Email = c.Empname,
                            Baseamt = a.Baseamt,
                            Discount = a.Discount,
                            Taxes = a.Taxes,
                            Others = a.Others,
                            TotalAmt = a.TotalAmt

                        }).OrderBy(b => b.Dat).ToList();
            }
            else
            {
                return null;
            }
        }
        public void findTeam(long? empno,string bCode,int? cCode, List<TeamDetails> team)
        {
            List<long?> emps = new List<long?>();
            if(empno != null)
             emps = db.HrdEmployees.Where(a => a.Mgr == empno && a.Branchid == bCode && a.CustomerCode == cCode).Select(b => b.RecordId).ToList();
            if (emps == null)
            {
                return;
            }
            else
            {
                if(emps.Count() > 0)
                {
                    
                   
                    foreach(var emp in emps)
                    {
                        team.Add(new TeamDetails
                        {
                            empno=emp
                        });
                        findTeam(emp, bCode, cCode, team);
                    }
                }
                    
            }
                    
               
            }


        public List<CrmEnquiriesRx> GetEmployeeDirectCalls([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            return db.CrmEnquiriesRx.Where(a => a.PrevcallId == null && a.Dat >= dat1 && a.Dat < dat2 && a.username == inf.usr.uCode && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
        }
        public List<CrmEnquiriesRx> GetEmployeeTeamCalls([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            StringConversions sc = new StringConversions();
            string str = sc.makeStringToAscii(inf.usr.uCode.ToLower());
            var empno = db.UserCompleteProfile.Where(a => a.UsrName == str && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.EmployeeNo).FirstOrDefault();
            List<long?> team = new List<long?>();
            List<TeamDetails> teamemps = new List<TeamDetails>();
            findTeam(empno, inf.usr.bCode, inf.usr.cCode, teamemps);
            foreach (var emp in teamemps)
            {
                team.Add(emp.empno);
            }
            var usercodes = db.UserCompleteProfile.Where(a => team.Contains(a.EmployeeNo) && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.UsrName).ToList();
            List<string> users = new List<string>();
            foreach(var cod in usercodes)
            {
                users.Add(sc.makeAsciitoString(cod).ToUpper());
            }
            return db.CrmEnquiriesRx.Where(a => a.PrevcallId == null && a.Dat >= dat1 && a.Dat < dat2 && users.Contains(a.username.ToUpper()) && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();

        }

    }
}
