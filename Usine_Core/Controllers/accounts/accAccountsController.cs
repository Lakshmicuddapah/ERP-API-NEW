using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
//using Complete_Solutions_Core.Controllers.admin
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;
using Usine_Core.others;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Accounts
{
    public class AccountDetails
    {
        public int? accountId { get; set; }
        public string accountname { get; set; }
        public string grp { get; set; }
        public int? grpid { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string stat { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string webid { get; set; }
        public string actype { get; set; }
        public int? acchk { get; set; }
        public double? balanceAmt { get; set; }
        public int? restrictMode { get; set; }
        public double? availableCredit { get; set; }
        public string pricelist { get; set; }
        public string discountlist { get; set; }

    }
    public class AccPartyDetailsForTrans
    {
        public int? accountId { get; set; }
        public string accountname { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string stat { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string mobile { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string webid { get; set; }
        public int? creditdayscheck { get; set; }
        public int? creditdays { get; set; }
        public int? creditamtcheck { get; set; }
        public double? creditamt { get; set; }
        public double? balanceamt { get; set; }
        public int? balancedays { get; set; }


    }
    public class AccountsForTransactions
    {
        public int? accountId { get; set; }
        public String accountName { get; set; }
        public String grp { get; set; }
        public String actype { get; set; }
        public double? balance { get; set; }
        public DateTime? dat { get; set; }

    }
    public class AccountsTotal
    {
        public FinAccounts acc { get; set; }
        public int? tracheck { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
    public class accAccountsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        [HttpPost]
        [Route("api/accAccounts/GetAccounts")]
        public IQueryable<AccountDetails> GetAccounts([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            try
            {
                return (from a in db.FinAccounts.Where(a => a.CustomerCode == usr.cCode)
                        join b in db.FinAccGroups.Where(a => a.CustomerCode == usr.cCode) on a.Accgroup equals b.RecordId
                        select new AccountDetails
                        {
                            accountId = a.Recordid,
                            accountname = a.Accname,
                            grp = b.SGrp,
                            grpid = b.RecordId,
                            address = a.Address,
                            country = a.Country,
                            stat = a.State,
                            district = a.District,
                            city = a.City,
                            zip = a.Pin,
                            mobile = a.Mobile,
                            tel = a.Tel,
                            fax = a.Fax,
                            email = a.Email,
                            webid = a.WebId,
                            actype = a.AcType,
                            acchk = a.AcChk
                        }).OrderBy(b => b.accountname);
            }
            catch
            {
                return null;
            }
        }


        
        [HttpPost]
        [Route("api/accAccounts/GetAccount")]
        public AccountDetails GetAccount([FromBody] GeneralInformation inf)
        {
            try
            {
                return (from a in db.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode && a.Recordid == inf.recordId)
                        join b in db.FinAccGroups.Where(a => a.CustomerCode == inf.usr.cCode) on a.Accgroup equals b.RecordId
                        select new AccountDetails
                        {
                            accountId = a.Recordid,
                            accountname = a.Accname,
                            grp = b.SGrp,
                            grpid = b.RecordId,
                            address = a.Address,
                            country = a.Country,
                            stat = a.State,
                            district = a.District,
                            city = a.City,
                            zip = a.Pin,
                            mobile = a.Mobile,
                            tel = a.Tel,
                            fax = a.Fax,
                            email = a.Email,
                            webid = a.WebId,
                            actype = a.AcType,
                            acchk = a.AcChk
                        }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/accAccounts/GetAccountsTypeWise")]
        public List<AccountDetails> GetAccountsTypeWise([FromBody] GeneralInformation inf)
        {
            try
            {
                return (from a in db.FinAccounts.Where(a => a.CustomerCode == inf.usr.cCode && a.AcType == inf.detail)
                        join b in db.FinAccGroups.Where(a => a.CustomerCode == inf.usr.cCode) on a.Accgroup equals b.RecordId
                        select new AccountDetails
                        {
                            accountId = a.Recordid,
                            accountname = a.Accname,
                            grp = b.SGrp,
                            grpid = b.RecordId,
                            address = a.Address,
                            country = a.Country,
                            stat = a.State,
                            district = a.District,
                            city = a.City,
                            zip = a.Pin,
                            mobile = a.Mobile,
                            tel = a.Tel,
                            fax = a.Fax,
                            email = a.Email,
                            webid = a.WebId,
                            actype = a.AcType,
                            acchk = a.AcChk
                        }).OrderBy(b => b.accountname).ToList();
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [Authorize]
        [Route("api/accAccounts/GetTotalAssetsandLiabilitiesAccounts")]
        public List<AccountDetails> GetTotalAssetsandLiabilitiesAccounts([FromBody] UserInfo usr)
        {
            return (from a in db.FinAccounts.Where(a => a.CustomerCode == usr.cCode)
                     join b in db.FinAccGroups.Where(a => (a.GrpTag.Contains("a") || a.GrpTag.Contains("b")) && a.CustomerCode == usr.cCode) on a.Accgroup equals b.RecordId
                     select new AccountDetails
                     {
                         accountId = a.Recordid,
                         accountname = a.Accname,
                         grp = b.SGrp,
                         grpid = b.RecordId,
                         address = a.Address,
                         country = a.Country,
                         stat = a.State,
                         district = a.District,
                         city = a.City,
                         zip = a.Pin,
                         mobile = a.Mobile,
                         tel = a.Tel,
                         fax = a.Fax,
                         email = a.Email,
                         webid = a.WebId,
                         actype = a.AcType,
                         acchk = a.AcChk
                     }).OrderBy(c => c.accountname).ToList();
        }

        [HttpGet]

        [Route("api/accAccounts/accExample")]
        public DataTable accExample()
        {
            UserInfo usr = new UserInfo();
            usr.cCode = 11120;
            AdminControl ac = new AdminControl();
            DateTime todat = ac.getPresentDateTime();
            DateTime frmdat = ac.getFinancialStart(todat, usr);

            String quer = "";

            quer = quer + " select a.recordID,a.accname,a.Sgrp, a.AcType,case when b.balance is null then 0 else b.balance end balance from";
            quer = quer + " (select a.recordID, accname, sGrp, acType from";
            quer = quer + " (select recordid, accname, accgroup, acType from finAccounts where customerCode = " + usr.cCode + " and acChk > 0)a,";
            quer = quer + " (select recordId, sGrp from finaccgroups where customerCode = " + usr.cCode + ")b where a.accgroup = b.recordID )a left outer join";
            quer = quer + " (select accname, sum(deb-cre) balance from finexecDet where recordID in(select recordID from finexecUni where  dat >= '" + ac.strDate(frmdat) + "' and dat <= '" + ac.strDate(todat) + "'  and customerCode = " + usr.cCode + ") and customerCode = " + usr.cCode + " group by accname)b";
            quer = quer + " on a.recordid = b.accname order by a.accname";

            DataSet dss = new DataSet();
            DataBaseContext g = new DataBaseContext();
          
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            using (g.db)
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    quer, g.db);
                adapter.Fill(dss);

            }
            if (dss.Tables.Count > 0)
            {
                return dss.Tables[0];
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/accAccounts/accTransactionAccInformation")]
        public List<AccountsForTransactions> accTransactionAccInformation([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            DateTime todat = ac.getPresentDateTime();
            DateTime frmdat = ac.getFinancialStart(todat, usr);
            General gg = new General();
            String quer = "";

            quer = quer + " select a.recordID,a.accname,a.Sgrp, a.AcType,case when b.balance is null then 0 else b.balance end balance from";
            quer = quer + " (select a.recordID, accname, sGrp, acType from";
            quer = quer + " (select recordid, accname, accgroup, acType from finAccounts where actype in('CAS','BAN','MOB') and customerCode = " + usr.cCode + " and acChk > 0)a,";
            quer = quer + " (select recordId, sGrp from finaccgroups where customerCode = " + usr.cCode + ")b where a.accgroup = b.recordID )a left outer join";
            quer = quer + " (select accname, sum(deb-cre) balance from finexecDet where recordID in(select recordID from finexecUni where  dat >= '" + ac.strDate(frmdat) + "' and dat <= '" + ac.strDate(todat) + "'  and customerCode = " + usr.cCode + ") and customerCode = " + usr.cCode + " group by accname)b";
            quer = quer + " on a.recordid = b.accname order by a.accname";

            DataSet dss = new DataSet();
            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<AccountsForTransactions> result = new List<AccountsForTransactions>();
            var balchk = ac.screenCheck(usr, 1, 8, 3, 0);
            while (dr.Read())
            {
                result.Add(new AccountsForTransactions
                {
                    accountId = int.Parse(dr[0].ToString()),
                    accountName = dr[1].ToString(),
                    grp = dr[2].ToString(),
                    actype = dr[3].ToString(),
                    balance = balchk ? gg.valNum(dr[4].ToString()) : 0
                });
            }
            return result;

        }



        [HttpPost]
        [Authorize]
        [Route("api/accAccounts/accCompleteInformation")]
        public List<AccountsForTransactions> accCompleteInformation([FromBody] UserInfo usr)
        {
            AdminControl ac = new AdminControl();
            DateTime todat = ac.getPresentDateTime();
            DateTime frmdat = ac.getFinancialStart(todat, usr);
            General gg = new General();
            String quer = "";

            quer = quer + " select a.recordID,a.accname,a.Sgrp, a.AcType,case when b.balance is null then 0 else b.balance end balance from";
            quer = quer + " (select a.recordID, accname, sGrp, acType from";
            quer = quer + " (select recordid, accname, accgroup, acType from finAccounts where customerCode = " + usr.cCode + " and acChk > 0)a,";
            quer = quer + " (select recordId, sGrp from finaccgroups where customerCode = " + usr.cCode + ")b where a.accgroup = b.recordID )a left outer join";
            quer = quer + " (select accname, sum(deb-cre) balance from finexecDet where recordID in(select recordID from finexecUni where  dat >= '" + ac.strDate(frmdat) + "' and dat <= '" + ac.strDate(todat) + "'  and customerCode = " + usr.cCode + ") and customerCode = " + usr.cCode + " group by accname)b";
            quer = quer + " on a.recordid = b.accname order by a.accname";

            DataSet dss = new DataSet();
            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<AccountsForTransactions> result = new List<AccountsForTransactions>();
            var balchk = ac.screenCheck(usr, 1, 8, 3, 0);
            while (dr.Read())
            {
                result.Add(new AccountsForTransactions
                {
                    accountId = int.Parse(dr[0].ToString()),
                    accountName = dr[1].ToString(),
                    grp = dr[2].ToString(),
                    actype = dr[3].ToString(),
                    balance = balchk ? gg.valNum(dr[4].ToString()) : 0,
                    dat=ac.getPresentDateTime()
                });
            }
            return result;

        }
        [HttpPost]
        [Authorize]
        [Route("api/accAccounts/setAccount")]
        public AccountsTotal setAccount([FromBody] AccountsTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (dupCheck(tot))
                {
                    if (ac.screenCheck(tot.usr, 1, 1, 2, (int)tot.tracheck))
                    {
                        switch (tot.tracheck)
                        {
                            case 1:
                                var account = new FinAccounts();
                           //     account.Recordid = findId();
                                account.Accname = tot.acc.Accname;
                                account.Accgroup = tot.acc.Accgroup;
                                account.Address = tot.acc.Address;
                                account.Country = tot.acc.Country;
                                account.State = tot.acc.State;
                                account.District = tot.acc.District;
                                account.City = tot.acc.City;
                                account.Pin = tot.acc.Pin;
                                account.Mobile = tot.acc.Mobile;
                                account.Tel = tot.acc.Tel;
                                account.Fax = tot.acc.Fax;
                                account.Email = tot.acc.Email;
                                account.WebId = tot.acc.WebId;
                                account.AcType = tot.acc.AcType;
                                account.AcChk = tot.acc.AcChk;
                                account.BranchId = tot.usr.bCode;
                                account.CustomerCode = tot.usr.cCode;
                                db.FinAccounts.Add(account);
                                db.SaveChanges();
                                msg = "OK";
                                break;
                            case 2:
                                var accupd = db.FinAccounts.Where(a => a.Recordid == tot.acc.Recordid && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (accupd != null)
                                {
                                    accupd.Accname = tot.acc.Accname;
                                    accupd.Accgroup = tot.acc.Accgroup;
                                    accupd.Address = tot.acc.Address;
                                    accupd.Country = tot.acc.Country;
                                    accupd.State = tot.acc.State;
                                    accupd.District = tot.acc.District;
                                    accupd.City = tot.acc.City;
                                    accupd.Pin = tot.acc.Pin;
                                    accupd.Mobile = tot.acc.Mobile;
                                    accupd.Tel = tot.acc.Tel;
                                    accupd.Fax = tot.acc.Fax;
                                    accupd.Email = tot.acc.Email;
                                    accupd.WebId = tot.acc.WebId;
                                    accupd.AcType = tot.acc.AcType;
                                    accupd.AcChk = tot.acc.AcChk;

                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var accdel = db.FinAccounts.Where(a => a.Recordid == tot.acc.Recordid && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (accdel != null)
                                {
                                    db.FinAccounts.Remove(accdel);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                        }
                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }
                }
                else
                {
                    if (tot.tracheck == 3)
                        msg = "This account is in use deletion is not possible";
                    else
                        msg = "Account name already existed";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }


            tot.result = msg;
            return tot;
        }
        private int? findId()
        {
            int? x = 0;
            var xx = db.FinAccounts.FirstOrDefault();
            if (xx != null)
            {
                x = db.FinAccounts.Max(a => a.Recordid);
            }
            x++;
            return x;
        }

        private Boolean dupCheck(AccountsTotal inf)
        {
            Boolean b = false;
            switch (inf.tracheck)
            {
                case 1:
                    var x = db.FinAccounts.Where(a => a.Accname == inf.acc.Accname && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (x == null)
                    {
                        b = true;
                    }

                    break;
                case 2:
                    var y = db.FinAccounts.Where(a => a.Accname == inf.acc.Accname && a.Recordid != inf.acc.Recordid && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (y == null)
                    {
                        b = true;
                    }

                    break;
                case 3:
                    var z = db.FinexecDet.Where(a => a.Accname == inf.acc.Recordid && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (z == null)
                    {
                        b = true;
                    }
                    break;
            }
            return b;
        }

     
    }
}