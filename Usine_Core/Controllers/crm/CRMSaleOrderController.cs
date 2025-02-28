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
using Usine_Core.Controllers.Purchases;

namespace Usine_Core.Controllers.crm
{
    public class CompleteSaleOrderRequirements
    {
        public String pono { get; set; }
        public DateTime dat { get; set; }
        public List<PartyDetails> customers { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<InvMaterialDetails> materials { get; set; }
        public List<AdmTaxes> taxes { get; set; }
        public List<InvMaterialsUnitsDetails> units { get; set; }
        public List<PurTerms> terms { get; set; }
        public List<Purpurchasetypes> purtypes { get; set; }
        public List<CrmPriceListUni> pricesuni { get; set; }
        public List<CrmPriceListDet> pricesdet { get; set; }
        public List<CrmDiscountListUni> discsuni { get; set; }
        public List<CrmDiscountListDet> discsdet { get; set; }
        public List<CrmTaxAssigningUni> taxesuni { get; set; }
        public List<CrmTaxAssigningDet> taxesdet { get; set; }

        public List<MisCountryMaster> countries { get; set; }
        public List<MisStateMaster> states { get; set; }
        public List<HrdEmployees> employees { get; set; }
    }
    public class CRMSaleOrderTotal
    {
        public CrmSaleOrderUni header { get; set; }
        public List<CrmSaleOrderDet> lines { get; set; }
        public List<CrmSaleOrderTerms> terms { get; set; }
        public List<CrmSaleOrderTaxes> taxes { get; set; }
        public List<TotAdvanceDetails> advances { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }

