using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using System;
using System.Linq;

namespace Usine_Core.Controllers.Accounts
{
    public class AccountsGeneral
    {
        public int? findAccountsTransactionId()
        {
            UsineContext db = new UsineContext();
            int? x = 0;
            var xx = db.FinexecUni.FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Max(a => a.RecordId);
            }
            x++;
            return x;
        }
        public int? findAccountsSeq(DateTime dat,UserInfo usr)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = DateTime.Parse(ac.strDate(dat));
            DateTime dat2 = dat1.AddDays(1);
            int? x = 0;
            var xx = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Dat >= dat1 && a.Dat < dat2).FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Dat >= dat1 && a.Dat < dat2).Max(b => b.Seq);
            }
            x++;
            return x;
        }
    }
}
