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
using System.Data.SqlClient;
using Usine_Core.Controllers.CRM;

namespace Usine_Core.Controllers.crm
{
    public class DashBoardDetails
    {
        public int? sno { get; set; }
        public string descr { get; set; }   
        public double? fir { get; set; }
        public double? sec { get; set; }
        public int? typ { get; set; }
    }
    
    public class CRMDashboardController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMDashboard/GetCRMDashboard")]
        public List<DashBoardDetails> GetCRMDashboard([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(usr, 7, 0, 0, 1))
                {
                    CRMGeneral cg = new CRMGeneral();
                    General gg = new General();
                    DateTime dat = ac.getPresentDateTime();
                    string dat1 = gg.strDate( gg.monthStart(dat));
                    string dat2 = gg.strDate( gg.monthEnd(dat));
                    GeneralInformation inf = new GeneralInformation();
                    inf.frmDate = dat1;
                    inf.toDate = dat2;
                    inf.usr = usr;

                    StringConversions sc = new StringConversions();
                    var user = sc.makeStringToAscii(usr.uCode.ToLower());
                    var empno = db.UserCompleteProfile.Where(a => a.UsrName == user && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Select(b => b.EmployeeNo).FirstOrDefault();


                    List<DashBoardDetails> lst = new List<DashBoardDetails>();
                    lst.Add(new DashBoardDetails
                    {
                        sno=1,
                        descr="Target achievement details",
                        fir= cg.EmployeeDirectBusiness(inf).Sum(b => b.Baseamt-b.Discount),
                        sec=cg.EmployeeTeamBusiness(inf).Sum(b => b.Baseamt-b.Discount),
                        typ=1
                    });

                    lst.Add(new DashBoardDetails
                    {
                        sno = 2,
                        descr = "Calls achievement details",
                        fir = cg.GetEmployeeDirectCalls(inf).Count(),
                        sec = cg.GetEmployeeTeamCalls(inf).Count(),
                        typ = 2
                    });
                    var det = db.CrmTargetSettings.Where(a => a.Empno == empno && a.Mont == dat.Month && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    lst.Add(new DashBoardDetails
                    {
                        sno = 3,
                        descr = "Targets",
                        fir = det.Sum(a => a.Tgt),
                        sec = det.Sum(a => a.Calls),
                        typ = 3
                    });
                    lst.Add(new DashBoardDetails
                    {
                        sno = 4,
                        descr = "Credits",
                        fir = 2,
                        sec = 3,
                        typ = 4
                    });


                    string quer = "";
                    dat = ac.getFinancialStart(dat, inf.usr);
                    var da1 = gg.strDate(dat);
                    var da2 = gg.strDate(dat.AddYears(1));
                     quer = quer + " select labe descri, amt val1,0 val2,sno typ from";
            quer = quer + " (select(year(dat) * 100) + month(dat) mont, DATENAME(MONTH, dat) labe, sum(baseamt - discount) amt, 0 rol, row_number() over(order by(year(dat) * 100) + month(dat)) + 100 sno from";
            quer = quer + " crmsaleOrderUni where dat >= '" + da1 + "' and dat < '" + da2 + "' and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + "";
            quer = quer + " group by  year(dat), month(dat), DATENAME(MONTH, dat))x";

                    DataBaseContext g = new DataBaseContext();
                    SqlCommand dc = new SqlCommand();
                    dc.Connection = g.db;
                    g.db.Open();
                    dc.CommandText = quer;
                    SqlDataReader dr = dc.ExecuteReader();
                    int? sno = 101;
                    while (dr.Read())
                    {
                        lst.Add(new DashBoardDetails
                        {
                            sno = sno,
                            descr = dr[0].ToString(),
                            fir = gg.valNum(dr[1].ToString()),
                            sec = 0,
                            typ = sno
                        });
                        sno++;
                    }
                    dr.Close();
                    g.db.Close();

                    sno = 201;
                    var teamemps = (from a in db.HrdEmployees.Where(a => a.Mgr == empno && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                                    join b in db.UserCompleteProfile.Where(a => a.CustomerCode == usr.cCode) on a.RecordId equals b.EmployeeNo
                                    select new
                                    {
                                        empno = a.RecordId,
                                        empname = a.Empname,
                                        username = sc.makeAsciitoString(b.UsrName)
                                    }).ToList();

                    foreach(var em in teamemps)
                    {
                        UserInfo u1 = new UserInfo();
                        u1.uCode = em.username.ToUpper();
                        u1.bCode = usr.bCode;
                        u1.cCode = usr.cCode;
                        GeneralInformation in1 = new GeneralInformation();
                        in1.frmDate = dat1;
                        in1.toDate = dat2;
                        in1.usr = u1;
                        var l1 = cg.EmployeeDirectBusiness(in1);
                        var l2 = cg.EmployeeTeamBusiness(in1);
                          lst.Add(new DashBoardDetails
                        {
                            sno=sno,
                            descr=em.empname,
                            fir= (l1==null?0:l1.Sum(b => b.Baseamt - b.Discount)) + (l2==null?0:l2.Sum(b => b.Baseamt - b.Discount)),
                            sec=0,
                            typ=sno
                            
                        });
                        sno++; 
                    }

                
                    return lst;

                }
                else
                {
                    return null;
                }
            }
            catch(Exception ee)
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMDashboard/GetPendingCalls")]
        public List<CRMPendingsList> GetPendingCalls([FromBody] UserInfo usr)
        {
            GeneralInformation inf = new GeneralInformation();
            inf.usr = usr;
            List<CRMPendingsList> lst = new List<CRMPendingsList>();
            DateTime dat = ac.getPresentDateTime();
            var det1 = db.CrmTeleCallingRx.Where(a => a.NextCallDate <= dat && a.PrevcallId==null && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            var det2 = db.CrmEnquiriesRx.Where(a => a.NextCallDate <= dat &&  a.PrevcallId == null && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();


             
            
            foreach (CrmTeleCallingRx det in det1)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = det.NextCallMode,
                    dat = det.Dat.Value,
                    customercomments = det.CustomerComments
                });
            }
            foreach (CrmEnquiriesRx det in det2)
            {
                lst.Add(new CRMPendingsList
                {
                    id = det.RecordId,
                    customer = det.Customer,
                    mobile = det.Mobile,
                    mode = det.NextCallMode.ToString(),
                    dat = det.Dat.Value,
                    customercomments = det.CustomerComments
                });
            }
            return lst;
        }
        static string getFullName(int month)
        {
            DateTime date = new DateTime(2020, month, 1);

            return date.ToString("MMMM");
        }
    }
}