    public class CRMSaleOrderController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public CRMSaleOrderController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleOrder/GetSaleOrderRequirements")]
        public CompleteSaleOrderRequirements GetSaleOrderRequirements([FromBody] UserInfo usr)
        {
            CompleteSaleOrderRequirements tot = new CompleteSaleOrderRequirements();
            tot.pono = findSeq(usr);

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
            tot.customers = db.PartyDetails.Where(a => a.PartyType == "CUS" && a.CustomerCode == usr.cCode).OrderBy(b => b.PartyName).ToList();

            InvMaterialsController iv = new InvMaterialsController();
             tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();

            tot.materials = iv.GetInvMaterials(usr).Where(a => a.statu == "Active").ToList();
              tot.units = iv.GetInvMaterialUnits(usr);
               tot.terms = db.PurTerms.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
            tot.purtypes = db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
              tot.countries = (from a in db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode)
                             select new MisCountryMaster
                             {
                                 RecordId = a.RecordId,
                                 Cntname = a.Cntname,
                                 ConversionFactor = a.ConversionFactor,
                                 Curr = a.Curr,
                                 CurrSymbol = a.CurrSymbol,
                             }).OrderBy(b => b.RecordId).ToList();
            tot.states = (from a in db.MisStateMaster.Where(a => a.CustomerCode == usr.cCode)
                          select new MisStateMaster
                          {
                              RecordId = a.RecordId,
                              Cntname = a.Cntname,
                              StateName = a.StateName

                          }).OrderBy(b => b.StateName).ToList();
            tot.pricesuni = db.CrmPriceListUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.pricesdet = (from a in db.CrmPriceListDet.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                             select new CrmPriceListDet
                             {
                                 RecordId = a.RecordId,
                                 Sno = a.Sno,
                                 ProductId = a.ProductId,
                                 Price = a.Price,
                                 TaxId=a.TaxId

                             }).ToList();
            tot.discsuni = db.CrmDiscountListUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.discsdet = (from a in db.CrmDiscountListDet.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                            select new CrmDiscountListDet
                            {
                                RecordId = a.RecordId,
                                Sno = a.Sno,
                                ProductId = a.ProductId,
                                Discount = a.Discount
                            }).ToList();
            tot.taxesuni = db.CrmTaxAssigningUni.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.taxesdet = (from a in db.CrmTaxAssigningDet.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                            select new CrmTaxAssigningDet
                            {
                                RecordId = a.RecordId,
                                Sno = a.Sno,
                                TaxCode = a.TaxCode,
                                Taxper = a.Taxper
                            }).ToList();
            tot.employees = (from a in db.HrdEmployees.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode)
                             join
b in db.HrdDesignations.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode) on a.Designation equals b.RecordId
                             select new HrdEmployees
                             {
                                 RecordId = a.RecordId,
                                 Empname = a.Empname,
                                 Mobile = a.Mobile,
                                 Tel = b.Designation
                             }).OrderBy(c => c.Empname).ToList();
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleOrder/GetSaleOrders")]
        public List<CrmSaleOrderUni> GetSaleOrders([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            try
            {
                return db.CrmSaleOrderUni.Where(a =>   a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.RecordId).ToList();
            }
            catch
            {
                return null;
            }

        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleOrder/GetSaleOrdersForApprovals")]
        public List<CrmSaleOrderUni> GetSaleOrdersForApprovals([FromBody] UserInfo usr)
        {
            return db.CrmSaleOrderUni.Where(a => a.Pos == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();

        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleOrder/SetSaleOrderForApprovals")]
        public TransactionResult SetSaleOrderForApprovals([FromBody] GeneralInformation inf)
        {
            string msg = "";
            var order = db.CrmSaleOrderUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if (order != null)
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
        [Route("api/CRMSaleOrder/GetSaleOrder")]
        public CRMSaleOrderTotal GetSaleOrder([FromBody] GeneralInformation inf)
        {
            var det = db.PurPurchasesUni.Where(a => a.RefPoid == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
             CRMSaleOrderTotal tot = new CRMSaleOrderTotal();
            if (det == null)
            {
                try
                {
                    tot.header = db.CrmSaleOrderUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    tot.lines = (from a in db.CrmSaleOrderDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                 join b in db.InvUm.Where(a => a.CustomerCode ==inf.usr.cCode) on a.Um equals b.RecordId
                                 select new CrmSaleOrderDet
                                            {
                                             RecordId=   a.RecordId,
                                                Sno=a.Sno,
                                              ItemId=  a.ItemId,
                                              ItemName=  a.ItemName,
                                                ItemDescription = a.ItemDescription,
                                             Qty=   a.Qty,
                                                 Um = a.Um,
                                                 BranchId=b.Um,
                                               Rat= a.Rat,
                                                ReqdBy = a.ReqdBy,
                                                DiscPer=a.DiscPer
                                            }).OrderBy(b => b.Sno).ToList();
                                
                    tot.terms = (from a in db.CrmSaleOrderTerms.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                 select new CrmSaleOrderTerms
                                 {
                                     RecordId =a.RecordId,
                                     Sno=a.Sno,
                                     Term=a.Term
                                 }).OrderBy(b => b.Sno).ToList();
                    tot.taxes = (from a in db.CrmSaleOrderTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                 select new CrmSaleOrderTaxes
                                 {
                                     RecordId=a.RecordId,
                                     Sno=a.Sno,
                                     LineId=a.LineId,
                                     Taxcode=a.Taxcode,
                                     Taxper=a.Taxper,
                                     Taxvalue=a.Taxvalue
                                 }).OrderBy(b => b.Sno).ToList();
                    tot.advances = db.TotAdvanceDetails.Where(a => a.TransactionId == inf.recordId && a.Tratype == "SAL_REC" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();

                    tot.result = "OK";

                }
                catch (Exception ee)
                {
                    tot.result = ee.Message;
                }
            }
            else
            {
                tot.result = "Sale against this order is already over";
            }
            return tot;

        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleOrder/SetSaleOrder")]
        public TransactionResult SetSaleOrder([FromBody] CRMSaleOrderTotal tot)
        {

            String msg = "";
            AdminControl ac = new AdminControl();

            if (ac.screenCheck(tot.usr, 7, 2, 4, (int)tot.traCheck))
            {
                int? traCheck = tot.traCheck;
                General gg = new General();
                var statuscheck = db.PurSetup.Where(a => a.SetupCode == "crm_ord" && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                int? statu = 2;
                if (statuscheck != null)
                {
                    statu = gg.valInt(statuscheck.SetupValue) == 1 ? 2 : 1;
                }

                try
                {

                    String tramsg = ac.transactionCheck("CRM", tot.header.Dat, tot.usr);
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
                                        tot.header.Seq = findSeq(tot.usr);
                                        tot.header.BranchId = tot.usr.bCode;
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        tot.header.Pos = statu;
                                        tot.header.PrintCount = 0;
                                        db.CrmSaleOrderUni.Add(tot.header);
                                        db.SaveChanges();
                                        int sno = 1;
                                        foreach (CrmSaleOrderDet line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        if (tot.lines.Count() > 0)
                                        {
                                            db.CrmSaleOrderDet.AddRange(tot.lines);
                                        }
                                        sno = 1;
                                        if (tot.terms != null)
                                        {
                                            if (tot.terms.Count() > 0)
                                            {
                                                foreach (CrmSaleOrderTerms term in tot.terms)
                                                {
                                                    term.RecordId = tot.header.RecordId;
                                                    term.Sno = sno;
                                                    term.BranchId = tot.usr.bCode;
                                                    term.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.CrmSaleOrderTerms.AddRange(tot.terms);

                                            }
                                        }

                                        sno = 1;
                                        if (tot.taxes != null)
                                        {
                                            if (tot.taxes.Count > 0)
                                            {

                                                foreach (CrmSaleOrderTaxes tax in tot.taxes)
                                                {
                                                    tax.RecordId = tot.header.RecordId;
                                                    tax.Sno = sno;
                                                    tax.BranchId = tot.usr.bCode;
                                                    tax.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.CrmSaleOrderTaxes.AddRange(tot.taxes);
                                            }
                                        }



                                        TransactionsAudit aud = new TransactionsAudit();
                                        aud.TraId = tot.header.RecordId;
                                        var amt1 = tot.header.TotalAmt;
                                        aud.Descr = "A Sale order of " + amt1.ToString() + " has been created";
                                        aud.Usr = tot.usr.uCode;
                                        aud.Tratype = 1;
                                        aud.Transact = "CRM_ORD";
                                        aud.TraModule = "CRM";
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
                                var headerupd = db.CrmSaleOrderUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                headerupd.Usr = tot.usr.uCode;
                                headerupd.SaleType = tot.header.SaleType;
                                  headerupd.Validity = ac.DateAdjustFromFrontEnd((DateTime)tot.header.Validity);
                                headerupd.Reference = tot.header.Reference;
                                headerupd.PartyId = tot.header.PartyId;
                                headerupd.PartyName = tot.header.PartyName;
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


                                var linesupd = db.CrmSaleOrderDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                int? slno = linesupd.Count() > 0 ? linesupd.Max(b => b.Sno) : 0;
                                slno++;
                                db.CrmSaleOrderDet.RemoveRange(linesupd);
                                foreach (CrmSaleOrderDet line in tot.lines)
                                {
                                    line.RecordId = tot.header.RecordId;
                                    line.Sno = slno;
                                    line.BranchId = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    slno++;
                                }
                                if (tot.lines.Count() > 0)
                                {
                                    db.CrmSaleOrderDet.AddRange(tot.lines);
                                }

                                var termsupd = db.CrmSaleOrderTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                slno = termsupd.Count() > 0 ? termsupd.Max(b => b.Sno) : 0;
                                slno++;
                                if (termsupd.Count() > 0)
                                {
                                    db.CrmSaleOrderTerms.RemoveRange(termsupd);
                                }
                                if (tot.terms != null)
                                {
                                    if (tot.terms.Count() > 0)
                                    {
                                        foreach (CrmSaleOrderTerms term in tot.terms)
                                        {
                                            term.RecordId = tot.header.RecordId;
                                            term.Sno = slno;
                                            term.BranchId = tot.usr.bCode;
                                            term.CustomerCode = tot.usr.cCode;
                                            slno++;
                                        }
                                        db.CrmSaleOrderTerms.AddRange(tot.terms);

                                    }
                                }

                                var taxesupd = db.CrmSaleOrderTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                slno = taxesupd.Count() > 0 ? taxesupd.Max(b => b.Sno) : 0;
                                slno++;
                                if (taxesupd.Count() > 0)
                                {
                                    db.CrmSaleOrderTaxes.RemoveRange(taxesupd);
                                }
                                if (tot.taxes != null)
                                {
                                    if (tot.taxes.Count > 0)
                                    {

                                        foreach (CrmSaleOrderTaxes tax in tot.taxes)
                                        {
                                            tax.RecordId = tot.header.RecordId;
                                            tax.Sno = slno;
                                            tax.BranchId = tot.usr.bCode;
                                            tax.CustomerCode = tot.usr.cCode;
                                            slno++;
                                        }
                                        db.CrmSaleOrderTaxes.AddRange(tot.taxes);
                                    }
                                }


                                TransactionsAudit aud1 = new TransactionsAudit();
                                aud1.TraId = tot.header.RecordId;
                                var amt = tot.header.TotalAmt;
                                aud1.Descr = "A Sale order of seq " + tot.header.Seq + amt.ToString() + " has been modified";
                                aud1.Usr = tot.usr.uCode;
                                aud1.Tratype = 2;
                                aud1.Transact = "CRM_ORD";
                                aud1.TraModule = "CRM";
                                aud1.Syscode = " ";
                                aud1.BranchId = tot.usr.bCode;
                                aud1.CustomerCode = tot.usr.cCode;
                                aud1.Dat = ac.getPresentDateTime();
                                db.TransactionsAudit.Add(aud1);

                            
                                db.SaveChanges();
                                msg = "OK";

                                break;
                            case 3:
                                var headerdel = db.CrmSaleOrderUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var linesdel = db.CrmSaleOrderDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var termsdel = db.CrmSaleOrderTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                var taxesdel = db.CrmSaleOrderTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if (taxesdel.Count() > 0)
                                {
                                    db.CrmSaleOrderTaxes.RemoveRange(taxesdel);
                                }
                                if (termsdel.Count() > 0)
                                {
                                    db.CrmSaleOrderTerms.RemoveRange(termsdel);
                                }
                                if (linesdel.Count() > 0)
                                {
                                    db.CrmSaleOrderDet.RemoveRange(linesdel);
                                }
                                if (headerdel != null)
                                {
                                    db.CrmSaleOrderUni.Remove(headerdel);
                                }

                                TransactionsAudit aud3 = new TransactionsAudit();
                                aud3.TraId = tot.header.RecordId;
                                var amt3 = tot.header.TotalAmt;
                                aud3.Descr = "A Sale order of " + amt3.ToString() + " has been deleted";
                                aud3.Usr = tot.usr.uCode;
                                aud3.Tratype = 3;
                                aud3.Transact = "CRM_ORD";
                                aud3.TraModule = "CRM";
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


        private String findSeq(UserInfo usr)
    {
        UsineContext db = new UsineContext();
        General g = new General();
        int x = 0;
        AdminControl ac = new AdminControl();
        DateTime dat = ac.getPresentDateTime();

        string str = "SO" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/";
        var xx = db.CrmSaleOrderUni.Where(a => a.Seq.Contains(str) && a.BranchId ==usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq) ;
        if (xx != null)
        {
            x = g.valInt(g.right(xx, 4));
        }
        x++;
        return str + g.zeroMake(x, 4);
    }
}
}
