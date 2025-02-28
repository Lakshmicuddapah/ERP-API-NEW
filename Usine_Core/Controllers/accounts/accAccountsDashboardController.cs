using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Usine_Core.Models;


namespace Usine_Core.Controllers.Accounts
{
    public class IAccDashDetails
    {
        public int? sno { get; set; }
        public String descr { get; set; }
        public double? fir { get; set; }
        public double? sec { get; set; }

    }
    public class DashBoardDetails
    {
        public int? sno { get; set; }
        public string descr { get; set; }
        public double? fir { get; set; }
        public double? sec { get; set; }
        public int? typ { get; set; }
    }
    public class accAccountsDashboardController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/accounts/getIAccountsDashboard")]
        public List<IAccDashDetails> getIAccountsDashboard([FromBody] UserInfo usr)
        {
            
            AdminControl ac = new AdminControl();
            if (ac.screenCheck(usr, 1, 0, 0, 1))
            {
                DateTime date1 = ac.getFinancialStart(DateTime.Now, usr);
                DateTime date2 = date1.AddYears(1);
                DateTime dat2 = ac.getPresentDateTime();
                DateTime dat1 = ac.getFinancialStart(dat2, usr);
                List<IAccDashDetails> details = new List<IAccDashDetails>();
                String quer = "";
                quer = quer + " select * from";
                quer = quer + " (select 1 sno, 'PNL' descr, case when sum(cre) is null then 0 else sum(cre) end  fir, case when sum(deb) is null then 0 else sum(deb) end  sec from finexecdet where recordId in (select recordID from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordID from finaccounts where customerCode = " + usr.cCode.ToString() + " and accgroup in (select recordID from finaccgroups where (grptag like 'c%' or grptag like 'd%') and customerCode =  " + usr.cCode.ToString() + "))) and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + " union all";
                quer = quer + " select 2 sno,'CASH' descr, case when sum(deb) is null then 0 else sum(deb) end fir,case when sum(cre) is null then 0 else sum(cre) end sec from finexecDet where recordID in";
                quer = quer + " (select recordId from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordId from finAccounts where acType = 'CAS' and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + " union all";
                quer = quer + " select 3 sno,'BANK' descr, case when sum(deb) is null then 0 else sum(deb) end fir,case when sum(cre) is null then 0 else sum(cre) end sec from finexecDet where recordID in";
                quer = quer + " (select recordId from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordId from finAccounts where acType = 'BAN' and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + " union all";
                quer = quer + " select 4 sno,'M_WALLET' descr, case when sum(deb) is null then 0 else sum(deb) end fir,case when sum(cre) is null then 0 else sum(cre) end sec from finexecDet where recordID in";
                quer = quer + " (select recordId from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordId from finAccounts where acType = 'MOB' and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + " union all";
                quer = quer + " select 5 sno,'SUP' descr, case when sum(cre) is null then 0 else sum(cre) end fir,case when sum(deb) is null then 0 else sum(deb) end sec from finexecDet where recordID in";
                quer = quer + " (select recordId from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordId from finAccounts where acType = 'SUP' and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + " union all";
                quer = quer + " select 6 sno,'CUS' descr, case when sum(deb) is null then 0 else sum(deb) end fir,case when sum(cre) is null then 0 else sum(cre) end sec from finexecDet where recordID in";
                quer = quer + " (select recordId from finexecuni where dat >= '" + ac.strDate(dat1) + "' and dat <= '" + ac.strDate(dat2) + "' and accname in (select recordId from finAccounts where acType = 'CUS' and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + ") and customerCode =  " + usr.cCode.ToString() + "";
                quer = quer + "";
                quer = quer + ")x order by sno, fir desc";

                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                while (dr.Read())
                {
                    details.Add(new IAccDashDetails
                    {
                        sno = int.Parse(dr[0].ToString()),
                        descr = dr[1].ToString(),
                        fir = double.Parse(dr[2].ToString()),
                        sec = double.Parse(dr[3].ToString())

                    });
                }
                dr.Close();
                g.db.Close();

                var totalamt = db.SalSalesUni.Where(a => a.Dat >= date1 && a.Dat < date2 && a.CustomerCode == usr.cCode).Sum(b => b.Baseamt - b.Discount);
                var lst1 = (from a in db.SalSalesUni.Where(a => a.Dat >= date1 && a.Dat < date2 && a.CustomerCode == usr.cCode).GroupBy(b => b.PartyName)
                            select new
                            {
                                partyname = a.Key,
                                amt = a.Sum(b => b.Baseamt - b.Discount),
                                rol = a.Sum(b => b.Baseamt - b.Discount) / totalamt * 100
                            }).OrderByDescending(c => c.amt).Take(10).ToList();
                foreach (var ls in lst1)
                {
                    details.Add(new IAccDashDetails
                    {
                        descr = ls.partyname,
                        fir = (double)ls.amt,
                        sec = (double)ls.rol,
                        sno = 7
                    });
                }

                return details;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/accounts/GetDashBoardDetails")]
        public List<DashBoardDetails> GetDashBoardDetails([FromBody] UserInfo usr)
        {
            if (ac.screenCheck(usr, 1, 0, 0, 1))
            {
                List<DashBoardDetails> list = new List<DashBoardDetails>();
                DateTime date1 = ac.getFinancialStart(DateTime.Now, usr);
                DateTime date2 = date1.AddYears(1);
                string dat1 = ac.strDate(date1);
                string dat2 = ac.strDate(date2);

                var headers = getIAccountsDashboard(usr).ToList();
                foreach (var header in headers)
                {
                    list.Add(new DashBoardDetails
                    {
                        sno = header.sno,
                        descr = header.descr,
                        fir = header.fir,
                        sec = header.sec,
                        typ = 1
                    });
                }
                string quer = "";

                quer = "select max(right(dbo.strdate(dat),6)) dats, sum(deb),sum(cre),month(dat) mon,year(dat) yea from finexecDet where accname in";
                quer = quer + " (select recordId from finaccounts where accgroup in";
                quer = quer + " (select recordID from finaccgroups where(grpTag like 'c%' or grpTag like 'd%') and customerCode = " + usr.cCode + ") and customerCode = " + usr.cCode + ") and dat >= '" + dat1 + "' and dat < '" + dat2 + "' group by month(dat),year(dat) order by yea,mon";

                DataBaseContext g = new DataBaseContext();
                SqlCommand dc = new SqlCommand();
                dc.Connection = g.db;
                g.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                int sno = 1;
                while (dr.Read())
                {
                    list.Add(new DashBoardDetails
                    {
                        sno = sno,
                        descr = dr[0].ToString(),
                        fir = double.Parse(dr[1].ToString()),
                        sec = double.Parse(dr[2].ToString()),
                        typ = 2
                    });
                    sno++;
                }
                dr.Close();

                quer = "";
                quer = quer + " select * from";
                quer = quer + " (select a.typ typ,case when b.amt is null then a.amt else b.amt end amt,a.sno from";
                quer = quer + " (select '0-30' typ,0 amt,1 sno union select '30-60' typ,0 amt,2 sno union select '60-90' typ,0 amt,3 sno union select ' > 90' typ,0 amt, 4 sno)a left outer join";

                quer = quer + " (select typ,sum(amt) amt,max(sno) sno from";
                quer = quer + " (select * from";
                quer = quer + " (select *, '0-30' typ, 1 sno from";
                quer = quer + " (select partyid, datediff(DD, dat, sysdatetime()) age, pendingamount - returnamount - creditNote - paymentAmount amt, customerCode from partyTransactions)x";
                quer = quer + " where amt > 0 and age >= 0 and age <= 30";
                quer = quer + " union all";
                quer = quer + " select *, '30-60' typ, 2 sno from";
                quer = quer + " (select partyid, datediff(DD, dat, sysdatetime()) age, pendingamount - returnamount - creditNote - paymentAmount amt, customerCode from partyTransactions)x";
                quer = quer + " where amt > 0 and age > 30 and age <= 60";
                quer = quer + " union all";
                quer = quer + " select *, '60-90' typ, 3 sno from";
                quer = quer + " (select partyid, datediff(DD, dat, sysdatetime()) age, pendingamount - returnamount - creditNote - paymentAmount amt, customerCode from partyTransactions)x";
                quer = quer + " where amt > 0 and age > 60 and age <= 90";
                quer = quer + " union all";
                quer = quer + " select *, ' > 90' typ, 4 smo from";
                quer = quer + " (select partyid, datediff(DD, dat, sysdatetime()) age, pendingamount - returnamount - creditNote - paymentAmount amt, customerCode from partyTransactions)x";
                quer = quer + " where amt > 0 and age > 90)x where partyid in (select recordId from partyDetails where partyType = 'CUS') and customerCode = 12002)x";
                quer = quer + " group by typ)b  on a.typ=b.typ)x order by sno";

                dc.CommandText = quer;
                dr = dc.ExecuteReader();
                 sno = 1;
                while (dr.Read())
                {
                    list.Add(new DashBoardDetails
                    {
                        sno = sno,
                        descr = dr[0].ToString(),
                        fir = double.Parse(dr[1].ToString()),
                        sec =0,
                        typ = 3
                    });
                    sno++;
                }
                dr.Close();
                g.db.Close();

                var totalamt = db.SalSalesUni.Where(a => a.Dat >= date1 && a.Dat < date2 && a.CustomerCode == usr.cCode).Sum(b => b.Baseamt - b.Discount);
                var lst1 = (from a in db.SalSalesUni.Where(a => a.Dat >= date1 && a.Dat < date2 && a.CustomerCode == usr.cCode).GroupBy(b => b.PartyName)
                            select new
                            {
                                partyname = a.Key,
                                amt = a.Sum(b => b.Baseamt - b.Discount),
                                rol = a.Sum(b => b.Baseamt - b.Discount) / totalamt * 100
                            }).OrderByDescending(c => c.amt).Take(10).ToList();
                foreach(var ls in lst1)
                {
                    list.Add(new DashBoardDetails
                    {
                        descr=ls.partyname,
                        fir=(double)ls.amt,
                        sec=(double)ls.rol,
                        typ=4
                    });
                }

                var lst2 = (from a in db.PartyTransactions.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.PartyId)
                            select new
                            {
                                partyid = a.Key,
                                amt = a.Sum(b => b.PendingAmount - b.ReturnAmount - b.CreditNote - b.PaymentAmount)
                            }).ToList();
                var lst3 = db.PartyDetails.Where(a => a.PartyType == "CUS" && a.CustomerCode == usr.cCode).ToList();
                var details = (from a in lst2
                               join b in lst3 on a.partyid equals b.RecordId
                               select new
                               {
                                   partyname = b.PartyName,
                                   amt = a.amt
                               }).OrderByDescending(b => b.amt).Take(10).ToList();
                foreach(var ls in details)
                {
                    list.Add(new DashBoardDetails
                    {
                        descr = ls.partyname,
                        fir = (double)ls.amt,
                        sec = 0,
                        typ = 5
                    });
                }

                return list;
            }
            else
            {
                return null;
            }
        }
    }
}
