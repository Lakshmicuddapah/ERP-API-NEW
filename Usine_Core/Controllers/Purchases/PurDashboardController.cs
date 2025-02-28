using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.Others;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Controllers.production;

namespace Usine_Core.Controllers.Purchases
{
    public class DashBoardDetails
    {
        public int? sno { get; set; }
        public string descr { get; set; }
        public double? fir { get; set; }
        public double? sec { get; set; }
        public int? typ { get; set; }
    }

    public class PurDashboardController : ControllerBase
    {
        AdminControl ac = new AdminControl();
        UsineContext db = new UsineContext();
       
        [HttpPost]
        [Authorize]
        [Route("api/PurDashboard/getPurDashBoardDetails")]
        public List<DashBoardDetails> getPurDashBoardDetails([FromBody] UserInfo usr)
        {
             DataBaseContext g = new DataBaseContext();
            General gg = new General();
            List<DashBoardDetails> tot = new List<DashBoardDetails>();
            string dat1 = gg.strDate( ac.getFinancialStart(ac.getPresentDateTime(), usr));
            string dat2 = gg.strDate( ac.getPresentDateTime().AddDays(1) );
            string quer = "";
            quer = quer + " select * from ";
            quer = quer + " (select 'Suppliers' descri,case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,1 typ from";
            quer = quer + " (select count(*) val1 from partyDetails where partyType = 'SUP' and customercode = " + usr.cCode + ")a,";
            quer = quer + " (select count(*) val2 from partyDetails where partyType = 'SUP' and statu = 1 and customercode = " + usr.cCode + ")b";
            quer = quer + " union all";
            quer = quer + " select 'Requests' descri,case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,2 typ from";
            quer = quer + " (select count(*) val1 from purPurchaseRequestUni where recordId not in";
            quer = quer + " (select purrequest from purpurchaseorderDet where purRequest is not null and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")";
            quer = quer + " and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select count(*) val2 from purPurchaseRequestUni where statu >= 2 and recordId not in";
            quer = quer + " (select purrequest from purpurchaseorderDet where purRequest is not null and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")";
            quer = quer + " and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b";
            quer = quer + " union all";
            quer = quer + " select 'Purchases' descri,case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,3 typ from";
            quer = quer + " (select sum(qty* rat) val1 from purpurchasesdet";
            quer = quer + " where itemid in (select recordID from invMaterials where grp";
            quer = quer + " in (select recordId from invGroups where groupCode= 'RAW' and customercode = " + usr.cCode + ")";
            quer = quer + " and customerCode = " + usr.cCode + ") and recordId in (select recordId from purPurchasesuni where";
            quer = quer + " dat >= '" + dat1 + "' and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ") and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select sum(qty * rat) val2 from purpurchasesdet";
            quer = quer + " where itemid in(select recordID from invMaterials where grp";
            quer = quer + " in  (select recordId from invGroups where groupCode <> 'RAW' and customercode = " + usr.cCode + ")";
            quer = quer + " and customerCode = " + usr.cCode + ") and recordId in (select recordId from purPurchasesuni where";
            quer = quer + " dat >= '" + dat1 + "' and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ") and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b";
            quer = quer + " union all";
            quer = quer + " select 'Supplier Credit ' descri, case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,4 typ from";
            quer = quer + " (select sum(pendingamount-returnamount - creditnote - paymentAmount) val1 from partyTransactions";
            quer = quer + " where partyid in(select recordId from partyDetails where partytype = 'SUP' and customerCode = " + usr.cCode + ")";
            quer = quer + " and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select sum(pendingamount - returnamount - creditnote - paymentAmount) val2 from partyTransactions";
            quer = quer + " where DATEDIFF(dd, dat, SYSDATETIME()) > 30 and partyid in(select recordId from partyDetails where partytype = 'SUP' and customerCode = " + usr.cCode + ")";
            quer = quer + " and customerCode = " + usr.cCode + ")b union all";
            quer = quer + " select 'Purchase Orders' descri, case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,5 typ from";
            quer = quer + " (select count(*) val1 from purPurchaseOrderUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and recordId not in";
            quer = quer + " (select refPOId from purPurchasesUni where refpoid is not null and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + "))a,";
            quer = quer + " (select count(*) val2 from purPurchaseOrderUni where validity < SYSDATETIME() and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and recordId not in";
            quer = quer + " (select refPOId from purPurchasesUni where refpoid is not null and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + "))b";
            quer = quer + " union all";
            quer = quer + " select 'Purchase Order Value' descri,case when val1 is null then 0 else val1 end val1,";
            quer = quer + " case when val2 is null then 0 else val2 end val2,6 typ from";
            quer = quer + " (select sum(totalAmt) val1 from purPurchaseOrderUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and recordId not in";
            quer = quer + " (select refPOId from purPurchasesUni where refpoid is not null and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + "))a,";
            quer = quer + " (select sum(totalAmt) val2 from purPurchaseOrderUni where validity < SYSDATETIME() and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " and recordId not in";
            quer = quer + " (select refPOId from purPurchasesUni where refpoid is not null and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + "))b";
            quer = quer + " union all";

            quer = quer + " select labe descri, amt val1,0 val2,sno typ from";
            quer = quer + " (select(year(dat) * 100) + month(dat) mont, DATENAME(MONTH, dat) labe, sum(baseamt - discount) amt, 0 rol, row_number() over(order by(year(dat) * 100) + month(dat)) + 100 sno from";
            quer = quer + " purPurchasesUni where dat >= '" + dat1 + "' and dat < '" + dat2 + "' and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + "";
            quer = quer + " group by  year(dat), month(dat), DATENAME(MONTH, dat))x";


            quer = quer + " union all";
            quer = quer + " select sGrp descri,case when val1 is null then 0 else val1 end val1,0 val2,slno + 200 typ from";
            quer = quer + " (select sGrp, row_number() over(order by sno) slno,groupCode from invGroups where customercode = " + usr.cCode + " and chk = 0)a left outer join";
            quer = quer + " (select b.groupCode, sum(valu) val1 from";
            quer = quer + " (select a.itemid, b.grp, a.itemname, a.qty* a.rat valu from";
            quer = quer + " (select* from purpurchasesdet where recordId in (select recordId from purpurchasesuni where";
            quer = quer + " dat >= '" + dat1 + "' and dat< '" + dat2 + "' and branchId = '" + usr.bCode + "' and customercode = " + usr.cCode + "))a,";
            quer = quer + " (select * from invMaterials where customercode = " + usr.cCode + ")b where a.itemId = b.recordId)a,";
            quer = quer + " (select recordId, groupCode from invGroups where customercode = " + usr.cCode + ")b where a.grp = b.recordId group by groupCode)b on a.groupCode = b.groupCode";
            quer = quer + " union all";
            quer = quer + " select top 10 vendorname descri, val1,0 val2,slno + 300 typ from";
            quer = quer + " (select vendorname, sum(totalAmt) val1, row_number() over(order by sum(totalAmt) desc) slno from purPurchasesUni where dat >= '" + dat1 + "' and dat< '" + dat2 + "'";
            quer = quer + " and branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + " group by vendorname)x";
            quer = quer + " union all";
            quer = quer + " select top 10 descri,val1,val2,slno typ from";
            quer = quer + " (select b.partyname descri, val1, 0 val2, slno + 400 slno from";
            quer = quer + " (select partyid, sum(pendingamount - returnamount - creditnote - paymentAmount) val1,";
            quer = quer + " row_number() over(order by sum(pendingamount - returnamount - creditnote - paymentAmount) desc) slno";
            quer = quer + " from partyTransactions";
            quer = quer + " where customerCode = " + usr.cCode + " group by partyid)a,";
            quer = quer + " (select * from partyDetails where partyType = 'SUP' and customercode = " + usr.cCode + ")b where a.partyId = b.recordId)x )t order by typ";

            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while(dr.Read())
            {
                tot.Add(new DashBoardDetails
                {
                    sno = 1,
                    descr = dr[0].ToString(),
                    fir = gg.valNum(dr[1].ToString()),
                    sec = gg.valNum(dr[2].ToString()),
                    typ = gg.valInt(dr[3].ToString())
                });
            }

            dr.Close();


            g.db.Close();
            return tot;
        }
    }
}
