using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Accounts;
using Usine_Core.Controllers.Admin;
 using Usine_Core.Controllers.Inventory;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.factories;
using iTextSharp.text.xml;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Usine_Core.Controllers.Purchases
{
    public class PurPurchaseOrderTotal
    {
        public PurPurchaseOrderUni header { get; set; }
        public List<PurPurchaseOrderDet> lines { get; set; }
        public List<PurPurchaseOrderTerms> terms{ get; set; }
        public List<PurPurchaseOrderTaxes> taxes { get; set; }
        public List<TotAdvanceDetails> advances { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }

    }
    public class SetPurchaseOrderTotal
    {
        public PurPurchaseOrderUni header { get; set; }
     public List<PurPurchaseOrderDet> lines { get; set; }
       public List<PurPurchaseOrderTerms> terms { get; set; }
       public List<PurPurchaseOrderTaxes> taxes { get; set; }
       public List<TotAdvanceDetails> advances { get; set; }
       public int? traCheck { get; set; }
       public String result { get; set; } 
        public UserInfo usr { get; set; }
    }
    public class OrderPrintResult
    {
        public string fname { get; set; }
        public string result { get; set; }
    }
    public class purTermTotal
    {
        public PurTerms term { get; set; }
        public String result { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurchaseRequestCompleteDetails
    {
        public int? recordId { get; set; }
        public int? sno { get; set; }
        public string seq { get; set; }
        public string dat { get; set; }
        public string employee { get; set; }
        public string department { get; set; }
        public int? itemId { get; set; }
        public string itemname { get; set; }
        public string purpose { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
    }
    public class CompletePurchaseOrderRequirements
    {
        public String pono { get; set; }
        public DateTime dat { get; set; }
        public List<AccountDetails> suppliers { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<InvMaterialDetails> materials { get; set; }
        public List<AdmTaxes> taxes { get; set; }
        public List<InvMaterialsUnitsDetails> units { get; set; }
        public List<PurTerms> terms { get; set; }
        public List<Purpurchasetypes> purtypes { get; set; }
          public dynamic discounts { get; set; }
        public List<PurchaseRequestCompleteDetails> pendingrequests { get; set; }
       public List<CrmTaxAssigningDet> taxassigns { get; set; }
        public List<MisCountryMaster> countries { get; set; }
        public List<MisStateMaster> states { get; set; }
        public List<PurSupplierWiseItemsPricesInformation> pricesinfo { get; set; }

    }
    public class PendingRequestInfo
    {
        public int? recordId { get; set; }
        public string seq { get; set; }
        public int? sno { get; set; }
        public int? itemid { get; set; }
        public string itemdescription { get; set; }
        public double? approvedQty { get; set; }
        public double? pendingQty { get; set; }
        public string department { get; set; }
    }
    public class PurchaseOrdersRequirements2
    {
        public dynamic materials { get; set; }
        public List<InvMaterialsUnitsDetails> units { get; set; }
        public dynamic prices { get; set; }
        public List<PendingRequestInfo> requests { get; set; }
        public List<PurTerms> terms { get; set; }


    
    }

    public  class PurchaseOrdersDetails2
    {
        public int? supId { get; set; }
        public string supplier { get; set; }
        public int? itemId { get; set; }
        public string itemname { get; set; }
        public int? prrequest { get; set; }
        public double? qty { get; set; }
        public double? rat { get; set; }
        public int? um { get; set; }
    }
    public class PurPurchaseOrdersTotal2
    {
        public List<PurchaseOrdersDetails2> details { get; set; }
        public List<string> terms { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CompletePurchaseOrderAdvanceTotal
    {
        public TotAdvanceDetails advance { get; set; }
        public string pono { get; set; }
        public int traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurchaseAdvanceRequirements
    {
        public string seq { get; set; }
        public List<FinAccounts> revaccounts { get; set; }

    }

    public class purPurchaseOrderController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public purPurchaseOrderController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseOrderRequirements")]
        public CompletePurchaseOrderRequirements GetPurchaseOrderRequirements([FromBody]UserInfo usr)
        {
            CompletePurchaseOrderRequirements tot = new CompletePurchaseOrderRequirements();
            PurchaseGeneral pg = new PurchaseGeneral();
            tot.pono = findSeq(usr,1);
            
            accAccountsController ac = new accAccountsController();
            AdminControl acc = new AdminControl();
            tot.dat = acc.getPresentDateTime();
            var cbalances = (from a in db.PartyTransactions.Where(a => a.CustomerCode == usr.cCode).GroupBy(b => b.PartyId)
                             select new
                             {
                                 partyid = a.Key,
                                 cbalance = a.Sum(b => b.PendingAmount - b.ReturnAmount - b.CreditNote - b.PaymentAmount)
                             }).ToList();
            var parties = (from a in db.PartyDetails.Where(a => a.Statu == 1 && a.PartyType == "SUP" && a.CustomerCode == usr.cCode)
                           select new
                           {
                               a.RecordId,
                               a.PartyName,
                               a.CrAmt,
                               a.RestrictMode

                           }).ToList();
            tot.suppliers = (from a in parties
                           join b in cbalances on a.RecordId equals b.partyid into gj
                           from subdet in gj.DefaultIfEmpty()
                           select new AccountDetails
                           {
                               accountId=a.RecordId,
                               accountname=a.PartyName,
                               availableCredit=a.CrAmt,
                               restrictMode=a.RestrictMode,
                               balanceAmt=subdet==null?0:subdet.cbalance
                           }).OrderBy(b => b.accountname).ToList();


            //tot.suppliers =
            //    (from p in db.PartyDetails.Where(a => a.Statu == 1 && a.PartyType == "SUP" && a.CustomerCode == usr.cCode)

            //     select new AccountDetails
            //     {
            //         accountId = p.RecordId,
            //         accountname = p.PartyName,
            //         availableCredit = p.CrAmt,
            //         restrictMode = p.RestrictMode
            //     }

            //     ).OrderBy(b => b.accountname).ToList();
                 
            InvMaterialsController iv = new InvMaterialsController();
            AdminTaxesController ax = new AdminTaxesController();
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
 
            tot.materials = iv.GetInvMaterials(usr).Where(a => a.statu== "Active").ToList();
            tot.taxes = ax.GetTaxes(usr);
            tot.units = iv.GetInvMaterialUnits(usr);
            tot.terms = GetPurchaseTerms(usr);
            tot.purtypes=db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
            tot.pendingrequests = new List<PurchaseRequestCompleteDetails>();
            tot.countries = (from a in db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode)
                             select new MisCountryMaster
                             {
                                 RecordId=a.RecordId,
                                 Cntname=a.Cntname,
                                 ConversionFactor=a.ConversionFactor,
                                 Curr=a.Curr,
                                 CurrSymbol=a.CurrSymbol,
                             }).OrderBy(b => b.RecordId).ToList();
            tot.states = (from a in db.MisStateMaster.Where(a => a.CustomerCode == usr.cCode)
                          select new MisStateMaster
                          {
                              RecordId=a.RecordId,
                              Cntname=a.Cntname,
                              StateName=a.StateName

                          }).OrderBy(b => b.StateName).ToList();
            DataBaseContext g = new DataBaseContext();
            string quer = "";
            quer = quer + " select a.recordId,a.sno,b.seq,dbo.strdate(b.dat) dat,b.empname,b.department,a.itemid,a.itemdescription,";
            quer = quer + " a.purpose,a.qty,a.um from";
            quer = quer + " (select a.recordId, a.sno, a.itemid, a.itemDescription, a.purpose, a.qty, b.um from";
            quer = quer + " (select* from";
            quer = quer + " (select a.recordId, a.sno, a.itemId, a.itemDescription, a.purpose, a.qty, a.um, b.purrequest from";
            quer = quer + " (select* from purPurchaseRequestDet where pos >= 2 and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a left outer join";
            quer = quer + " (select* from purPurchaseOrderDet where purRequest is not null and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b on a.recordId = b.purRequest)x where purrequest is null)a,";
            quer = quer + " (select * from invum where customerCode = " + usr.cCode + ")b where a.um = b.recordId)a,";
            quer = quer + " (select a.recordId,a.seq,a.dat,a.department,b.empname from";
            quer = quer + " (select a.recordId, a.seq, a.dat, b.department, a.empno from";
            quer = quer + " (select recordId, seq, dat, department, empno from purPurchaseRequestUni where branchId = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select * from invDepartments where customerCode = " + usr.cCode + ")b where a.department = b.recordId)a,";
            quer = quer + " (select * from hrdEmployees where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.empno = b.recordId)b where a.recordId = b.recordId";


            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            General gg = new General();
            while(dr.Read())
            {
                tot.pendingrequests.Add(new PurchaseRequestCompleteDetails
                {
                    recordId=gg.valInt(dr[0].ToString()),
                    sno=gg.valInt(dr[1].ToString()),
                    seq=dr[2].ToString(),
                    dat=dr[3].ToString(),
                    employee=dr[4].ToString(),
                    department=dr[5].ToString(),
                    itemId=gg.valInt(dr[6].ToString()),
                    itemname=dr[7].ToString(),
                    purpose=dr[8].ToString(),
                    qty=gg.valNum(dr[9].ToString()),
                    um=dr[10].ToString()
                });
            }
            dr.Close();
            g.db.Close();

            tot.pricesinfo = pg.GetSuppliersLatestPriceList(usr);
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseOrderRequirements2")]
        public CompletePurchaseOrderRequirements GetPurchaseOrderRequirements2([FromBody] UserInfo usr)
        {
            CompletePurchaseOrderRequirements tot = new CompletePurchaseOrderRequirements();
            tot.pono = findSeq(usr,2);
            accAccountsController ac = new accAccountsController();
            tot.suppliers =
                (from p in db.PartyDetails.Where(a => a.Statu == 1 && a.PartyType == "CUS" && a.CustomerCode == usr.cCode)
                 select new AccountDetails
                 {
                     accountId = p.RecordId,
                     accountname = p.PartyName,
                     
                    pricelist=p.Pricelist,
                    discountlist=p.Discountlist
                 }

                 ).OrderBy(b => b.accountname).ToList();

            InvMaterialsController iv = new InvMaterialsController();
            AdminTaxesController ax = new AdminTaxesController();


            tot.materials = iv.GetInvMaterials(usr).Where(a => a.statu == "Active").ToList();
            tot.taxes = ax.GetTaxes(usr);
            tot.units = iv.GetInvMaterialUnits(usr);
            tot.terms = GetPurchaseTerms(usr);
            tot.purtypes = db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).ToList();
            tot.taxassigns = db.CrmTaxAssigningDet.Where(a => a.CustomerCode == usr.cCode).ToList();

        /*    tot.prices = (from a in db.CrmPriceListUni.Where(a => a.CustomerCode == usr.cCode)
                          join b in db.CrmPriceListDet.Where(a => a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                          select new
                          {
                              pricename = a.PriceListName,
                              productid = b.ProductId,
                              price = b.Price,
                              taxid = b.TaxId
                          }).OrderBy(b => b.pricename).ThenBy(c => c.productid).ToList();*/
            tot.discounts= (from a in db.CrmDiscountListUni.Where(a => a.CustomerCode == usr.cCode)
                            join b in db.CrmDiscountListDet.Where(a => a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                            select new
                            {
                                discname = a.DiscountListName,
                                productid = b.ProductId,
                                price = b.Discount,
                               
                            }).OrderBy(b => b.discname).ThenBy(c => c.productid).ToList();

            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseOrders")]
        public List<PurPurchaseOrderUni> GetPurchaseOrders([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
              try
            {
                return db.PurPurchaseOrderUni.Where(a => a.TypeOfOrder ==inf.detail && a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
            }
            catch
            {
                return null;
            }

        }
        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseOrdersForApprovals")]
        public List<PurPurchaseOrderUni> GetPurchaseOrdersForApprovals([FromBody] UserInfo usr)
        {
            return db.PurPurchaseOrderUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();

        }
        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/SetPurchaseOrderForApprovals")]
        public TransactionResult SetPurchaseOrderForApprovals([FromBody] GeneralInformation inf)
        {
            string msg = "";
            var order = db.PurPurchaseOrderUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if(order != null)
            {
                order.Pos = 3;
                db.SaveChanges();
            }
            msg = "OK";
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;

        }
        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseOrder")]
        public PurPurchaseOrderTotal GetPurchaseOrder([FromBody] GeneralInformation inf)
        {
             var det = db.PurPurchasesUni.Where(a => a.RefPoid == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            //var det = db.PurPurchaseOrderUni.Where(a => a.RecordId == inf.recordId).FirstOrDefault();
            PurPurchaseOrderTotal tot = new PurPurchaseOrderTotal();
            if (det == null)
            {
                try
                {
                    tot.header = db.PurPurchaseOrderUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    tot.lines = (from a in (from p in db.PurPurchaseOrderDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                            join q in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on p.Um equals q.RecordId
                                            select new
                                            {
                                                p.RecordId,
                                                p.Sno,
                                                p.ItemId,
                                                p.ItemName,
                                                p.ItemDescription,
                                                p.Qty,
                                                Umid=p.Um,
                                                Um=q.Um,
                                                p.Rat,
                                                p.ReqdBy,
                                                p.PurRequest
                                            })
                                 join b in db.PurPurchaseRequestUni.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode) on a.PurRequest equals b.RecordId
                                 into gj
                                 from subdet in gj.DefaultIfEmpty()
                                 select new PurPurchaseOrderDet
                                 {
                                     RecordId=a.RecordId,
                                     Sno=a.Sno,
                                     ItemId=a.ItemId,
                                     ItemName=a.ItemName,
                                     ItemDescription=a.Um,
                                     Qty=a.Qty,
                                     Um=a.Umid,
                                     ReqdBy=a.ReqdBy,
                                     Rat=a.Rat,
                                     PurRequest=a.PurRequest,
                                     BranchId=subdet==null?" ":subdet.Seq
                                 }).OrderBy(b => b.Sno).ToList();
                    tot.terms = db.PurPurchaseOrderTerms.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                    tot.taxes = db.PurPurchaseOrderTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                    tot.advances = db.TotAdvanceDetails.Where(a => a.TransactionId == inf.recordId && a.Tratype == "PUR_VOU" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();

                    tot.result = "OK";

                }
                catch (Exception ee)
                {
                    tot.result = ee.Message;
                }
            }
            else
            {
                tot.result = "Purchase against this order is already over";
            }
            return tot;

        }


        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/PurOrderRequirements2")]
        public PurchaseOrdersRequirements2 PurOrderRequirements2([FromBody]UserInfo usr)
        {
            InvMaterialsController iv = new InvMaterialsController();
            PurchaseOrdersRequirements2 tot = new PurchaseOrdersRequirements2();
            PurchaseGeneral pg = new PurchaseGeneral();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 = DateTime.Now;
            tot.materials = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                             join b in (from p in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.ItemName)
                                        select new
                                        {
                                            materail = p.Key,
                                            qty = p.Sum(b => b.Qtyin - b.Qtyout)
                                        }) on a.RecordId equals b.materail into gj
                             from subdet in gj.DefaultIfEmpty()

                             select new
                             {
                                 itemid = a.RecordId,
                                 itemname = a.Itemname,
                                 grpname = a.Grpname,
                                 stock = subdet == null ? 0 : subdet.qty,
                                 um = a.Um,
                             }).OrderBy(b => b.itemname).ToList();

            tot.units = iv.GetInvMaterialUnits(usr);
           /*tot.prices = (from p in (from a in db.PurQuotationUni.Where(a => a.Validity >= dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                           join b in db.PurQuotationDet.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                           select new
                           {
                               a.Vendorid,
                               a.Vendorname,
                               b.ItemId,
                               b.ItemName,
                               b.Rat,
                               b.Um,
                               
                           }) join q in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on p.Um equals q.RecordId
                          select new
                          {
                              p.Vendorid,
                              p.Vendorname,
                              p.ItemId,
                              p.ItemName,
                              p.Rat,
                              umid=p.Um,
                              um=q.Um
                          }).OrderBy(c => c.Rat).ThenBy(d => d.ItemId).ToList();*/

             tot.prices= pg.GetSuppliersLatestPriceList(usr);

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            string quer = "";

            General gg = new General();
            quer = quer + " select a.recordId,a.seq,a.sno,itemid,itemdescription,approvedQty,pendingQty,b.department from";
            quer = quer + " (select b.recordId, a.seq, b.sno, b.itemid, b.itemdescription, approvedQty, pendingQty, b.department from";
            quer = quer + " (select* from PurPurchaseRequestuni where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a,";
            quer = quer + " (select recordId, sno, a.itemid, a.itemDescription, approvedQty, approvedQty -case when b.qty is null then 0 else b.qty end pendingQty,department from";
            quer = quer + " (select* from PurPurchaseRequestDet where pos>= 2 and branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")a left outer join";
            quer = quer + " (select purRequest, itemid, sum(qty) qty from purpurchaseorderdet  where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + " group by purRequest, itemid)b on a.recordId = b.purRequest and a.itemid = b.itemid)b where a.recordId = b.recordId)a,";
            quer = quer + " (select * from invDepartments where branchid = '" + usr.bCode + "' and customerCode = " + usr.cCode + ")b where a.department = b.recordId order by a.recordId";
            dc.CommandText = quer;
            tot.requests = new List<PendingRequestInfo>();
            SqlDataReader dr = dc.ExecuteReader();
            while(dr.Read())
            {
                tot.requests.Add(new PendingRequestInfo
                {
                    recordId=gg.valInt(dr[0].ToString()),
                    seq=dr[1].ToString(),
                    sno=gg.valInt(dr[2].ToString()),
                    itemid=gg.valInt(dr[3].ToString()),
                    itemdescription=dr[4].ToString(),
                    approvedQty=gg.valNum(dr[5].ToString()),
                    pendingQty=gg.valNum(dr[6].ToString()),
                    department=dr[7].ToString()
                });
            }
            dr.Close();
            g.db.Close();
            tot.terms = db.PurTerms.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
             return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/SetPurchaseOrder2")]
        public TransactionResult SetPurchaseOrder2([FromBody] PurPurchaseOrdersTotal2 tot)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(tot.usr,2,3,4,1))
                {
                    var suppliers = (from a in tot.details
                                     select new
                                     {
                                         supid = a.supId,
                                         supplier = a.supplier
                                     }).Distinct().ToList();
                    UsineContext db1 = new UsineContext();
                    foreach(var sup in suppliers)
                    {
                        var supp = db1.PartyDetails.Where(a => a.RecordId == sup.supid).FirstOrDefault();
                        PurPurchaseOrderUni header = new PurPurchaseOrderUni();
                        header.Seq = findSeq(tot.usr,1);
                        header.Dat = ac.getPresentDateTime();
                        header.Usr = tot.usr.uCode;
                        header.Vendorname = sup.supplier;
                        header.Vendorid = sup.supid;
                        if(supp != null)
                        {
                            
                            header.TypeOfOrder = "PO";
                           
                        }
                        db.PurPurchaseOrderUni.Add(header);
                        db.SaveChanges();
                        var lines = tot.details.Where(a => a.supId == sup.supid).ToList();
                        header.Baseamt = lines.Sum(a => (a.qty * a.rat));
                        header.TotalAmt = header.Baseamt;
                        header.Pos = 1;
                        header.PrintCount = 0;
                        header.BranchId = tot.usr.bCode;
                        header.CustomerCode = tot.usr.cCode;
                        db.SaveChanges();
                        int sno = 1;
                        List<PurPurchaseOrderDet> mlines = new List<PurPurchaseOrderDet>();
                        foreach(var line in lines)
                        {
                            mlines.Add(new PurPurchaseOrderDet
                            {
                                RecordId = header.RecordId,
                                Sno = sno,
                                ItemId=line.itemId,
                                ItemName=line.itemname,
                                ItemDescription="",
                                Qty=line.qty,
                                Um=line.um,
                                Rat=line.rat,
                                BranchId=tot.usr.bCode,
                                CustomerCode=tot.usr.cCode,
                                PurRequest=line.prrequest
                            }) ;
                            sno++;
                        }
                        db.PurPurchaseOrderDet.AddRange(mlines);
                        db.SaveChanges();

                    }
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised fot this transaction";
                }
            }
            catch(Exception ex)
            {
                msg = ex.Message;
            }

            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }



      
             [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/SetPurchaseOrder")]
        public TransactionResult SetPurchaseOrder([FromBody] PurPurchaseOrderTotal tot)
        {

            String msg = "";
            AdminControl ac = new AdminControl();

            if (ac.screenCheck(tot.usr, 2, 2, 5, (int)tot.traCheck))
            {
                int? traCheck = tot.traCheck;
                General gg = new General();
                var statuscheck = db.PurSetup.Where(a => a.SetupCode == "pur_ord" && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                int? statu = 2;
                if (statuscheck != null)
                {
                    statu = gg.valInt(statuscheck.SetupValue) == 1 ? 2 : 1;
                }
                 
                try
                {

                    String tramsg = ac.transactionCheck("Inventory", tot.header.Dat, tot.usr);
                    if (tramsg == "OK")
                    {
                        switch (traCheck)
                        {
                            case 1:

                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.header.Dat = ac.getPresentDateTime();
                                        /*  tot.header.RecordId = header.RecordId;*/
                                        tot.header.Seq = findSeq(tot.usr, 1);
                                        tot.header.BranchId = tot.usr.bCode;
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        tot.header.Pos = statu;
                                        tot.header.PrintCount = 0;
                                        db.PurPurchaseOrderUni.Add(tot.header);
                                        db.SaveChanges();
                                        int sno = 1;
                                        foreach (PurPurchaseOrderDet line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        if (tot.lines.Count() > 0)
                                        {
                                            db.PurPurchaseOrderDet.AddRange(tot.lines);
                                        }
                                        sno = 1;
                                        if (tot.terms != null)
                                        {
                                            if (tot.terms.Count() > 0)
                                            {
                                                foreach (PurPurchaseOrderTerms term in tot.terms)
                                                {
                                                    term.RecordId = tot.header.RecordId;
                                                    term.Sno = sno;
                                                    term.BranchId = tot.usr.bCode;
                                                    term.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.PurPurchaseOrderTerms.AddRange(tot.terms);

                                            }
                                        }

                                        sno = 1;
                                        if (tot.taxes != null)
                                        {
                                            if (tot.taxes.Count > 0)
                                            {

                                                foreach (PurPurchaseOrderTaxes tax in tot.taxes)
                                                {
                                                    tax.RecordId = tot.header.RecordId;
                                                    tax.Sno = sno;
                                                    tax.BranchId = tot.usr.bCode;
                                                    tax.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.PurPurchaseOrderTaxes.AddRange(tot.taxes);
                                            }
                                        }



                                        TransactionsAudit aud = new TransactionsAudit();
                                        aud.TraId = tot.header.RecordId;
                                        var amt1 = tot.header.TotalAmt;
                                        aud.Descr = "A Purchase order of " + amt1.ToString() + " has been created";
                                        aud.Usr = tot.usr.uCode;
                                        aud.Tratype = 1;
                                        aud.Transact = "PUR_ORD";
                                        aud.TraModule = "PUR";
                                        aud.Syscode = " ";
                                        aud.BranchId = tot.usr.bCode;
                                        aud.CustomerCode = tot.usr.cCode;
                                        aud.Dat = ac.getPresentDateTime();
                                        db.TransactionsAudit.Add(aud);


                                        db.SaveChanges();

                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        txn.Rollback();
                                        msg = ee.Message;
                                    }
                                }


                                break;
                            case 2:
                                var headerupd = db.PurPurchaseOrderUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                headerupd.Usr = tot.usr.uCode;
                                headerupd.PurchaseType = tot.header.PurchaseType;
                                headerupd.RefQuotation = tot.header.RefQuotation;
                                headerupd.Validity = ac.DateAdjustFromFrontEnd((DateTime)tot.header.Validity);
                                headerupd.Reference = tot.header.Reference;
                                headerupd.Vendorid = tot.header.Vendorid;
                                headerupd.Vendorname = tot.header.Vendorname;
                                headerupd.Addr = tot.header.Addr;
                                headerupd.Country = tot.header.Country;
                                headerupd.Stat = tot.header.Stat;
                                headerupd.District = tot.header.District;
                                headerupd.City = tot.header.City;
                                headerupd.Zip = tot.header.Zip;
                                headerupd.Mobile = tot.header.Mobile;
                                headerupd.Tel = tot.header.Tel;
                                headerupd.Fax = tot.header.Fax;
                                headerupd.Email = tot.header.Email;
                                headerupd.Webid = tot.header.Webid;
                                headerupd.Baseamt = tot.header.Baseamt;
                                headerupd.Discount = tot.header.Discount;
                                headerupd.Taxes = tot.header.Taxes;
                                headerupd.Others = tot.header.Others;
                                headerupd.TotalAmt = tot.header.TotalAmt;
                                headerupd.CountryId = tot.header.CountryId;
                                headerupd.ConversionFactor = tot.header.ConversionFactor;


                                var linesupd = db.PurPurchaseOrderDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                int? slno = linesupd.Count() > 0 ? linesupd.Max(b => b.Sno) : 0;
                                slno++;
                                db.PurPurchaseOrderDet.RemoveRange(linesupd);
                                foreach (PurPurchaseOrderDet line in tot.lines)
                                {
                                    line.RecordId = tot.header.RecordId;
                                    line.Sno = slno;
                                    line.BranchId = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    slno++;
                                }
                                if (tot.lines.Count() > 0)
                                {
                                    db.PurPurchaseOrderDet.AddRange(tot.lines);
                                }

                                var termsupd = db.PurPurchaseOrderTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                slno = termsupd.Count() > 0 ? termsupd.Max(b => b.Sno) : 0;
                                slno++;
                                if (termsupd.Count() > 0)
                                {
                                    db.PurPurchaseOrderTerms.RemoveRange(termsupd);
                                }
                                if (tot.terms != null)
                                {
                                    if (tot.terms.Count() > 0)
                                    {
                                        foreach (PurPurchaseOrderTerms term in tot.terms)
                                        {
                                            term.RecordId = tot.header.RecordId;
                                            term.Sno = slno;
                                            term.BranchId = tot.usr.bCode;
                                            term.CustomerCode = tot.usr.cCode;
                                            slno++;
                                        }
                                        db.PurPurchaseOrderTerms.AddRange(tot.terms);

                                    }
                                }

                                var taxesupd = db.PurPurchaseOrderTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                slno = taxesupd.Count() > 0 ? taxesupd.Max(b => b.Sno) : 0;
                                slno++;
                                if (taxesupd.Count() > 0)
                                {
                                    db.PurPurchaseOrderTaxes.RemoveRange(taxesupd);
                                }
                                if (tot.taxes != null)
                                {
                                    if (tot.taxes.Count > 0)
                                    {

                                        foreach (PurPurchaseOrderTaxes tax in tot.taxes)
                                        {
                                            tax.RecordId = tot.header.RecordId;
                                            tax.Sno = slno;
                                            tax.BranchId = tot.usr.bCode;
                                            tax.CustomerCode = tot.usr.cCode;
                                            slno++;
                                        }
                                        db.PurPurchaseOrderTaxes.AddRange(tot.taxes);
                                    }
                                }


                                TransactionsAudit aud1 = new TransactionsAudit();
                                aud1.TraId = tot.header.RecordId;
                                var amt = tot.header.TotalAmt;
                                aud1.Descr = "A Purchase order of seq " + tot.header.Seq  + amt.ToString() + " has been modified";
                                aud1.Usr = tot.usr.uCode;
                                aud1.Tratype = 2;
                                aud1.Transact = "PUR_ORD";
                                aud1.TraModule = "PUR";
                                aud1.Syscode = " ";
                                aud1.BranchId = tot.usr.bCode;
                                aud1.CustomerCode = tot.usr.cCode;
                                aud1.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud1);

                                TransactionsAudit aud2 = new TransactionsAudit();
                                aud2.TraId = tot.header.RecordId;
                                var amt2 = tot.header.TotalAmt;
                                aud2.Descr = "A Purchase order of " + amt2.ToString() + " has been created";
                                aud2.Usr = tot.usr.uCode;
                                aud2.Tratype = 1;
                                aud2.Transact = "PUR_ORD";
                                aud2.TraModule = "PUR";
                                aud2.Syscode = " ";
                                aud2.BranchId = tot.usr.bCode;
                                aud2.CustomerCode = tot.usr.cCode;
                                aud2.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud2);

                                db.SaveChanges();
                                msg = "OK";

                                break;
                            case 3:
                                var headerdel= db.PurPurchaseOrderUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var linesdel = db.PurPurchaseOrderDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var termsdel = db.PurPurchaseOrderTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var taxesdel = db.PurPurchaseOrderTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (taxesdel.Count() > 0)
                                {
                                    db.PurPurchaseOrderTaxes.RemoveRange(taxesdel);
                                }
                                if(termsdel.Count() > 0)
                                {
                                    db.PurPurchaseOrderTerms.RemoveRange(termsdel);
                                }
                                if(linesdel.Count() > 0)
                                {
                                    db.PurPurchaseOrderDet.RemoveRange(linesdel);
                                }
                                if(headerdel != null)
                                {
                                    db.PurPurchaseOrderUni.Remove(headerdel);
                                }

                                TransactionsAudit aud3= new TransactionsAudit();
                                aud3.TraId = tot.header.RecordId;
                                var amt3 = tot.header.TotalAmt;
                                aud3.Descr = "A Purchase order of " + amt3.ToString() + " has been created";
                                aud3.Usr = tot.usr.uCode;
                                aud3.Tratype = 1;
                                aud3.Transact = "PUR_ORD";
                                aud3.TraModule = "PUR";
                                aud3.Syscode = " ";
                                aud3.BranchId = tot.usr.bCode;
                                aud3.CustomerCode = tot.usr.cCode;
                                aud3.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud3);

                                db.SaveChanges();
                                msg = "OK";



                                break;
                        }
                    }
                    else
                    {
                        msg = tramsg;
                    }
                }
                catch (Exception ee)
                {
                    msg = ee.Message;
                }
            }
            else
            {
                msg = "You are not authorised for this transaction";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;

            
        }
        private String findSeq(UserInfo usr,int typ)
        {
            UsineContext db = new UsineContext();
            General g = new General();
            int x = 0;
            AdminControl ac = new AdminControl();
            DateTime dat = ac.getPresentDateTime();
             
            string str =  (typ==1?"PO":"SO") + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/";
            var xx = db.PurPurchaseOrderUni.Where(a => a.Seq.Contains(str)).Max(b => b.Seq);
            if(xx!=null)
            {
                x = g.valInt(g.right(xx, 4));
            }
            x++;
            return str + g.zeroMake(x, 4);
        }





        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseTerms")]
        public List<PurTerms> GetPurchaseTerms([FromBody]UserInfo usr)
        {
            return db.PurTerms.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/SetPurchaseTerm")]
        public purTermTotal SetPurchaseTerm([FromBody]purTermTotal tot)
        {
            String msg = "";
            try
            {
                AdminControl ac = new AdminControl();
                switch (tot.traCheck)
                {
                    case 1:
                        
                        if(ac.screenCheck(tot.usr,2,8,2,0))
                        {
                            PurTerms term = new PurTerms();
                           
                            term.Term = tot.term.Term;
                            term.BranchId = tot.usr.bCode;
                            term.CustomerCode = tot.usr.cCode;
                            db.PurTerms.Add(term);
                            db.SaveChanges();
                            msg="OK";
                        }
                        else
                        {
                            msg = "You are not authorised to Create term";
                        }
                        break;
                    case 3:
                        
                        if (ac.screenCheck(tot.usr, 2, 8, 2, 0))
                        {
                            var ter = db.PurTerms.Where(a => a.Slno == tot.term.Slno  && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if(ter != null)
                            {
                                db.PurTerms.Remove(ter);
                                db.SaveChanges();
                                msg = "OK";
                            }
                        }
                        else
                        {
                            msg = "You are not authorised to Delete term";
                        }
                        break;
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/GetPurchaseAdvanceRequirements")]
        public PurchaseAdvanceRequirements GetPurchaseAdvanceRequirements([FromBody] UserInfo usr)
        {
            PurchaseAdvanceRequirements requirements = new PurchaseAdvanceRequirements();
         //  requirements.seq = findSeq(usr);
            requirements.revaccounts = db.FinAccounts.Where(a => (a.AcType == "CAS" || a.AcType == "BAN" || a.AcType == "MOB") && a.CustomerCode==usr.cCode).OrderBy(b => b.Accname).ToList();
            return requirements;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/setPurchaseOrderAdvance")]
        public TransactionResult setPurchaseOrderAdvance(CompletePurchaseOrderAdvanceTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            GeneralInformation inf = new GeneralInformation();
            try
            {
                switch(tot.traCheck)
                {
                    case 1:
                        if(ac.screenCheck(tot.usr,2,3,4,11))
                        {
                            using (var txn = db.Database.BeginTransaction())
                            {
                                try
                                {
                                    var porder = db.PurPurchaseOrderUni.Where(a => a.RecordId == tot.advance.TransactionId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                    string poseq = "";
                                    int? partyid = null;
                                    if (porder != null)
                                    {
                                        poseq = porder.Seq;
                                        partyid = porder.Vendorid;
                                    }

                                    TotAdvanceDetails adva = new TotAdvanceDetails();

                                    adva.Seq = findAdvanceSeq(tot.usr);
                                    adva.TransactionId = tot.advance.TransactionId;
                                    adva.Tratype = "PUR_ADV";
                                    adva.Dat = tot.advance.Dat;
                                    adva.Amt = tot.advance.Amt;
                                    adva.Paymentmode = tot.advance.Paymentmode;
                                    adva.Remarks = tot.advance.Remarks;
                                    adva.Bankdetails = tot.advance.Bankdetails;
                                    adva.UsrName = tot.usr.uCode;
                                    adva.BranchId = tot.usr.bCode;
                                    adva.CustomerCode = tot.usr.cCode;
                                    adva.PartyId = partyid;
                                    adva.AccountId = tot.advance.AccountId;
                                    adva.Detail1 = tot.advance.Detail1;
                                    adva.Detail2 = tot.advance.Detail2;
                                    adva.Detail3 = tot.advance.Detail3;
                                    adva.PrintCount = 0;
                                    db.TotAdvanceDetails.Add(adva);
                                    db.SaveChanges();
                                    
                                    inf.frmDate = ac.strDate(tot.advance.Dat.Value);
                                    inf.usr = tot.usr;

                                    if(ac.transactionCheck("Accounts",tot.advance.Dat,tot.usr)=="OK")
                                    {
                                        var party = db.AccountsAssign.Where(a => a.Transcode == "PUR_VOU" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                        if (party != null)
                                        {
                                            var finheader = new FinexecUni();
                                            finheader.Dat = tot.advance.Dat;
                                            finheader.Seq = findAccountSeq(inf);
                                            finheader.Narr = "Advance paid to supplier for purchase order " + poseq;
                                            finheader.Tratype = "PUR_ADV";
                                            finheader.Traref = tot.advance.TransactionId.ToString();
                                            finheader.Vouchertype = "Payment";
                                            finheader.Branchid = tot.usr.bCode;
                                            finheader.CustomerCode = tot.usr.cCode;
                                            finheader.Usr = tot.usr.uCode;
                                            finheader.PrintCount = 0;
                                            db.FinexecUni.Add(finheader);
                                            db.SaveChanges();

                                            var finlines = new List<FinexecDet>();
                                            finlines.Add(new FinexecDet
                                            {
                                                RecordId = finheader.RecordId,
                                                Sno=1,
                                                Accname=party.Account,
                                                Cre=0,
                                                Deb=tot.advance.Amt,
                                                Branchid=tot.usr.bCode,
                                                CustomerCode=tot.usr.cCode,
                                                Dat=tot.advance.Dat
                                            }) ;
                                            finlines.Add(new FinexecDet
                                            {
                                                RecordId = finheader.RecordId,
                                                Sno = 2,
                                                Accname =tot.advance.AccountId,
                                                Cre = tot.advance.Amt,
                                                Deb = 0,
                                                Branchid = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode,
                                                Dat = tot.advance.Dat
                                            });
                                        }
                                    }

                                    TransactionsAudit aud = new TransactionsAudit();
                                    aud.TraId = adva.RecordId;
                                    var amt = tot.advance.Amt;
                                    aud.Descr = "An advance  of " + amt.ToString() + " paid to Purchase order " + tot.pono;
                                    aud.Usr = tot.usr.uCode;
                                    aud.Tratype = 1;
                                    aud.Transact = "PUR_ADV";
                                    aud.TraModule = "PUR";
                                    aud.Syscode = " ";
                                    aud.BranchId = tot.usr.bCode;
                                    aud.CustomerCode = tot.usr.cCode;
                                    aud.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(aud);
                                    db.SaveChanges();


                                    msg = "OK";
                                }
                                catch(Exception ee)
                                {
                                    txn.Rollback();
                                    msg = ee.Message;
                                }
                            }
                            
                        }
                        else
                        {
                            msg = "You are not authorised to add advances";
                        }
                        break;
                    case 2:
                        if (ac.screenCheck(tot.usr, 2, 3, 4, 12))
                        {
                            var adv = db.TotAdvanceDetails.Where(a => a.RecordId == tot.advance.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if(adv != null)
                            {
                                var amt = adv.Amt;
                                var paymode = adv.Paymentmode;
                                adv.Amt = tot.advance.Amt;
                                adv.Paymentmode = tot.advance.Paymentmode;
                                adv.Remarks = tot.advance.Remarks;
                                adv.Bankdetails = tot.advance.Bankdetails;

                                TransactionsAudit aud = new TransactionsAudit();
                                aud.TraId = adv.RecordId;

                                aud.Descr = "An advance of " + adv.Seq + " has been changed  from " + amt.ToString() + " to " + tot.advance.Amt.ToString();
                                aud.Usr = tot.usr.uCode;
                                aud.Tratype = 2;
                                aud.Transact = "PUR_ADV";
                                aud.TraModule = "PUR";
                                aud.Syscode = " ";
                                aud.BranchId = tot.usr.bCode;
                                aud.CustomerCode = tot.usr.cCode;
                                aud.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud);
                                db.SaveChanges();

                                msg = "OK";
                            }
                            else
                            {
                                msg = "No record found";
                            }
                          
                        }
                        else
                        {
                            msg = "You are not authorised to modify advance";
                        }
                            break;
                    case 3:
                        if (ac.screenCheck(tot.usr, 2, 3, 4, 13))
                        {
                            var adv = db.TotAdvanceDetails.Where(a => a.RecordId == tot.advance.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            if (adv != null)
                            {
                                var amt = adv.Amt;
                                db.TotAdvanceDetails.Remove(adv);
                               
                                TransactionsAudit aud = new TransactionsAudit();
                                aud.TraId = adv.RecordId;

                                aud.Descr = "An advance of " + adv.Seq + " has been deleted  of amount " + tot.advance.Amt.ToString();
                                aud.Usr = tot.usr.uCode;
                                aud.Tratype = 3;
                                aud.Transact = "PUR_ADV";
                                aud.TraModule = "PUR";
                                aud.Syscode = " ";
                                aud.BranchId = tot.usr.bCode;
                                aud.CustomerCode = tot.usr.cCode;
                                aud.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud);
                                db.SaveChanges();

                                msg = "OK";
                            }
                            else
                            {
                                msg = "No record found";
                            }
                        }
                        else
                        {
                            msg = "You are not authorised to delete advance";
                        }

                            break;
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
        private String findAdvanceSeq(UserInfo usr)
        {
            int x = 0;
            General g = new General();
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart( DateTime.Now,usr);
            DateTime dat2 = dat1.AddYears(1);
            var se = db.TotAdvanceDetails.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(se != null)
            {
                x = int.Parse(se.Substring(10, 5));
            }
            x++;
            return "ADVO" + dat1.Year.ToString() + g.zeroMake(x, 5);
        }


     



        [HttpPost]
        [Authorize]
        [Route("api/purPurchaseOrders/printPurchaseOrder")]


        public OrderPrintResult printPurchaseOrder([FromBody] GeneralInformation inf)
        {
            string msg = "";
            OrderPrintResult result = new OrderPrintResult();
            DateTime dats = DateTime.Now;
            string dat = ac.strDate(dats) + dats.Hour.ToString() + dats.Minute.ToString() + dats.Second.ToString();
            string fname = inf.usr.uCode + "PurchaseOrder" + dat + inf.usr.cCode + inf.usr.bCode + ".pdf";
            String filename = ho.WebRootPath + "\\Reps\\" + fname;
            LoginControlController ll = new LoginControlController();
            var addr = ll.makeBranchAddress(inf.usr.bCode, inf.usr.cCode);

            var header = db.PurPurchaseOrderUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();

            var lines = (from p in db.PurPurchaseOrderDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join q in db.InvUm.Where(a => a.CustomerCode==inf.usr.cCode) on p.Um equals q.RecordId
                         select new 
                         {
                             sno = p.Sno,
                             product = p.ItemName,
                             description =p.ItemDescription,
                             qty = p.Qty,
                             um=q.Um,
                             rat = p.Rat,
                             valu = p.Qty * p.Rat
                         }
                        ).OrderBy(a => a.sno).ToList();

            var notes = db.PurPurchaseOrderTerms.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
            var taxes = db.PurPurchaseOrderTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Taxvalue > 0).OrderBy(b => b.Sno).ToList();




            using (FileStream ms = new FileStream(filename, FileMode.Append, FileAccess.Write))

            {
                Document document = new Document(PageSize.A4, 75f, 40f, 20f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);


                document.Open();
                double pagelines = 33;
                int pageno = 1;
                var totalpages = Math.Ceiling(lines.Count() / pagelines);
                General gg = new General();
                PdfPTable ptot1;
                int sno = 1;
                while (pageno <= totalpages)
                {
                    ptot1 = new PdfPTable(1);
                    float[] widths1 = new float[] { 550f };
                    ptot1.SetWidths(widths1);
                    ptot1.TotalWidth = 550f;
                    ptot1.LockedWidth = true;

                    PdfPCell plC = new PdfPCell(makeOrderHeader(header, pageno, totalpages,addr));
                    plC.BorderWidth = 1f;

                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);


                    int skipIndex = int.Parse((pagelines * (pageno - 1)).ToString());
                    var sublines = lines.Skip(skipIndex).Take(int.Parse(pagelines.ToString()));
                    iTextSharp.text.Font fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));

                    PdfPTable pcombo = new PdfPTable(6);
                    pcombo.SetWidths(new float[] { 40f, 260f,50f,50f, 70f, 80f });
                    pcombo.TotalWidth = 550f;
                    pcombo.LockedWidth = true;

                    foreach (var ln in sublines)
                    {
                        plC = new PdfPCell(new Phrase(sno.ToString(), fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);
                        plC = new PdfPCell(new Phrase(ln.product, fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);

                        plC = new PdfPCell(new Phrase(ln.qty.ToString(), fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);
                        plC = new PdfPCell(new Phrase(ln.um, fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);
                        plC = new PdfPCell(new Phrase(gg.fixCur( (double)ln.rat,2), fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);
                        plC = new PdfPCell(new Phrase(gg.fixCur((double)ln.valu, 2), fn));
                        plC.BorderWidth = 0f;
                        plC.BorderWidthRight = 1f;
                        plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                        plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                        pcombo.AddCell(plC);
 
                        sno++;
                    }

                    if (sublines.Count() < pagelines)
                    {
                        int linecount = sublines.Count();
                        while (linecount <= pagelines)
                        {
                            for (var i = 0; i < 6; i++)
                            {
                                plC = new PdfPCell(new Phrase(" ", fn));
                                plC.BorderWidth = 0f;
                                plC.BorderWidthRight = 1f;
                                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                pcombo.AddCell(plC);
                            }
                            linecount++;
                        }
                    }

                    plC = new PdfPCell(pcombo);
                    plC.BorderWidth = 1f;




                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);
                    

                    plC = new PdfPCell(makeFooter(header, notes, taxes, lines.Sum(a => a.qty),addr));
                    plC.BorderWidth = 1f;

                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    ptot1.AddCell(plC);


                    document.Add(ptot1);

                    document.NewPage();
                    pageno++;
                }


                document.Close();
            }


            result.result = msg;
            result.fname = fname;
            header.PrintCount = header.PrintCount + 1;
            db.SaveChanges();
            return result;

        }
        private int? findAccountSeq(GeneralInformation inf)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = DateTime.Parse( inf.frmDate);
            DateTime dat2 = dat1.AddDays(1);
            int? x = 0;
            var xx = db.FinexecUni.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Dat >= dat1 && a.Dat < dat2).FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Where(a => a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode && a.Dat >= dat1 && a.Dat < dat2).Max(b => b.Seq);
            }
            x++;
            return x;
        }

        private PdfPTable makeFooter(PurPurchaseOrderUni header, List<PurPurchaseOrderTerms> notes, List<PurPurchaseOrderTaxes> taxes, double? qty,UserAddress addr)
        {
            General g = new General();

            PdfPTable pleft = new PdfPTable(1);
            pleft.SetWidths(new float[] { 300f });
            pleft.TotalWidth = 300f;
            pleft.LockedWidth = true;
            iTextSharp.text.Font fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.BOLD));
            PdfPCell plC;
            if (notes.Count() > 0)
            {
                plC = new PdfPCell(new Phrase("Terms", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_LEFT;
                pleft.AddCell(plC);
            }
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 8, Font.NORMAL));
            foreach (PurPurchaseOrderTerms note in notes)
            {
                plC = new PdfPCell(new Phrase(note.Term, fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pleft.AddCell(plC);
            }

            PdfPTable pright = new PdfPTable(2);
            pright.SetWidths(new float[] { 100f, 150f });
            pright.TotalWidth = 250f;
            pright.LockedWidth = true;

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
          /*  plC = new PdfPCell(new Phrase("Total Blanks in Pc", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
            plC = new PdfPCell(new Phrase(qty.ToString(), fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);*/

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
            plC = new PdfPCell(new Phrase("Base Amount", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
            plC = new PdfPCell(new Phrase(g.makeCur((double)header.Baseamt, 2), fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            if (header.Discount > 0)
            {
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
                plC = new PdfPCell(new Phrase("Discount", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
                plC = new PdfPCell(new Phrase(g.makeCur((double)header.Discount, 2), fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
            }

            /* if (header.Taxes > 0)
             {
                 fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
                 plC = new PdfPCell(new Phrase("Taxes", fn));
                 plC.BorderWidth = 0f;
                 plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                 plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                 pright.AddCell(plC);
                 fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
                 plC = new PdfPCell(new Phrase(g.fixCur((double)header.Taxes, 2), fn));
                 plC.BorderWidth = 0f;
                 plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                 plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                 pright.AddCell(plC);
             }*/

            var taxgroups = taxes.GroupBy(a => new { a.Taxcode, a.Taxper }).ToList();
            for (var i = 0; i < taxgroups.Count(); i++)
            {
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
                plC = new PdfPCell(new Phrase(taxgroups[i].Key.Taxcode + " @ " + taxgroups[i].Key.Taxper.ToString() + "%", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
                plC = new PdfPCell(new Phrase(g.makeCur((double)taxgroups[i].Sum(a => a.Taxvalue), 2), fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
            }

            if (header.Others > 0)
            {
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
                plC = new PdfPCell(new Phrase("Other Charges", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
                plC = new PdfPCell(new Phrase(g.makeCur((double)header.Others, 2), fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                pright.AddCell(plC);
            }

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
            plC = new PdfPCell(new Phrase("Total Amount", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(g.makeCur((double)header.TotalAmt, 2), fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
           
            PdfPTable ptot = new PdfPTable(2);
            float[] widths = new float[] { 300f, 250f };
            ptot.SetWidths(widths);
            ptot.TotalWidth = 550f;
            ptot.LockedWidth = true;

            plC = new PdfPCell(pleft);
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(pright);
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLDITALIC));
            plC = new PdfPCell(new Phrase("(For " + addr.branchName + ")", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            for (int i = 1; i <= 6; i++)
            {
                plC = new PdfPCell(new Phrase(" ", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);
            }
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.NORMAL));
            plC = new PdfPCell(new Phrase("Authorised Signature", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            return ptot;

        }
        private PdfPTable makeOrderHeader(PurPurchaseOrderUni header, int pageno, double pages,UserAddress addr)
        {
            PdfPTable ptot = new PdfPTable(1);
            float[] widths = new float[] { 550f };
            ptot.SetWidths(widths);
            ptot.TotalWidth = 550f;
            ptot.LockedWidth = true;

            iTextSharp.text.Font fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));

            PdfPCell plC = new PdfPCell(new Phrase("PURCHASE ORDER", fn));
            plC.BorderWidth = 0f;
            plC.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 16, Font.BOLD));
            plC = new PdfPCell(new Phrase(addr.branchName, fn));
            plC.BorderWidth = 1f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            PdfPTable pleft = new PdfPTable(1);
            pleft.SetWidths(new float[] { 300f });
            pleft.TotalWidth = 300f;
            pleft.LockedWidth = true;
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));


            plC = new PdfPCell(new Phrase( addr.branchName.Length > 100? addr.branchName.Substring(1,100):addr.branchName, fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pleft.AddCell(plC);
            plC = new PdfPCell(new Phrase(addr.branchName.Length > 100?addr.branchName.Substring(101,100):"", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pleft.AddCell(plC);
            plC = new PdfPCell(new Phrase( addr.city + ", " + addr.zip + ", " + addr.stat + ", " + addr.country, fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pleft.AddCell(plC);
            plC = new PdfPCell(new Phrase("Ph : " + addr.tel, fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pleft.AddCell(plC);


            PdfPTable pright = new PdfPTable(2);
            pright.SetWidths(new float[] { 100f, 150f });
            pright.TotalWidth = 250f;
            pright.LockedWidth = true;
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));

            plC = new PdfPCell(new Phrase("PO No", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(header.Seq, fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));

            plC = new PdfPCell(new Phrase("Date", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            plC = new PdfPCell(new Phrase(ac.strDate(header.Dat.Value), fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            plC = new PdfPCell(new Phrase("GST", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            plC = new PdfPCell(new Phrase(addr.fax, fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            plC = new PdfPCell(new Phrase("Page", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);
            plC = new PdfPCell(new Phrase(pageno.ToString() + " of " + pages.ToString(), fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            PdfPTable pcombo = new PdfPTable(2);
            pcombo.SetWidths(new float[] { 300f, 250f });
            pcombo.TotalWidth = 550f;
            pcombo.LockedWidth = true;
            plC = new PdfPCell(pleft);
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(pright);
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            ptot.AddCell(pcombo);


            plC = new PdfPCell(new Phrase("Supplier Details", fn));
            plC.BorderWidth = 1f;
            plC.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(header.Vendorname, fn));
            plC.BorderWidth = 0f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase(header.Addr, fn));
            plC.BorderWidth = 0f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            plC = new PdfPCell(new Phrase(header.City + ", " + header.Stat + (header.Fax == null || header.Fax.Trim() == "" ? "" : ", GST" + header.Fax), fn));
            plC.BorderWidth = 0f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);


            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 9, Font.BOLD));
           

             pcombo = new PdfPTable(6);
            pcombo.SetWidths(new float[] { 40f, 260f, 50f, 50f, 70f, 80f });
            pcombo.TotalWidth = 550f;
            pcombo.LockedWidth = true;

            plC = new PdfPCell(new Phrase("#", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(new Phrase("Material", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(new Phrase("Qty", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(new Phrase("UOM", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(new Phrase("Rate", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            plC = new PdfPCell(new Phrase("Value", fn));
            plC.BorderWidth = 1f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pcombo.AddCell(plC);
            
            plC = new PdfPCell(pcombo);
            plC.BorderWidth = 0f;

            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            ptot.AddCell(plC);
            return ptot;

        }



    }
}
