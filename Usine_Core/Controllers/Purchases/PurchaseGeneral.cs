using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Models;
using System.Data;
using System.Data.SqlClient;

namespace Usine_Core.Controllers.Purchases
{
    public class PurSupplierWiseItemsPricesInformation
    {
        public int? sno { get; set; }
        public string seq { get; set; }
        public int? vendorid { get; set; }
        public int? itemid { get; set; }
        public string itemname { get; set; }
        public double? rat { get; set; }
        public int? umid { get; set; }
        public string um { get; set; }
        public string reference { get; set; }
        public string vendorname { get; set; }
    }
    public class PurchaseGeneral
    {
        DataBaseContext g = new DataBaseContext();
        General gg = new General();
        public List<PurSupplierWiseItemsPricesInformation> GetSuppliersLatestPriceList(UserInfo usr)
        {
            string quer = "";
            quer = quer + " select * from ";
            quer = quer + " (select 1 sno, a.seq, a.vendorid, b.itemid, b.itemname, b.rat, b.umid, b.um, 'As per quotation id ' + a.seq + ' of supplier quotation number ' + (case when a.refQuotation is null then ' '  else a.refQuotation end ) +' dated ' + dbo.strDate(a.dat) referene,vendorname from";
            quer = quer + " (select* from purQuotationUni where validity >= SYSDATETIME() and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select a.recordId,a.itemid,a.itemName,a.rat,a.um umid, b.um from(select* from purQuotationDet where branchid= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select * from invUm where customerCode = " + usr.cCode + ")b where a.um = b.recordId)b where a.recordId = b.recordId";
            quer = quer + " union all";
            quer = quer + " select 2 sno,a.seq,a.vendorid,b.itemid,b.itemname,b.rat,b.umid,b.um, 'As per invoice id ' + a.seq + ' of supplier invoice number ' + (case when a.invoiceno is null then ' '  else a.invoiceno end ) +' dated ' + dbo.strDate(a.dat) referene,vendorname from";
            quer = quer + " (select* from purpurchasesuni where recordId in";
            quer = quer + " (select max(recordId) recordId from purPurchasesUni  where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + " group by vendorid))a,";
            quer = quer + " (select a.recordId,a.itemid,a.itemName,a.rat,a.um umid, b.um from(select* from purpurchasesdet where branchid= '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select * from invUm where customerCode = " + usr.cCode + ")b where a.um = b.recordId)b where a.recordID = b.recordId)x order by sno, vendorid, itemid";
            List<PurSupplierWiseItemsPricesInformation> lst = new List<PurSupplierWiseItemsPricesInformation>();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr=dc.ExecuteReader();
            while(dr.Read())
            {
                lst.Add(new PurSupplierWiseItemsPricesInformation
                {
                    sno=gg.valInt(dr[0].ToString()),
                    seq=dr[1].ToString(),
                    vendorid=gg.valInt(dr[2].ToString()),
                    itemid=gg.valInt(dr[3].ToString()),
                    itemname=dr[4].ToString(),
                    rat=gg.valNum(dr[5].ToString()),
                    umid=gg.valInt(dr[6].ToString()),
                    um=dr[7].ToString(),
                    reference=dr[8].ToString(),
                    vendorname=dr[9].ToString()
                });
            }

          
            dr.Close();
            g.db.Close();

            return lst;

        }
    }
}
