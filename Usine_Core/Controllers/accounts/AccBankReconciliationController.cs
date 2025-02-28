using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
 
namespace Usine_Core.Controllers.Accounts
{
     public class BankTransactionInfo
    {
        public string detail { get; set; }
    }
    public class BankReconcilationTotal
    {
        public FinBankCheckings check { get; set; }
        public UserInfo usr { get; set; }
    }
    public class AccBankReconciliationController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        [HttpPost]
        [Authorize]
        [Route("api/AccBankReconciliation/getBankTransactions")]
        public BankTransactionInfo getBankTransactions([FromBody] GeneralInformation inf)
        {
            try
            {
                AdminControl am = new AdminControl();
                string dat1 = inf.frmDate;
                string dat2 = ac.strDate( DateTime.Parse(inf.toDate).AddDays(1));

                BankTransactionInfo det = new BankTransactionInfo();
                string quer = "";
                quer = quer + " select a.recordId,a.dat,onAccount,deb,cre,narration,case when b.pos is null then 0 else b.pos end pos,b.clearedDat,b.description,bankdet from";
                quer = quer + " (select a.recordId, b.dat, ltrim(rtrim(a.namevalues)) onaccount,case when valu > 0 then 0 else valu end deb,case when valu<0 then 0 else abs(valu) end cre, b.narr narration,bankdet from";
                quer = quer + " (select recordId, sum(deb - cre) valu, STUFF((";
                quer = quer + " SELECT ', ' + accname + ' ' + CAST(detail AS VARCHAR(MAX))";
                quer = quer + " FROM(";
                quer = quer + " select b.recordId, a.accname,case when b.deb > 0 then ' Dr ' + convert(varchar(10), b.deb) else ' Cr ' + convert(varchar, b.cre) end detail, b.deb, b.cre from";
                quer = quer + " (select* from finaccounts where customerCode= " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from finexecDet where recordId in";
                quer = quer + " (select recordID from finexecdet where  accname = " + inf.recordId.ToString() + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ") and accname<> " + inf.recordId.ToString() + " and customerCode = " + inf.usr.cCode + ")b where a.recordid = b.accname )x";
                quer = quer + " WHERE(recordId = Results.recordId)";
                quer = quer + " FOR XML PATH(''),TYPE).value('(./text())[1]', 'VARCHAR(MAX)')";
                quer = quer + "  ,1,2,'') AS NameValues";
                quer = quer + " FROM(";
                quer = quer + " select b.recordId, a.accname,case when b.deb > 0 then ' Dr ' + convert(varchar(10), b.deb) else ' Cr ' + convert(varchar, b.cre) end detail, b.deb, b.cre from";
                quer = quer + " (select* from finaccounts where customerCode= " + inf.usr.cCode + ")a,";
                quer = quer + " (select * from finexecDet where recordId in";
                quer = quer + " (select recordID from finexecdet where  accname = " + inf.recordId.ToString() + " and dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ") and accname<>" + inf.recordId.ToString() + "  and customerCode = " + inf.usr.cCode + ")b where a.recordid = b.accname )";
                quer = quer + " Results GROUP BY recordId)a,";
                quer = quer + " (select * from finexecuni where dat >= '" + dat1 + "' and dat< '" + dat2 + "' and customerCode = " + inf.usr.cCode + ")b where a.recordID = b.recordID )a left outer join";
                quer = quer + " (select* from finBankCheckings where customerCode= " + inf.usr.cCode + ")b on a.recordID = b.recordId order by a.dat,a.recordID";

                DataBaseContext gg = new DataBaseContext();
                General g = new General();
                SqlCommand dc = new SqlCommand();
                dc.Connection = gg.db;
                gg.db.Open();
                dc.CommandText = quer;
                SqlDataAdapter da = new SqlDataAdapter(dc);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gg.db.Close();

                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(dt);

                det.detail = JSONString;
                return det;
            }
            catch (Exception ee)
            {
                return null;
            }




        }
        [HttpPost]
        [Authorize]
        [Route("api/AccBankReconciliation/setReconciliation")]
        public TransactionResult setReconciliation([FromBody] BankReconcilationTotal tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,1,2,5,0))
                {
                    var check = db.FinBankCheckings.Where(a => a.RecordId == tot.check.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    if(check != null)
                    {
                        db.FinBankCheckings.Remove(check);
                    }
                    tot.check.ClearedDat = ac.getPresentDateTime();
                    tot.check.Usrname = tot.usr.uCode;
                    tot.check.Clearedby = tot.usr.uCode;
                    tot.check.Branchid = tot.usr.bCode;
                    tot.check.CustomerCode = tot.usr.cCode;
                    db.FinBankCheckings.Add(tot.check);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authroised for this transaction";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
    }
}
