using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;

namespace Usine_Core.Controllers.Accounts
{
    public class AccountAssigningDetails
    {
        public string transcode { get; set; }
        public string transdescri { get; set; }
        public int sno { get; set; }
        public int? account { get; set; }
    }
    public class AccountAssigningDetailsTotal
    {
        public List<AccountsAssign> list { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }

    public class AccountsAssignController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Route("api/accountsassign/GetAccountsAssignDetailsPurchases")]
        public List<AccountAssigningDetails> GetAccountsAssignDetailsPurchases([FromBody] UserInfo usr)
        {
            String quer = "";
            quer = quer + " select a.transcode,a.descr,a.sno,case when b.account is null then 0 else b.account end account from ";
            quer = quer + " (select* from (select 'pur_' + sGrp transcode, sGrp + ' Purchases' descr,  sno from invGroups where mGrp = 'MATERIALS' and customercode = " + usr.cCode.ToString() + "";

            if (usr.cCode == 11120)
            {
                quer = quer + " union all select 'pur_blanks'  transcode, 'Blanks Purchases' descr, 101 sno";
                quer = quer + " union all select 'pur_stocks'  transcode, 'Stock Purchases' descr, 102 sno";
            }
            quer = quer + " union all select 'pur_other'  transcode, 'Other amounts in Purchases' descr, 103 sno";
            quer = quer + " union all select 'pur_discount'  transcode, 'Discounts in Purchases' descr, 104 sno";
            quer = quer + " union all select 'pur_' + taxCode + '@' + convert(varchar(10), taxper) transcode, taxcode + ' @ ' + convert(varchar(10), taxper) descr,row_number() over(order by recordID) + 200 sno from admTaxes where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode.ToString() + "";
            quer = quer + " union all select 'pur_cash'  transcode,'Cash Transaction' descr, 301 sno";
            quer = quer + " union all select 'pur_bank'  transcode,'Bank Transaction' descr,302 sno";
            
            quer = quer + " union all select 'pur_mwallet'  transcode, 'Mobile Wallet Transaction' descr, 303 sno";
            quer = quer + " union all select 'pur_rebate' transcode, 'Rebates in Purchases' descr,304 sno";
            quer = quer + " union all select 'pur_cnote' transcode, 'Credit Notes' descr,305 sno";
            quer = quer + " )xx)a left outer join";
            quer = quer + " (select* from accountsAssign where module = 'PUR' and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b on a.transcode = b.transcode order by a.sno";

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            List<AccountAssigningDetails> list = new List<AccountAssigningDetails>();
            while (dr.Read())
            {
                list.Add(new AccountAssigningDetails
                {
                        transcode=dr[0].ToString(),
                        transdescri=dr[1].ToString(),
                        account=int.Parse(dr[3].ToString())
                });
            }
            dr.Close();
            g.db.Close();
            return list;
            
        }
        [HttpPost]
        [Route("api/accountsassign/SetAccountsAssignDetails")]
        public AccountAssigningDetailsTotal SetAccountsAssignDetails([FromBody]AccountAssigningDetailsTotal tot)
        {
            String msg = "";
            Boolean b = false;
            String module = "";
            if(tot.list.Count() > 0)
            {
                module = tot.list[0].Module;
            }
            switch(module)
            {
                case "PUR":
                    b = ac.screenCheck(tot.usr, 2, 8, 3, 0);
                    break;
                case "SAL":
                    b = ac.screenCheck(tot.usr, 7, 10, 3, 0);
                    break;
            }
            try
            {
                if(b)
                {
                    var det = db.AccountsAssign.Where(a => a.Module == module && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                    db.AccountsAssign.RemoveRange(det);

                    foreach(AccountsAssign acc in tot.list)
                    {
                        acc.BranchId = tot.usr.bCode;
                        acc.CustomerCode = tot.usr.cCode;
                    }
                    db.AccountsAssign.AddRange(tot.list);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised to assign accounts ";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            tot.result = msg;
            return tot;
        }
    }
}
