using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Usine_Core.Controllers.Purchases
{
    public class PurPurchasesTotal
    {
        public PurPurchasesUni header { get; set; }
        public List<PurPurchasesDet> lines { get; set; }
        public List<PurPurchaseTaxes> taxes { get; set; }
        public dynamic linedetails { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }

    }
    public class PurchaseRequirements
    {
        public string seq { get; set; }
        public DateTime dat { get; set; }
        public List<InvStores> stores { get; set; }
        public List<MisCountryMaster> countries { get; set; }
        public List<Purpurchasetypes> purchasetypes { get; set; }
        public List<PartyDetails> suppliers { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public dynamic materials { get; set; }
        public dynamic units { get; set; }
        public List<PurPurchaseOrderUni> orders { get; set; }
        public List<AdmTaxes> taxes { get; set; }
        public List<PurSetup> sets { get; set; }
    }
    public class purPurchasesController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/purPurchases/GetPurchaseRequirements")]
        public PurchaseRequirements GetPurchaseRequirements([FromBody] UserInfo usr)
        {
            PurchaseRequirements tot = new PurchaseRequirements();
            
            tot.seq = findSeq(usr);
            tot.dat = ac.getPresentDateTime();
            var dat1 = ac.getFinancialStart(tot.dat, usr);
            var dat2 = dat1.AddYears(1);
            tot.stores = db.InvStores.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            tot.countries = db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Cntname).ToList();
            tot.purchasetypes = db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).ToList();
            tot.suppliers = db.PartyDetails.Where(a => a.Statu==1 && a.PartyType == "SUP" && a.CustomerCode == usr.cCode).OrderBy(b => b.PartyName).ToList();
            var list1 = (from a in db.InvMaterialCompleteDetailsView.Where(a => a.Customercode == usr.cCode)
                         join b in db.InvMaterials.Where(a => a.CustomerCode == usr.cCode) on a.RecordId equals b.RecordId
                         select new InvMaterials
                         {
                             RecordId = b.RecordId,
                             ItemName=a.Itemname,
                             BranchId=a.Grpname,
                             Itemid=a.Um,
                             CostingType=b.CostingType,
                            ShelfLifeReqd=b.ShelfLifeReqd,
                            }).ToList();
            var list2 = (from a in db.InvMaterialManagement.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).GroupBy(b => b.ItemName)
                         select new InvMaterialManagement
                         {
                             ItemName=a.Key,
                             Qtyin=a.Sum(a => a.Qtyin-a.Qtyout),
                         }).ToList();
            tot.materials = (from a in list1
                             join b in list2 on a.RecordId equals b.ItemName
                into gj
                             from subdet in gj.DefaultIfEmpty()
                             select new
                             {
                                 recordid = a.RecordId,
                                 itemname = a.ItemName,
                                 grpname = a.BranchId,
                                 um = a.Itemid,
                                 qty = subdet == null ? 0 : subdet.Qtyin,
                                 costingtype=a.CostingType,
                                shelflife=a.ShelfLifeReqd
                             }).OrderBy(a => a.itemname).ToList();
            tot.orders = db.PurPurchaseOrderUni.Where(a => a.TypeOfOrder=="PO" && a.Pos <= 3 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            tot.sets = db.PurSetup.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
            var umlist1 = (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode)
                           join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.Um equals b.RecordId
                           select new InvMaterialUnits
                           {
                               RecordId = a.RecordId,
                               Um = a.Um,
                               BranchId = b.Um,
                               ConversionFactor = a.ConversionFactor
                           }
                       ).Distinct().ToList();

            var umlist2 = (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode)
                           join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.StdUm equals b.RecordId
                           select new InvMaterialUnits
                           {
                               RecordId = a.RecordId,
                               StdUm = a.StdUm,
                               BranchId = b.Um
                               
                           }
                       ).Distinct().ToList();
            tot.units = (from a in umlist1 join b in umlist2 on a.RecordId equals b.RecordId
                         select new
                         {
                             matid = a.RecordId,
                             umid = a.Um,
                             um = a.BranchId,
                             conversion = a.ConversionFactor,
                             stdumid = b.StdUm,
                             stdum = b.BranchId
                         }).OrderBy(a => a.matid).ThenByDescending(b => b.conversion).ToList();
            tot.taxes = db.AdmTaxes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.TaxCode).ThenBy(c => c.TaxPer).ToList();
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/purPurchases/GetPurchases")]
        public List<PurPurchasesUni> GetPurchases([FromBody]GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);
            return db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/purPurchases/GetPurchase")]
        public PurPurchasesTotal GetPurchase([FromBody]GeneralInformation inf)
        {
            PurPurchasesTotal tot = new PurPurchasesTotal();
            string msg = "";
            try
            {
                UsineContext db1 = new UsineContext();
                List<string> gins= db1.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.Gin).ToList();
                var det = db1.InvMaterialManagement.Where(a => gins.Contains(a.Gin) && a.TransactionType > 100 && a.Qtyout > 0 && a.CustomerCode == inf.usr.cCode);

                if (det.Count() > 0)
                {
                    msg = "Materials of this purchase are already in use";
                }
                else
                {
                    tot.header = db.PurPurchasesUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();

                    tot.linedetails = (from a in db.PurPurchasesDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                       join
b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on a.Um equals b.RecordId
                                       select new
                                       {
                                           a.RecordId,
                                           a.Sno,
                                           a.Store,
                                           a.ItemId,
                                           a.ItemName,
                                           a.Batchno,
                                           a.Manudate,
                                           a.Expdate,
                                           a.Qty,
                                           a.Rat,
                                           Umid = a.Um,
                                           b.Um
                                       }).OrderBy(b => b.Sno).ToList();

                    tot.taxes = db.PurPurchaseTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                    msg = "OK";
                }
                tot.result = msg;
                return tot;
            }
            catch(Exception ee)
            {
                tot.result = ee.Message;
                return tot;
            }

        }




        [HttpPost]
        [Authorize]
        [Route("api/purPurchases/SetPurchase")]
        public TransactionResult SetPurchase([FromBody] PurPurchasesTotal tot)
        {
            String msg = "";
            General gg = new General();
            AdminControl ac = new AdminControl();
            try
            {

                String tramsg = ac.transactionCheck("Inventory", tot.header.Dat, tot.usr);
                if (tramsg == "OK")
                {
                    switch (tot.traCheck)
                    {
                        case 1:
                            if (ac.screenCheck(tot.usr, 2, 3, 5, 1))
                            {
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        PurPurchasesUni header = new PurPurchasesUni();
                                        header.Seq = findSeq(tot.usr);
                                        header.Dat = tot.header.Dat;
                                        header.Usr = tot.usr.uCode;
                                        header.PurchaseType = tot.header.PurchaseType;
                                        header.Invoiceno = tot.header.Invoiceno;
                                        header.Invoicedat = tot.header.Invoicedat;
                                        header.Transporter = tot.header.Transporter;
                                        header.Vendorid = tot.header.Vendorid;
                                        header.Vendorname = tot.header.Vendorname;
                                        header.Addr = tot.header.Addr;
                                        header.Country = tot.header.Country;
                                        header.Stat = tot.header.Stat;
                                        header.District = tot.header.District;
                                        header.City = tot.header.City;
                                        header.Zip = tot.header.Zip;
                                        header.Mobile = tot.header.Mobile;
                                        header.Tel = tot.header.Tel;
                                        header.Fax = tot.header.Fax;
                                        header.Email = tot.header.Email;
                                        header.Webid = tot.header.Webid;
                                        header.Baseamt = tot.header.Baseamt;
                                        header.Discount = tot.header.Discount;
                                        header.Taxes = tot.header.Taxes;
                                        header.Others = tot.header.Others;
                                        header.TotalAmt = tot.header.TotalAmt;
                                        header.Pos = 1;
                                        header.BranchId = tot.usr.bCode;
                                        header.RefPoid = tot.header.RefPoid;
                                        header.CustomerCode = tot.usr.cCode;
                                        header.QcCheck = tot.header.QcCheck;
                                        header.CurrencySymbol = tot.header.CurrencySymbol;
                                        header.CurrencyConversion = tot.header.CurrencyConversion;
                                        db.PurPurchasesUni.Add(header);
                                        db.SaveChanges();
                                        
                                        int sno = 1;
                                        List<InvMaterialManagement> mgts = new List<InvMaterialManagement>();
                                        string gin = tot.header.Dat.Value.Year.ToString().Substring(2, 2) + gg.zeroMake(tot.header.Dat.Value.Month, 2) + gg.zeroMake(tot.header.Dat.Value.Day, 2);
                                        int? x = findGin(tot.usr,gin);
                                        foreach (PurPurchasesDet line in tot.lines)
                                        {

                                            if (tot.header.QcCheck != 1)
                                            {

                                                mgts.Add(new InvMaterialManagement
                                                {
                                                    TransactionId = header.RecordId,
                                                    Sno = sno,
                                                    Gin = gin + gg.zeroMake((int)x, 5),
                                                    ItemName = line.ItemId,
                                                    Dat = tot.header.Dat,
                                                    BatchNo = line.Batchno,
                                                    Manudate = line.Manudate,
                                                    Expdate = line.Expdate,
                                                    Store = line.Store,
                                                    Qtyin = line.Qty * line.Mrp,
                                                    Qtyout = 0,
                                                    Rat = line.Rat / line.Mrp,
                                                    Descr = line.BranchId,
                                                    TransactionType = 1,
                                                    BranchId = tot.usr.bCode,
                                                    CustomerCode = tot.usr.cCode

                                                });
                                            }
                                            line.RecordId = header.RecordId;
                                            line.Sno = sno;
                                            line.Gin = gin + gg.zeroMake((int)x, 5);
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            x++;
                                            sno++;
                                        }
                                        if (tot.lines.Count() > 0)
                                        {
                                            db.PurPurchasesDet.AddRange(tot.lines);
                                            db.InvMaterialManagement.AddRange(mgts);
                                        }
                                         
                                        sno = 1;
                                        foreach (PurPurchaseTaxes tax in tot.taxes)
                                        {
                                            tax.RecordId = header.RecordId;
                                            tax.Sno = sno;
                                            tax.BranchId = tot.usr.bCode;
                                            tax.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.PurPurchaseTaxes.AddRange(tot.taxes);
                                     

                                        double? advance = 0;
                                        if(header.RefPoid != null)
                                        {
                                            var po = db.PurPurchaseOrderUni.Where(a => a.RecordId == header.RefPoid && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                            if(po != null)
                                            {
                                                po.Pos = 5;
                                            }
                                            var advances = db.TotAdvanceDetails.Where(a => a.TransactionId == header.RefPoid && a.Tratype == "PUR_VOU" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                            if(advances.Count > 0)
                                            {
                                                advance = advances.Sum(b => b.Amt);
                                            }

                                        }
                                        var totalamt = header.TotalAmt - advance;
                                        if(totalamt > 0)
                                        {
                                            TotSalesSupportings support = new TotSalesSupportings();
                                            support.BillNo = header.RecordId;
                                            support.Amt = totalamt;
                                            support.BillType = "PUR";
                                            support.AccName = header.Vendorid;
                                            support.SettleMode = "CREDIT";
                                            support.Usrname = tot.usr.uCode;
                                            support.SettleDate = header.Dat.Value;
                                            support.Branchid = tot.usr.bCode;
                                            support.CustomerCode = tot.usr.cCode;
                                            db.TotSalesSupportings.Add(support);

                                            PartyTransactions trans = new PartyTransactions();
                                            trans.TransactionId = header.RecordId;
                                            trans.TransactionType = "PUR";
                                            trans.Dat = header.Dat;
                                            trans.PartyId = header.Vendorid;
                                            trans.PartyName = header.Vendorname;
                                            trans.TransactionAmt = header.TotalAmt;
                                            trans.PendingAmount = totalamt;
                                            trans.ReturnAmount = 0;
                                            trans.CreditNote = 0;
                                            trans.PaymentAmount = 0;
                                            trans.Descriptio = "Purchase of MIR " + header.Seq;
                                            trans.Username = tot.usr.uCode;
                                            trans.BranchId = tot.usr.bCode;
                                            trans.CustomerCode = tot.usr.cCode;
                                            db.PartyTransactions.Add(trans);
                                                
                                        }
                                        db.SaveChanges();
                                        if (ac.transactionCheck("Accounts",tot.header.Dat.Value,tot.usr)=="OK")
                                        {
                                            makeAccounts(header,tot.taxes,advance,totalamt, tot.usr);
                                        }

                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch (Exception ee)
                                    {
                                        txn.Rollback();
                                        msg = ee.Message;
                                    }
                                }

                            }
                            else
                            {
                                msg = "You are not authorised to create Purchase";
                            }
                            break;
                        case 2:
                            if (ac.screenCheck(tot.usr, 2, 3, 5, 2))
                            {
                                PurPurchasesUni header = db.PurPurchasesUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (header != null)
                                {
                                    header.Dat = tot.header.Dat;
                                    header.Usr = tot.usr.uCode;
                                    header.PurchaseType = tot.header.PurchaseType;
                                    header.Invoiceno = tot.header.Invoiceno;
                                    header.Invoicedat = tot.header.Invoicedat;
                                    header.Transporter = tot.header.Transporter;
                                    header.Vendorid = tot.header.Vendorid;
                                    header.Vendorname = tot.header.Vendorname;
                                    header.Addr = tot.header.Addr;
                                    header.Country = tot.header.Country;
                                    header.Stat = tot.header.Stat;
                                    header.District = tot.header.District;
                                    header.City = tot.header.City;
                                    header.Zip = tot.header.Zip;
                                    header.Mobile = tot.header.Mobile;
                                    header.Tel = tot.header.Tel;
                                    header.Fax = tot.header.Fax;
                                    header.Email = tot.header.Email;
                                    header.Webid = tot.header.Webid;
                                    header.Baseamt = tot.header.Baseamt;
                                    header.Discount = tot.header.Discount;
                                    header.Taxes = tot.header.Taxes;
                                    header.Others = tot.header.Others;
                                    header.TotalAmt = tot.header.TotalAmt;
                                    header.Pos = 1;
                                    header.BranchId = tot.usr.bCode;
                                    header.RefPoid = tot.header.RefPoid;
                                    header.CustomerCode = tot.usr.cCode;
                                    header.QcCheck = tot.header.QcCheck;
                                }
                                int? sno = db.PurPurchasesDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno); ;
                               if(sno==null)
                                {
                                    sno = 0;
                                }
                                sno++;
                                var linesupd = db.PurPurchasesDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if(linesupd.Count > 0)
                                {
                                    db.PurPurchasesDet.RemoveRange(linesupd);
                                }
                                var mgtupd = db.InvMaterialManagement.Where(a => a.TransactionId == tot.header.RecordId && a.TransactionType==1 && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if(mgtupd.Count > 0)
                                {
                                    db.InvMaterialManagement.RemoveRange(mgtupd);
                                }
                                List<InvMaterialManagement> mgts = new List<InvMaterialManagement>();
                                string gin = tot.header.Dat.Value.Year.ToString().Substring(2, 2) + gg.zeroMake(tot.header.Dat.Value.Month, 2) + gg.zeroMake(tot.header.Dat.Value.Day, 2);
                                int? x = findGin(tot.usr, gin);
                                foreach (PurPurchasesDet line in tot.lines)
                                {

                                    if (tot.header.QcCheck != 1)
                                    {

                                        mgts.Add(new InvMaterialManagement
                                        {
                                            TransactionId = header.RecordId,
                                            Sno = sno,
                                            Gin = gin + gg.zeroMake((int)x, 5),
                                            ItemName = line.ItemId,
                                            Dat = tot.header.Dat,
                                            BatchNo = line.Batchno,
                                            Manudate = line.Manudate,
                                            Expdate = line.Expdate,
                                            Store = line.Store,
                                            Qtyin = line.Qty * line.Mrp,
                                            Qtyout = 0,
                                            Rat = line.Rat / line.Mrp,
                                            Descr = line.BranchId,
                                            TransactionType = 1,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode

                                        });
                                    }
                                    line.RecordId = header.RecordId;
                                    line.Gin = gin + gg.zeroMake((int)x, 5);
                                    line.Sno = sno;
                                    line.BranchId = tot.usr.bCode;
                                    line.CustomerCode = tot.usr.cCode;
                                    x++;
                                    sno++;
                                }
                                if (tot.lines.Count() > 0)
                                {
                                    db.PurPurchasesDet.AddRange(tot.lines);
                                    db.InvMaterialManagement.AddRange(mgts);
                                }

                                sno = db.PurPurchaseTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Max(b => b.Sno); ;
                                if (sno == null)
                                {
                                    sno = 0;
                                }
                                sno++;
                                var taxesupd = db.PurPurchaseTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                if(taxesupd.Count > 0)
                                {
                                    db.PurPurchaseTaxes.RemoveRange(taxesupd);
                                }
                                foreach (PurPurchaseTaxes tax in tot.taxes)
                                {
                                    tax.RecordId = header.RecordId;
                                    tax.Sno = sno;
                                    tax.BranchId = tot.usr.bCode;
                                    tax.CustomerCode = tot.usr.cCode;
                                    sno++;
                                }
                                db.PurPurchaseTaxes.AddRange(tot.taxes);

                                double? advance = 0;
                                if (header.RefPoid != null)
                                {
                                    var po = db.PurPurchaseOrderUni.Where(a => a.RecordId == header.RefPoid && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                    if (po != null)
                                    {
                                        po.Pos = 0;
                                    }
                                    var advances = db.TotAdvanceDetails.Where(a => a.TransactionId == header.RefPoid && a.Tratype == "PUR_VOU" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (advances.Count > 0)
                                    {
                                        advance = advances.Sum(b => b.Amt);
                                    }

                                }
                                var totalamt = header.TotalAmt - advance;
                                var settle1 = db.TotSalesSupportings.Where(a => a.BillNo == header.RecordId && a.BillType =="PUR" && a.Branchid == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                var settle2 = db.PartyTransactions.Where(a => a.TransactionId == header.RecordId && a.TransactionType == "PUR" && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();

                                if(settle1 != null)
                                {
                                    db.TotSalesSupportings.Remove(settle1);
                                }
                                if(settle2 != null)
                                {
                                    db.PartyTransactions.Remove(settle2);
                                }
                                if (totalamt > 0)
                                {
                                    TotSalesSupportings support = new TotSalesSupportings();
                                    support.BillNo = header.RecordId;
                                    support.Amt = totalamt;
                                    support.BillType = "PUR";
                                    support.AccName = header.Vendorid;
                                    support.SettleMode = "CREDIT";
                                    support.Usrname = tot.usr.uCode;
                                    support.SettleDate = header.Dat.Value;
                                    support.Branchid = tot.usr.bCode;
                                    support.CustomerCode = tot.usr.cCode;
                                    db.TotSalesSupportings.Add(support);


                                    PartyTransactions trans = new PartyTransactions();
                                    trans.TransactionId = header.RecordId;
                                    trans.TransactionType = "PUR";
                                    trans.Dat = header.Dat;
                                    trans.PartyId = header.Vendorid;
                                    trans.PartyName = header.Vendorname;
                                    trans.TransactionAmt = header.TotalAmt;
                                    trans.PendingAmount = totalamt;
                                    trans.ReturnAmount = 0;
                                    trans.CreditNote = 0;
                                    trans.PaymentAmount = 0;
                                    trans.Descriptio = "Purchase of MIR " + header.Seq;
                                    trans.Username = tot.usr.uCode;
                                    trans.BranchId = tot.usr.bCode;
                                    trans.CustomerCode = tot.usr.cCode;
                                    db.PartyTransactions.Add(trans);
                                }
                                db.SaveChanges();
                                if (ac.transactionCheck("Accounts", tot.header.Dat.Value, tot.usr) == "OK")
                                {
                                    makeAccounts(tot.header, tot.taxes, advance, totalamt, tot.usr);
                                }
                                msg = "OK";

                            }
                            else
                            {
                                msg = "You are not authorised to modify Purchase";
                            }
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


            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }
        private void makeAccounts(PurPurchasesUni header, List<PurPurchaseTaxes> taxes,double? advance,double? totalamt, UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            UsineContext db2 = new UsineContext();
            UsineContext db3 = new UsineContext();
            int chk = 0;
            try
            {
                var headerupd = db2.FinexecUni.Where(a => a.Traref == header.RecordId.ToString() && a.Tratype == "PUR_PUR" && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
                if(headerupd != null)
                {
                    var linesupd = db3.FinexecDet.Where(a => a.RecordId == headerupd.RecordId && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    if(linesupd.Count > 0)
                    {
                        db3.FinexecDet.RemoveRange(linesupd);
                        db3.SaveChanges();
                    }
                    db2.FinexecUni.Remove(headerupd);
                    db2.SaveChanges();
                }

                var accounts = db1.AccountsAssign.Where(a => a.Module == "PUR" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                List<FinexecDet> lines = new List<FinexecDet>();
                var account = accounts.Where(a => a.Transcode == "PUR_PUR").FirstOrDefault();
                
                if(account != null)
                {
                    lines.Add(new FinexecDet
                    {
                        Sno = 1,
                        Accname = account.Account,
                        Cre = 0,
                        Deb = header.Baseamt *(header.CurrencyConversion==null?1:header.CurrencyConversion),
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = header.Dat
                    });
                }    
                else
                {
                    chk = 1;
                }

                account = accounts.Where(a => a.Transcode == "PUR_DIS").FirstOrDefault();

                if (account != null)
                {
                    if (header.Discount > 0)
                    {
                        lines.Add(new FinexecDet
                        {
                            Sno = 2,
                            Accname = account.Account,
                            Cre = header.Discount * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                            Deb = 0,
                            Branchid = usr.bCode,
                            CustomerCode = usr.cCode,
                            Dat = header.Dat
                        });
                    }
                }
                else
                {
                    if(header.Discount > 0)
                    chk = 1;
                }

                account = accounts.Where(a => a.Transcode == "PUR_OTH").FirstOrDefault();
                if (account != null)
                {
                    if (header.Others > 0)
                    {
                        lines.Add(new FinexecDet
                        {
                            Sno = 3,
                            Accname = account.Account,
                            Cre = 0,
                            Deb = header.Others * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                            Branchid = usr.bCode,
                            CustomerCode = usr.cCode,
                            Dat = header.Dat
                        });
                    }
                }
                else
                {
                    if (header.Others > 0)
                        chk = 1;
                }

                int sno = 6;
                foreach(var tax in taxes)
                {
                    account = accounts.Where(a => a.Transcode == "PUR_" + tax.Taxcode + "@" + tax.Taxper.ToString()).FirstOrDefault();
                    if (account != null)
                    {
                        if (tax.Taxvalue > 0)
                        {
                            lines.Add(new FinexecDet
                            {
                                Sno = sno,
                                Accname = account.Account,
                                Cre = 0,
                                Deb = tax.Taxvalue * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                                Branchid = usr.bCode,
                                CustomerCode = usr.cCode,
                                Dat = header.Dat
                            });
                        }
                    }
                    else
                    {
                        if (tax.Taxvalue > 0)
                            chk = 1;
                    }
                    sno++;
                }

                if(advance > 0)
                {
                    account = accounts.Where(a => a.Transcode == "PUR_VOU").FirstOrDefault();
                    if (account != null)
                    {
                         
                            lines.Add(new FinexecDet
                            {
                                Sno = 21,
                                Accname = account.Account,
                                Cre = advance * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                                Deb = 0,
                                Branchid = usr.bCode,
                                CustomerCode = usr.cCode,
                                Dat = header.Dat
                            });
                         
                    }
                    else
                    {
                         
                            chk = 1;
                    }
                }
                if(totalamt != 0)
                {
                    account = accounts.Where(a => a.Transcode == "PUR_CRP").FirstOrDefault();
                    if (account != null)
                    {

                        lines.Add(new FinexecDet
                        {
                            Sno = 22,
                            Accname = account.Account,
                            Cre = (totalamt >0?totalamt:0) * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                            Deb = (totalamt < 0? (double)(Math.Abs((decimal)totalamt)):0) * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                            Branchid = usr.bCode,
                            CustomerCode = usr.cCode,
                            Dat = header.Dat
                        });

                    }
                    else
                    {

                        chk = 1;
                    }
                }

                FinexecUni acheader = new FinexecUni();
                if(chk==0)
                {
                    acheader.Seq = finAccSeq(usr, header.Dat.Value);
                    acheader.Dat = header.Dat;
                    acheader.Narr = "Purchase Detail of MIR " + header.Seq;
                    acheader.Tratype = "PUR_PUR";
                    acheader.Traref = header.RecordId.ToString();
                    acheader.Vouchertype = "PURCHASE";
                    acheader.Branchid=usr.bCode;
                    acheader.CustomerCode = usr.cCode;
                    acheader.Usr = usr.uCode;
                    db1.FinexecUni.Add(acheader);
                    db1.SaveChanges();

                    foreach(var lin in lines)
                    {
                        lin.RecordId = acheader.RecordId;
                    }
                    db1.FinexecDet.AddRange(lines);
                    db1.SaveChanges();

                }
            }
            catch(Exception ee)
            {

            }
        }
        private int? finAccSeq(UserInfo usr,DateTime dat)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = dat;
            DateTime dat2 = dat1.AddDays(1);
            int? x = 0;
            var xx = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode ==  usr.cCode && a.Dat >= dat1 && a.Dat < dat2).FirstOrDefault();
            if (xx != null)
            {
                x = db.FinexecUni.Where(a => a.Branchid == usr.bCode && a.CustomerCode == usr.cCode && a.Dat >= dat1 && a.Dat < dat2).Max(b => b.Seq);
            }
            x++;
            return x;
        }
        public int? findGin(UserInfo usr,string gin)
        {
            UsineContext db1 = new UsineContext();
            General g = new General();
            var det = db1.InvMaterialManagement.Where(a => a.Gin.Contains(gin) && a.CustomerCode == usr.cCode).Max(b => b.Gin);
            int x = 0;
            if(det != null)
            {
                x = g.valInt(g.right(det, 5));
            }
            x++;
            return x;
        }
        public String findSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            General g = new General();
            int x = 0;
            AdminControl ac = new AdminControl();
            DateTime dat = ac.getPresentDateTime();
            var xx = db.PurPurchasesUni.Where(a => a.Dat.Value.Month == dat.Month && a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
            if (xx != null)
            {
                var y = db.PurPurchasesUni.Where(a => a.Dat.Value.Month == dat.Month && a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
                x = int.Parse(g.right(y,4));

            }
            x++;
            return "MIR" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/" + g.zeroMake(x, 4);
        }



    }
}
