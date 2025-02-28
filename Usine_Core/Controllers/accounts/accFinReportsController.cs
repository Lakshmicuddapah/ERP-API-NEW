using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.Controllers.Admin;
using System.Data;
using System.Data.SqlClient;
 using Microsoft.AspNetCore.Hosting;
using Usine_Core.others;
using Usine_Core.Controllers.Others;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Accounts
{
    public class CashBankBookDetails
    {
        public int? slno { get; set; }
        public int? seq { get; set; }
        public string accname { get; set; }
        public string vouchertype { get; set; }
        public string debit { get; set; }
        public string credit { get; set; }
        public int sno { get; set; }
        public string dat { get; set; }
        public double? cre { get; set; }
        public double? deb { get; set; }
    }
    public class CashBankBookDetailsComplete
    {
        public List<CashBankBookDetails> data { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }


    public class TrialBalanceDetails
    {
        public string sno { get; set; }
        public string account { get; set; }
        public string grp { get; set; }
        public string deb { get; set; }
        public string cre { get; set; }
        public double? debs { get; set; }
        public double? cres { get; set; }
    }
    public class TrialBalanceDetailsComplete
    {
        public List<TrialBalanceDetails> data { get; set; }
        public string pdffile { get; set; }
        public string excelfile { get; set; }
    }

    public class accFinReportsController : ControllerBase
    {


        private readonly IHostingEnvironment ho;
        public accFinReportsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepCashBankBook")]
        public CashBankBookDetailsComplete getAccRepCashBankBook([FromBody] GeneralInformation inf)
        {
            try
            {
                CashBankBookDetailsComplete det = new CashBankBookDetailsComplete();

                String quer = "";
                General g = new General();
                String dat1 = inf.frmDate;
                String dat = g.strDate(ac.getFinancialStart(DateTime.Parse(inf.frmDate), inf.usr));
                String dat2 = g.strDate(DateTime.Parse(inf.frmDate).AddDays(1));

                quer = quer + " select seq, accname,vouchertype, dbo.makcur(debit) debit,dbo.makcur(credit) credit from";
                quer = quer + " (select 1 slno, 0 seq,'Opening Balance' accname,' ' vouchertype,case when opb > 0 then opb else 0 end debit,case when opb > 0 then 0 else abs(opb) end credit,1 sno from";
                quer = quer + " (select sum(deb-cre) opb from finexecdet where accname in(select recordId from finaccounts where acType = '" + inf.detail + "' and customerCode = " + inf.usr.cCode + ") and recordID in (select recordId from finexecUni where dat >= '" + dat + "'";
                quer = quer + " and dat< '" + dat1 + "' and customerCode = " + inf.usr.cCode + ")  and customerCode = " + inf.usr.cCode + ")x";
                quer = quer + " union all";
                quer = quer + " select slno,seq,b.accname,vouchertype,debit,credit,2 sno from";
                quer = quer + " (select a.recordId slno,seq,accname,vouchertype,cre debit, deb credit from";
                quer = quer + " (select* from finexecUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from finexecdet where recordId in ";
                quer = quer + " (select distinct recordId from finexecdet where recordId in(select recordId from finexecuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ") and";
                quer = quer + " accname in (select recordId from finAccounts where actype = '" + inf.detail + "' and customerCode = " + inf.usr.cCode + ") and";
                quer = quer + " customercode = " + inf.usr.cCode + "))b where a.recordID = b.recordID )a,";
                quer = quer + " (select * from finaccounts where customerCode = " + inf.usr.cCode + " and actype<> '" + inf.detail + "')b where a.accname = b.recordid";
                quer = quer + " union all";
                quer = quer + " select distinct slno,seq,narr,' ' vouchertype,0 debit,0 credit,3 sno from";
                quer = quer + " (select a.recordId slno,seq,narr,accname,vouchertype,cre debit, deb credit from";
                quer = quer + " (select * from finexecUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from finexecdet where recordId in ";
                quer = quer + " (select distinct recordId from finexecdet where recordId in(select recordId from finexecuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ") and";
                quer = quer + " accname in (select recordId from finAccounts where actype = '" + inf.detail + "' and customerCode = " + inf.usr.cCode + ") and";
                quer = quer + " customercode = " + inf.usr.cCode + "))b where a.recordID = b.recordID )a,";
                quer = quer + " (select * from finaccounts where customerCode = " + inf.usr.cCode + " and actype<> '" + inf.detail + "')b where a.accname = b.recordid";
                quer = quer + " union all";
                quer = quer + " select 10000000 slno, 0 seq,'Closing Balance' accname,' ' vouchertype,case when opb > 0 then 0 else abs(opb) end debit,case when opb > 0 then opb else 0 end credit,1 sno from";
                quer = quer + " (select sum(deb-cre) opb from finexecdet where accname in(select recordId from finaccounts where acType = '" + inf.detail + "' and customerCode = " + inf.usr.cCode + ") and recordID in (select recordId from finexecUni where dat >= '" + dat + "'";
                quer = quer + " and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ")  and customerCode = " + inf.usr.cCode + ")x)xx order by slno, seq, sno";

                SqlCommand dc = new SqlCommand();
                DataBaseContext gg = new DataBaseContext();
                dc.Connection = gg.db;
                gg.db.Open();
                dc.CommandText = quer;
                SqlDataReader dr = dc.ExecuteReader();
                det.data = new List<CashBankBookDetails>();
                while (dr.Read())
                {
                    det.data.Add(new CashBankBookDetails
                    {
                        seq =  g.valInt(dr[0].ToString()),
                        accname = dr[1].ToString(),
                        vouchertype = dr[2].ToString(),
                        debit = dr[3].ToString(),
                        credit = dr[4].ToString(),

                    });
                }
                dr.Close();
                gg.db.Close();

                return det;
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepDayBook")]

        public CashBankBookDetailsComplete getAccRepDayBook([FromBody]GeneralInformation inf)
        {
            CashBankBookDetailsComplete tot = new CashBankBookDetailsComplete();
            UsineContext db1 = new UsineContext();
            General g = new General();
            DateTime dat1 = DateTime.Parse( inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            List<CashBankBookDetails> list = new List<CashBankBookDetails>();
            var records = db1.FinexecUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
            foreach(FinexecUni record in records)
            {
                var detail = (from a in db.FinexecDet.Where(a => a.RecordId == record.RecordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                              join b in db.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode) on a.Accname equals b.Recordid
                              select new
                              {
                                  a.RecordId,
                                  a.Sno,
                                  accountid = a.Accname,
                                  accountname = b.Accname,
                                  a.Deb,
                                  a.Cre
                              }).OrderBy(b => b.Sno).ToList();

                for(var i=0;i<detail.Count;i++)
                {
                    list.Add(new CashBankBookDetails
                    {
                        seq=i==0?record.Seq:null,
                        accname=detail[i].accountname,
                        vouchertype=i==0?record.Vouchertype:"",
                        debit= detail[i].Deb==0?" " :  g.fixCur((double)detail[i].Deb,2),
                        credit= detail[i].Cre == 0 ? " " : g.fixCur((double)detail[i].Cre, 2),
                    });
                }
                list.Add(new CashBankBookDetails
                {
                    seq = null,
                    accname = record.Narr,
                    vouchertype = " ",
                    debit = " ",
                    credit = " ",
                });
                list.Add(new CashBankBookDetails
                {
                    seq =   null,
                    accname = " ",
                    vouchertype = " ",
                    debit = " ",
                    credit = " ",
                });

            }
            tot.data = list;
            if(list.Count>0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("seq", typeof(string));
                dt.Columns.Add("Account", typeof(string));
                dt.Columns.Add("Transaction", typeof(string));
                dt.Columns.Add("Debit", typeof(string));
                dt.Columns.Add("Credit", typeof(string));

                int i = 1;
                foreach (CashBankBookDetails grp in list)
                {
                    dt.Rows.Add(grp.seq, grp.accname,grp.vouchertype, grp.debit, grp.credit);
                    i++;
                }
                List<string> titles = new List<string>();
                titles.Add("#");
                titles.Add("Account");
                titles.Add("Voucher");
                titles.Add("Debit");
                titles.Add("Credit");
                float[] widths = { 50f, 200f,120f, 90f, 90f };
                int[] aligns = { 3, 1,1, 2, 2 };
                


                DateTime dats = DateTime.Now;
                string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                string fname = inf.usr.uCode + "daybook" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                string filename = ho.WebRootPath + "\\Reps\\" + fname;
                string msg = "";
                PDFExcelMake ep = new PDFExcelMake();
                msg = ep.pdfConversion(filename, "Daybook as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, false);
                if (msg == "OK")
                {
                    tot.pdffile = fname;
                }

                string fname1 = inf.usr.uCode + "daybook" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                string filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                msg = "";
                msg = ep.makeExcelConversion(filename1, "Daybook as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, false);

                if (msg == "OK")
                {
                    tot.excelfile = fname1;
                }
            }

            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepTrialBalance")]
        public TrialBalanceDetailsComplete getAccRepTrialBalance([FromBody]GeneralInformation inf)
        {
            TrialBalanceDetailsComplete data = new TrialBalanceDetailsComplete();
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), inf.usr);
            DateTime dat2 = DateTime.Parse(  inf.frmDate);
            DateTime dat3 = dat2.AddDays(1);
            List<TrialBalanceDetails> lst = new List<TrialBalanceDetails>();
            String quer = "";

            quer = quer + " select* from";
            quer = quer + " (select row_number() over (order by b.accname) sno,b.accname account, dbo.makCur(deb) debs,dbo.makCur(cre) cres,1 flag from";
            quer = quer + " (select accname, sum(deb) deb, sum(cre) cre from";
            quer = quer + " (select* from finexecdet where recordID in (select recordID from finexecUni where dat >= '" + ac.strDate(dat1) + "' and dat < '" + ac.strDate(dat3) + "' and customerCode = " + inf.usr.cCode + ") and customerCode = " + inf.usr.cCode + ")x group by accname)a,";
            quer = quer + " (select recordID, accname from finAccounts where customerCode = " + inf.usr.cCode + ")b where a.accname = b.recordid";
            quer = quer + " union all";
            quer = quer + " select 0 sno,' ' account,' ' debs,' ' cres ,2 flag";
            quer = quer + " union all";
            quer = quer + " select 0 sno,'Total' account, dbo.makCur(sum(deb)) debs,dbo.makCur(sum(cre)) cres,3 flag from finexecdet where recordID in(select recordID from finexecUni where dat >= '" + ac.strDate(dat1) + "' and dat < '" + ac.strDate(dat3) + "' and customerCode = " + inf.usr.cCode + ") and customerCode = " + inf.usr.cCode + ")x";
            quer = quer + " order by flag, account";

            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while (dr.Read())
            {
                lst.Add(new TrialBalanceDetails
                {
                    sno = dr[0].ToString() == "0" ? "" : dr[0].ToString(),
                    account = dr[1].ToString(),
                    deb = dr[2].ToString(),
                    cre = dr[3].ToString()
                });
            }
            dr.Close();
            g.db.Close();




            DataTable dt = new DataTable();
            dt.Columns.Add("sno", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Debit", typeof(string));
            dt.Columns.Add("Credit", typeof(string));

            int i = 1;
            foreach (TrialBalanceDetails grp in lst)
            {
                dt.Rows.Add(grp.sno, grp.account, grp.deb, grp.cre);
                i++;
            }
            List<string> titles = new List<string>();
            titles.Add("#");
            titles.Add("Account");
            titles.Add("Debit");
            titles.Add("Credit");
            float[] widths = { 50f, 300f, 100f, 100f };
            int[] aligns = { 3, 1, 2, 2 };
            data.data = lst;

            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "TrialBalance" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            string filename = ho.WebRootPath + "\\Reps\\" + fname;
            string msg = "";
            PDFExcelMake ep = new PDFExcelMake();
            msg = ep.pdfConversion(filename,"Trial Balance as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.pdffile = fname;
            }

            string fname1 = inf.usr.uCode + "TrialBalance" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
            string filename1 = ho.WebRootPath + "\\Reps\\" + fname;
            msg = "";
            msg = ep.makeExcelConversion(filename1,"Trial Balance as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);

            if (msg == "OK")
            {
                data.excelfile = fname1;
            }
             
           return data;
        }



        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepSchedules")]
        public TrialBalanceDetailsComplete getAccRepSchedules([FromBody] GeneralInformation inf)
        {
            TrialBalanceDetailsComplete data = new TrialBalanceDetailsComplete();
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), inf.usr);
            DateTime dat2 = DateTime.Parse( inf.frmDate);
            DateTime dat3 = dat2.AddDays(1);
            List<TrialBalanceDetails> lst = new List<TrialBalanceDetails>();
            String quer = "";

            quer = quer + " select* from";
            quer = quer + " (select row_number() over (order by b.accname) sno,b.accname account, dbo.makCur(deb) debs,dbo.makCur(cre) cres,1 flag from";
            quer = quer + "(select accname,case when sum(deb) > sum(cre) then sum(deb-cre) else 0 end deb,case when sum(cre) > sum(deb) then sum(cre-deb) else 0 end cre from ";
            quer = quer + " (select* from finexecdet where recordID in (select recordID from finexecUni where dat >= '" + ac.strDate(dat1) + "' and dat < '" + ac.strDate(dat3) + "' and customerCode = " + inf.usr.cCode + ") and customerCode = " + inf.usr.cCode + ")x group by accname)a,";
            quer = quer + " (select recordID, accname from finAccounts where customerCode = " + inf.usr.cCode + ")b where a.accname = b.recordid";
            quer = quer + " union all";
            quer = quer + " select 0 sno,' ' account,' ' debs,' ' cres ,2 flag";
            quer = quer + " union all";
            quer = quer + " select 0 sno,'Total' account, dbo.makCur(sum(deb)) debs,dbo.makCur(sum(cre)) cres,3 flag from";
            quer = quer + "(select accname,case when sum(deb) > sum(cre) then sum(deb-cre) else 0 end deb,case when sum(cre) > sum(deb) then sum(cre-deb) else 0 end cre from ";
            quer = quer + " (select* from finexecdet where recordID in (select recordID from finexecUni where dat >= '" + ac.strDate(dat1) + "' and dat < '" + ac.strDate(dat3) + "' and customerCode = " + inf.usr.cCode + ") and customerCode = " + inf.usr.cCode + ")x group by accname)a,";
            quer = quer + " (select recordID, accname from finAccounts where customerCode = " + inf.usr.cCode + ")b where a.accname = b.recordid";

            quer = quer + " )x order by flag, account";

            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while (dr.Read())
            {
                lst.Add(new TrialBalanceDetails
                {
                    sno = dr[0].ToString() == "0" ? "" : dr[0].ToString(),
                    account = dr[1].ToString(),
                    deb = dr[2].ToString(),
                    cre = dr[3].ToString()
                });
            }
            dr.Close();
            g.db.Close();




            DataTable dt = new DataTable();
            dt.Columns.Add("sno", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Debit", typeof(string));
            dt.Columns.Add("Credit", typeof(string));

            int i = 1;
            foreach (TrialBalanceDetails grp in lst)
            {
                dt.Rows.Add(grp.sno, grp.account, grp.deb, grp.cre);
                i++;
            }
            List<string> titles = new List<string>();
            titles.Add("#");
            titles.Add("Account");
            titles.Add("Debit");
            titles.Add("Credit");
            float[] widths = { 50f, 300f, 100f, 100f };
            int[] aligns = { 3, 1, 2, 2 };
            data.data = lst;
           
            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "TrialBalance" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            string filename = ho.WebRootPath + "\\Reps\\" + fname;
            string msg = "";
            PDFExcelMake ep = new PDFExcelMake();
            msg = ep.pdfConversion(filename,"Schedules as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.pdffile = fname;
            }

            string fname1 = inf.usr.uCode + "TrialBalance" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
            string filename1 = ho.WebRootPath + "\\Reps\\" + fname;
            msg = "";
            msg = ep.makeExcelConversion(filename1,"Schedules as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);

            if (msg == "OK")
            {
                data.excelfile = fname1;
            }
  

            return data;
        }



        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepPNLAccount")]
        public TrialBalanceDetailsComplete getAccRepPNLAccount([FromBody] GeneralInformation inf)
        {
            TrialBalanceDetailsComplete data = new TrialBalanceDetailsComplete();
            AdminControl ac = new AdminControl();
            string dat1 = ac.strDate(ac.getFinancialStart(ac.getPresentDateTime(), inf.usr));
            DateTime dat2 = DateTime.Parse( inf.frmDate).AddDays(1);
            string dat3 = ac.strDate(dat2);
            List<TrialBalanceDetails> lst = new List<TrialBalanceDetails>();
            String quer = "";


            quer = quer + " select slno, accname, sGrp, debs, cres from";
            quer = quer + " (select convert(varchar(5), row_number() over(order by b.accname)) slno, b.accname, b.sGrp, dbo.makCur(a.debit) debs, dbo.makCur(a.credit) cres, a.debit, a.credit, 1 sno from";
            quer = quer + " (select accname, sum(deb) debit, sum(cre) credit from finexecDet where dat >= '" + dat1 + "' and dat < '" + dat3 + "' and";
            quer = quer + " customerCode = " + inf.usr.cCode  + " and accname in";
            quer = quer + " (select recordId from finAccounts where customerCode = " + inf.usr.cCode + " and accgroup in";
            quer = quer + " (select recordId from finAccGroups where (grpTag like 'c%' or grpTag like 'd%')";
            quer = quer + " and customerCode = " + inf.usr.cCode + ")) group by accname)a,";
            quer = quer + " (select a.recordId,a.accname,b.sGrp from";
            quer = quer + " (select * from finAccounts where customerCode= " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from finAccGroups where customerCode = " + inf.usr.cCode + ")b where a.accgroup = b.recordId)b where a.accname = b.recordid";
            quer = quer + " union all";
            quer = quer + " select ' ' slno, ' ' accname,' ' sGrp,' ' debs,' ' cres, 0 debit,0 credit, 2 sno";
            quer = quer + " union all";
            quer = quer + " select  ' '  slno,'Total' accname, ' ' sGrp,  dbo.makcur(sum(deb)) debs,dbo.makCur(sum(cre)) cres, sum(deb) debit,sum(cre) credit,3 sno from";
            quer = quer + " finexecDet where dat >= '" + dat1 + "' and dat< '" + dat3 + "' and";
            quer = quer + " customerCode = " + inf.usr.cCode + " and accname in";
            quer = quer + " (select recordId from finAccounts where customerCode = " + inf.usr.cCode + " and accgroup in";
            quer = quer + " (select recordId from finAccGroups where(grpTag like 'c%' or grpTag like 'd%')";
            quer = quer + " and customerCode = " + inf.usr.cCode + ")) ";
            quer = quer + " union all";
            quer = quer + " select ' ' slno, case when sum(deb-cre) > 0 then 'Loss' else case when sum(cre-deb) > 0 then 'Profit' else 'No Profit no loss' end end accname,";
            quer = quer + " ' ' sGrp, ";
            quer = quer + " case when sum(cre-deb) > 0 then dbo.makCur(sum(cre - deb)) else '' end cres,";
            quer = quer + " case when sum(deb-cre) > 0 then dbo.makcur(sum(deb - cre)) else '' end debs,";
            quer = quer + " 0 debit,0 credit,4 sno from";
            quer = quer + " finexecDet where dat >= '" + dat1 + "' and dat< '" + dat3 + "' and";
            quer = quer + " customerCode = " + inf.usr.cCode + " and accname in";
            quer = quer + " (select recordId from finAccounts where customerCode = " + inf.usr.cCode + " and accgroup in";
            quer = quer + " (select recordId from finAccGroups where(grpTag like 'c%' or grpTag like 'd%')";
            quer = quer + " and customerCode = " + inf.usr.cCode + ")))x order by sno, accname";

            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while (dr.Read())
            {
                lst.Add(new TrialBalanceDetails
                {
                    sno = dr[0].ToString() == "0" ? "" : dr[0].ToString(),
                    account = dr[1].ToString(),
                    grp=dr[2].ToString(),
                    deb = dr[3].ToString(),
                    cre = dr[4].ToString()
                });
            }
            dr.Close();
            g.db.Close();




            DataTable dt = new DataTable();
            dt.Columns.Add("sno", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Grp", typeof(string));
            dt.Columns.Add("Debit", typeof(string));
            dt.Columns.Add("Credit", typeof(string));

            int i = 1;
            foreach (TrialBalanceDetails grp in lst)
            {
                dt.Rows.Add(grp.sno, grp.account,grp.grp, grp.deb, grp.cre);
                i++;
            }
            List<string> titles = new List<string>();
            titles.Add("#");
            titles.Add("Account");
            titles.Add("Group");
            titles.Add("Debit");
            titles.Add("Credit");
            float[] widths = { 50f, 200f,150f, 75f, 75f };
            int[] aligns = { 3, 1,1, 2, 2 };
            data.data = lst;

            PDFExcelMake ep = new PDFExcelMake();
            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "PNLAC" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            string fname1 = inf.usr.uCode + "PNLAC" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
            string filename1 = ho.WebRootPath + "\\Reps\\" + fname;
            string msg = "";
            msg = ep.pdfConversion(filename1, "P N L Account as on " + inf.frmDate  , inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.pdffile = fname;
            }
            filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
            msg = ep.makeExcelConversion(filename1, "P N L Account as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.excelfile = fname1;
            }


            return data;
        }




        //Ledger Code starts

        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepLedgerDetaild")]
        public CashBankBookDetailsComplete getAccRepLedgerDetaild([FromBody] GeneralInformation inf)
        {
            try
            {
                General gg = new General();
                List<CashBankBookDetails> lst = new List<CashBankBookDetails>();
                AdminControl ac = new AdminControl();
                DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), inf.usr);
                DateTime dat2 = DateTime.Parse(inf.frmDate);
                DateTime dat3 = DateTime.Parse(inf.toDate);
                dat3 = dat3.AddDays(1);
                var opb = db.FinexecDet.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.CustomerCode == inf.usr.cCode &&
              a.Accname == inf.recordId).Sum(x => x.Deb - x.Cre);
                lst.Add(new CashBankBookDetails
                {
                    slno = 1,
                    seq = 0,
                    accname = "Opening Balance",
                    dat = " ",
                    vouchertype = " ",
                    debit =gg.makeCur((double)( opb > 0 ? opb : 0),2),
                    credit = gg.makeCur((double)(opb < 0 ? Math.Abs((double)opb) : 0), 2),
                    deb= (double)(opb > 0 ? opb : 0),
                    cre= (double)(opb < 0 ? Math.Abs((double)opb):0),
                    sno = 1

                });

                var transactionids = db.FinexecDet.Where(a => a.Dat >= dat2 && a.Dat < dat3 && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Accname == inf.recordId).Select(g => g.RecordId).Distinct();
                UsineContext db1 = new UsineContext();
                UsineContext db2 = new UsineContext();
                foreach (int id in transactionids)
                {
                    var header = db2.FinexecUni.Where(a => a.RecordId == id && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (header != null)
                    {
                        var detail = (from x in db1.FinexecDet.Where(a => a.RecordId == id && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                      join y in db1.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode && a.Recordid != inf.recordId)
                                      on x.Accname equals y.Recordid
                                      select new
                                      {
                                          slno = 2,
                                          seq = header.Seq,
                                          accname = y.Accname,
                                          dat = ac.strDate((DateTime)x.Dat),
                                          vouchertype = header.Vouchertype,
                                          debit = x.Cre,
                                          credit = x.Deb,
                                          sno = 2

                                      }).ToList();
                        for (var i = 0; i < detail.Count(); i++)
                        {
                            lst.Add(new CashBankBookDetails
                            {
                                slno = 2,
                                seq = (int)(i == 0 ? detail[i].seq : 0),
                                accname = detail[i].accname,
                                dat = i == 0 ? detail[i].dat : "",
                                vouchertype = i == 0 ? detail[i].vouchertype : "",
                                debit = gg.makeCur((double)detail[i].debit, 2),
                                credit = gg.makeCur((double)detail[i].credit, 2),
                                cre = detail[i].credit,
                                deb = detail[i].debit

                            }); 
                        }

                        lst.Add(new CashBankBookDetails
                        {
                            slno = 2,
                            seq = 0,
                            accname = header.Narr,
                            dat = "",
                            vouchertype = "",
                            debit = "0.00",
                            credit = "0.00",
                             cre = 0,
                            deb = 0
                        });

                    }

                }





                var cpb = db.FinexecDet.Where(a => a.Dat >= dat1 && a.Dat < dat3 && a.CustomerCode == inf.usr.cCode &&
             a.Accname == inf.recordId).Sum(d => d.Deb - d.Cre);

                lst.Add(new CashBankBookDetails
                {
                    slno = 3,
                    seq = 0,
                    accname = "Closing Balance",
                    dat = "",
                    vouchertype = " ",
                    //debit = cpb < 0 ? Math.Abs((double)cpb) : 0,
                    debit = gg.makeCur((double)(cpb < 0 ? Math.Abs((double)cpb) : 0), 2),
                    credit = gg.makeCur((double)(cpb > 0 ? cpb : 0), 2),
                    sno = 1,
                    deb= (double)(cpb < 0 ? Math.Abs((double)cpb) : 0),
                    cre= (double)(cpb > 0 ? cpb : 0)

                });

                 
                if(lst.Count > 0)
                {
                    var cres = lst.Sum(a => a.cre);
                    var debs = lst.Sum(a => a.deb);
                    lst.Add(new CashBankBookDetails
                    {
                        slno = 4,
                        seq = 0,
                        accname = " ",
                        dat = "",
                        vouchertype = " ",
                        debit = gg.makeCur((double)debs, 2),
                        credit = gg.makeCur((double)cres, 2),
                        sno = 1,
                        deb=0,
                        cre=0
                    });

                }

                if (lst.Count > 0)
                {
                    CashBankBookDetailsComplete det = new CashBankBookDetailsComplete();
                    det.data = lst;// lst.OrderBy(a => a.slno).ThenBy(b => b.seq).ThenBy(c => c.sno).ToList();


                    PDFExcelMake ep = new PDFExcelMake();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("seq", typeof(string));
                    dt.Columns.Add("accname", typeof(string));
                    dt.Columns.Add("date", typeof(string));
                    dt.Columns.Add("vouchertype", typeof(string));
                    dt.Columns.Add("debit", typeof(string));
                    dt.Columns.Add("credit", typeof(string));
                    int i = 1;
                    AdminControl am = new AdminControl();
                    for (i = 0; i < det.data.Count; i++)
                    {
                        dt.Rows.Add(det.data[i].seq == 0 ? "" : det.data[i].seq.ToString(), det.data[i].accname, det.data[i].dat, det.data[i].vouchertype, (gg.valNum(det.data[i].debit) == 0 ? "" :  det.data[i].debit ), (gg.valNum(det.data[i].credit) == 0 ? "" :  det.data[i].credit));

                    }

                    List<string> titles = new List<string>();
                    titles.Add("Seq");
                    titles.Add("Account");
                    titles.Add("Date");
                    titles.Add("voucher");
                    titles.Add("Debit");
                    titles.Add("Credit");
                    float[] widths = { 60f, 180f, 80f, 80f, 80f, 80f };
                    int[] aligns = { 1, 1, 1, 1, 2, 2 };
                    DateTime dats = DateTime.Now;
                    string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
                    string fname = inf.usr.uCode + "LedgerDet" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
                    string fname1 = inf.usr.uCode + "LedgerDet" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
                    string filename1 = ho.WebRootPath + "\\Reps\\" + fname;
                    string msg = "";
                    msg= ep.pdfConversion( filename1, "Ledger of " + inf.detail + " from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                    if(msg=="OK")
                    {
                        det.pdffile = fname;
                    }
                    filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
                   msg = ep.makeExcelConversion(filename1,"Ledger of " + inf.detail + " from " + inf.frmDate + " to " + inf.toDate, inf.usr, dt, titles, widths, aligns, true);
                if(msg=="OK")
                    {
                        det.excelfile = fname1;
                    }
                    
                    
                    return det;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception ee)
            {
                return null;
            }


        }


        [HttpPost]
        [Authorize]
        [Route("api/accFinReports/getAccRepBalanceSheet")]
        public TrialBalanceDetailsComplete getAccRepBalanceSheet([FromBody] GeneralInformation inf)
        {
            TrialBalanceDetailsComplete data = new TrialBalanceDetailsComplete();
            AdminControl ac = new AdminControl();
            string dat1 = ac.strDate(ac.getFinancialStart(ac.getPresentDateTime(), inf.usr));
            DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);
            string dat3 = ac.strDate(dat2);
            List<TrialBalanceDetails> lst = new List<TrialBalanceDetails>();
            String quer = "";


            quer = quer + " select convert(varchar(5),row_number() over(order by b.accname)) slno, b.accname,b.sGrp,debit,credit,deb,cre,1 sno from";
            quer = quer + " (select accname, dbo.makCur(sum(deb)) debit, dbo.makCur(sum(cre)) credit, sum(deb) deb, sum(cre) cre from finexecDet where dat >= '" + dat1 + "' and dat < '" + dat3 + "' and";
            quer = quer + " customerCode = " + inf.usr.cCode + " and accname in";
            quer = quer + " (select recordId from finAccounts where customerCode = " + inf.usr.cCode + " and accgroup in";
            quer = quer + " (select recordId from finAccGroups where (grpTag like 'a%' or grpTag like 'b%')";
            quer = quer + " and customerCode = " + inf.usr.cCode + ")) group by accname)a,";
            quer = quer + " (select a.recordId,a.accname,b.sGrp from";
            quer = quer + " (select* from finAccounts where customerCode= " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from finAccGroups where customerCode = " + inf.usr.cCode + ")b where a.accgroup = b.recordId)b where a.accname = b.recordid";
            quer = quer + " union all";
            quer = quer + " select ' ' slno, case when loss > 0 then 'Loss' else case when profit > 0 then 'Profit' else 'No Profit No Loss' end end accname,";
            quer = quer + " ' ' sGrp,dbo.makCur(loss) debit,dbo.makCur(profit) credit,loss,profit,2 sno from";
            quer = quer + " (select  case when sum(deb) -sum(cre) > 0 then sum(deb)-sum(cre) else 0 end loss,";
            quer = quer + " case when sum(cre)-sum(deb) > 0 then sum(cre)-sum(deb) else 0 end profit";
            quer = quer + " from finexecDet where dat >= '" + dat1 + "' and dat< '" + dat3 + "' and";
            quer = quer + " customerCode = " + inf.usr.cCode + " and accname in";
            quer = quer + " (select recordId from finAccounts where customerCode = " + inf.usr.cCode + " and accgroup in";
            quer = quer + " (select recordId from finAccGroups where(grpTag like 'c%' or grpTag like 'd%')";
            quer = quer + " and customerCode = " + inf.usr.cCode + ")) )x";

            General gg = new General();
            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while (dr.Read())
            {
                lst.Add(new TrialBalanceDetails
                {
                    sno = dr[0].ToString() == "0" ? "" : dr[0].ToString(),
                    account = dr[1].ToString(),
                    grp = dr[2].ToString(),
                    deb = dr[3].ToString(),
                    cre = dr[4].ToString(),
                    debs=gg.valNum(dr[5].ToString()),
                    cres=gg.valNum(dr[6].ToString())
                });
            }
            dr.Close();
            g.db.Close();

            var dr1 = lst.Sum(a => a.debs);
            var cr1 = lst.Sum(a => a.cres);

            lst.Add(new TrialBalanceDetails
            {
                sno = " ",
                account = " ",
                grp = " ",
                deb = " ",
                cre = " ",
                debs = 0,
                cres = 0
            });
            lst.Add(new TrialBalanceDetails
            {
                sno = " ",
                account = "Total",
                grp = " ",
                deb = gg.makeCur((double)dr1,2),
                cre = gg.makeCur((double)cr1, 2),
                debs = 0,
                cres = 0
            });

            DataTable dt = new DataTable();
            dt.Columns.Add("sno", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Grp", typeof(string));
            dt.Columns.Add("Debit", typeof(string));
            dt.Columns.Add("Credit", typeof(string));

            int i = 1;
            foreach (TrialBalanceDetails grp in lst)
            {
                dt.Rows.Add(grp.sno, grp.account,grp.grp, grp.deb, grp.cre);
                i++;
            }
            List<string> titles = new List<string>();
            titles.Add("#");
            titles.Add("Account");
            titles.Add("Group");
            titles.Add("Debit");
            titles.Add("Credit");
            float[] widths = { 50f, 200f, 150f, 75f, 75f };
            int[] aligns = { 3, 1, 1, 2, 2 };
            data.data = lst;


            PDFExcelMake ep = new PDFExcelMake();
            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "BalanceSheet" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            string fname1 = inf.usr.uCode + "BalanceSheet" + dat + inf.usr.cCode + inf.usr.bCode + ".xlsx";
            string filename1 = ho.WebRootPath + "\\Reps\\" + fname;
            string msg = "";
            msg = ep.pdfConversion(filename1, "Balance Sheet as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.pdffile = fname;
            }
            filename1 = ho.WebRootPath + "\\Reps\\" + fname1;
            msg = ep.makeExcelConversion(filename1, "Balance Sheet as on " + inf.frmDate, inf.usr, dt, titles, widths, aligns, true);
            if (msg == "OK")
            {
                data.excelfile = fname1;
            }


            return data;
        }



    }
}
