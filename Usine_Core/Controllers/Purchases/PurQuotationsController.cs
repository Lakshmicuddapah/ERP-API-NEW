using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Accounts;
using Usine_Core.Controllers.Admin;
 
using Usine_Core.Controllers.Inventory;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Usine_Core.Controllers.Purchases
{
    public class PurQuotationTotal
    {
        public PurQuotationUni header { get; set; }
        public List<PurQuotationDet> lines { get; set; }
        public List<PurQuotationTerms> terms { get; set; }
        public List<PurQuotationTaxes> taxes { get; set; }
        public List<FileUploadAttribute> imgs { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }

    }
    
    public class CompletePurchaseQuotationRequirements
    {
        public String pqseq { get; set; }
        public string dat { get; set; }
        public List<AccountDetails> suppliers { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<InvMaterialDetails> materials { get; set; }
        public List<AdmTaxes> taxes { get; set; }
        public List<InvMaterialsUnitsDetails> units { get; set; }
        public List<PurTerms> terms { get; set; }
        public List<PurPurchaseEnquiryUni> enquiries { get; set; }
    }
   

    public class PurQuotationsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public PurQuotationsController(IHostingEnvironment hostingEnvironment)
        {

            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/GetPurchaseQuotationRequirements")]
        public CompletePurchaseQuotationRequirements GetPurchaseQuotationRequirements([FromBody] UserInfo usr)
        {
            CompletePurchaseQuotationRequirements tot = new CompletePurchaseQuotationRequirements();
            tot.pqseq = findSeq(usr);

            tot.suppliers =
                (from p in db.PartyDetails.Where(a => a.PartyType == "SUP" && a.CustomerCode == usr.cCode)
                 select new AccountDetails
                 {
                     accountId = p.RecordId,
                     accountname = p.PartyName,
                 }


                 ).OrderBy(a => a.accountname).ToList();

            InvMaterialsController iv = new InvMaterialsController();
            AdminTaxesController ax = new AdminTaxesController();
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            tot.dat = ac.strDate( ac.getPresentDateTime());
            tot.materials = iv.GetInvMaterials(usr);
            tot.taxes = ax.GetTaxes(usr);
            tot.units = iv.GetInvMaterialUnits(usr);
            tot.terms = db.PurTerms.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Slno).ToList();
            var enqdets= (from a in db.PurPurchaseEnquiryUni.Where(a => a.Pos > 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode)
                                             join b in db.PurQuotationUni.Where(a => a.BranchId ==  usr.bCode && a.CustomerCode ==  usr.cCode) on a.RecordId equals b.RefQuotationId into gj
                                            from subdet in gj.DefaultIfEmpty()
                                            select new PurPurchaseEnquiryUni
                                            {
                                                RecordId = a.RecordId,
                                                Seq = a.Seq,
                                                Reference = subdet == null ? " " : subdet.Seq,
                                                Dat = a.Dat,
                                                Usr = a.Usr,
                                                Validity = a.Validity,
                                                Supplier = a.Supplier,
                                                Mobile = a.Mobile,
                                                Pos = a.Pos,
                                                BranchId = a.Pos == 1 ? "Pending" : (subdet == null ? "No Quotation" : "Cleared")
                                            }).ToList();
            tot.enquiries = enqdets.Where(a => a.Pos > 1 && a.BranchId != "Cleared").OrderBy(a => a.Seq).ToList();
            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/GetPurQuotations")]
        public List<PurQuotationUni> GetPurQuotations([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            return db.PurQuotationUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
          }

        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/GetPurQuotationsForApprovals")]
        public List<PurQuotationUni> GetPurQuotationsForApprovals([FromBody] UserInfo usr)
        {
            return db.PurQuotationUni.Where(a => a.Pos==1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
        }
        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/GetPurQuotation")]
        public PurQuotationTotal GetPurchaseQuotation([FromBody] GeneralInformation inf)
        {
            PurQuotationTotal tot = new PurQuotationTotal();
            tot.header = db.PurQuotationUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            tot.lines = (from a in db.PurQuotationDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         join b in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode)
                         on a.Um equals b.RecordId
                         select new PurQuotationDet
                         {
                             RecordId = a.RecordId,
                             Sno = a.Sno,
                             ItemId = a.ItemId,
                             ItemName = a.ItemName,
                             ItemDescription = a.ItemDescription,
                             Qty = a.Qty,
                             LeadDays=a.LeadDays,
                             Um = a.Um,
                             Rat = a.Rat,
                             BranchId = b.Um

                         }).OrderBy(b => b.Sno).ToList();
            tot.taxes = (from a in db.PurQuotationTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         select new PurQuotationTaxes
                         {
                             RecordId=a.RecordId,
                             Sno=a.Sno,
                             TaxCode=a.TaxCode,
                             TaxPer=a.TaxPer,
                             TaxValue=a.TaxValue
                         }).OrderBy(b => b.Sno).ToList();
            tot.terms = (from a in db.PurQuotationTerms.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                         select new PurQuotationTerms
                         {
                             RecordId=a.RecordId,
                             Sno=a.Sno,
                             Term=a.Term
                         }).OrderBy(b => b.Sno).ToList();
            tot.result = "OK";
            /* string JSONString = string.Empty;
             JSONString = JsonConvert.SerializeObject(tot);
             return JSONString;*/
            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/setPurQuotation")]
        public TransactionResult setPurQuotation([FromBody] PurQuotationTotal tot)
        {
            string msg = "";
            if (ac.screenCheck(tot.usr, 2, 2, 4, (int)tot.traCheck))
            {
                DataBaseContext ggg = new DataBaseContext();
                General gg = new General();
                int? sno = 1;
                try
                {
                    var statuscheck = db.PurSetup.Where(a => a.SetupCode == "pur_quo" && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                    int? statu = 2;
                    if (statuscheck != null)
                    {
                        statu = gg.valInt(statuscheck.SetupValue) == 1 ? 2 : 1;
                    }
                    string tramsg = ac.transactionCheck("Purchases", tot.header.Dat, tot.usr);
                    if (tramsg == "OK")
                    {
                        switch (tot.traCheck)
                        {
                            case 1:
                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        tot.header.Seq = findSeq(tot.usr);
                                        tot.header.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                                        tot.header.Validity = ac.DateAdjustFromFrontEnd(tot.header.Validity.Value);
                                        tot.header.Pos = statu;
                                        tot.header.BranchId = tot.usr.bCode;
                                        tot.header.CustomerCode = tot.usr.cCode;
                                        db.PurQuotationUni.Add(tot.header);
                                        db.SaveChanges();


                                        foreach (var line in tot.lines)
                                        {
                                            line.RecordId = tot.header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.PurQuotationDet.AddRange(tot.lines);

                                        sno = 1;
                                        if (tot.taxes != null)
                                        {
                                            if (tot.taxes.Count() > 0)
                                            {
                                                foreach (var tax in tot.taxes)
                                                {
                                                    tax.RecordId = tot.header.RecordId;
                                                    tax.Sno = sno;
                                                    tax.BranchId = tot.usr.bCode;
                                                    tax.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.PurQuotationTaxes.AddRange(tot.taxes);
                                            }
                                        }
                                        sno = 1;
                                        if (tot.terms != null)
                                        {
                                            if (tot.terms.Count() > 0)
                                            {
                                                foreach (var term in tot.terms)
                                                {
                                                    term.RecordId = tot.header.RecordId;
                                                    term.Sno = sno;
                                                    term.BranchId = tot.usr.bCode;
                                                    term.CustomerCode = tot.usr.cCode;
                                                    sno++;
                                                }
                                                db.PurQuotationTerms.AddRange(tot.terms);
                                            }
                                        }
                                        if (tot.header.RefQuotationId > 0)
                                        {
                                            var enquiry = db.PurPurchaseEnquiryUni
                                                            .Where(e => e.RecordId == tot.header.RefQuotationId
                                                                     && e.BranchId == tot.usr.bCode
                                                                     && e.CustomerCode == tot.usr.cCode)
                                                            .FirstOrDefault();

                                            if (enquiry != null)
                                            {
                                                tot.header.RefQuotation = enquiry.Seq;  // Save Enquiry Sequence number
                                            }
                                        }
                                        db.SaveChanges();

                                        TransactionsAudit audit = new TransactionsAudit();
                                        audit.TraId = (int)tot.header.RecordId;
                                        audit.Descr = "A new Quotation created with Id of " + tot.header.Seq + " from the supplier " + tot.header.Vendorname;
                                        audit.Usr = tot.usr.uCode;
                                        audit.Tratype = 1;
                                        audit.Transact = "PUR_QUO";
                                        audit.TraModule = "PUR";
                                        audit.Syscode = "Purchase Quotation";
                                        audit.BranchId = tot.usr.bCode;
                                        audit.CustomerCode = tot.usr.cCode;
                                        audit.Dat = ac.getPresentDateTime();
                                        db.TransactionsAudit.Add(audit);
                                        db.SaveChanges();

                                        string fn = "PUR_QUO_" + tot.header.RecordId + "_" + tot.usr.bCode + "_" + tot.usr.cCode + ".jpg";
                                        var imgresult = ggg.imageUploadGeneric(tot.imgs, ho.WebRootPath + "\\Attachments\\" + "purchases\\" + fn);
                                        txn.Commit();
                                        msg = "OK";

                                    }
                                    catch (Exception ex)
                                    {
                                        msg = ex.Message;
                                        txn.Rollback();
                                    }
                                }
                                break;
                            case 2:
                                var headerupd = db.PurQuotationUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headerupd != null)
                                {
                                    headerupd.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                                    headerupd.Usr = tot.usr.uCode;
                                    headerupd.PurchaseType = tot.header.PurchaseType;
                                    if (tot.header.RefQuotationId > 0)
                                    {
                                        headerupd.RefQuotationId = tot.header.RefQuotationId;
                                    }
                                    headerupd.RefQuotation = tot.header.RefQuotation;
                                    headerupd.Validity = tot.header.Validity;
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

                                    var linesupd = db.PurQuotationDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    var termsupd = db.PurQuotationTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    var taxesupd = db.PurQuotationTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (linesupd.Count() > 0)
                                    {
                                        sno = linesupd.Max(a => a.Sno);
                                    }
                                    else
                                    {
                                        sno = 0;
                                    }
                                    sno++;

                                    foreach (var line in tot.lines)
                                    {
                                        line.RecordId = tot.header.RecordId;
                                        line.Sno = sno;
                                        line.BranchId = tot.usr.bCode;
                                        line.CustomerCode = tot.usr.cCode;
                                        sno++;
                                    }


                                    if (taxesupd.Count() > 0)
                                    {
                                        sno = taxesupd.Max(a => a.Sno);
                                    }
                                    else
                                    {
                                        sno = 0;
                                    }
                                    sno++;
                                    if (tot.taxes != null)
                                    {
                                        if (tot.taxes.Count() > 0)
                                        {
                                            foreach (var tax in tot.taxes)
                                            {
                                                tax.RecordId = tot.header.RecordId;
                                                tax.Sno = sno;
                                                tax.BranchId = tot.usr.bCode;
                                                tax.CustomerCode = tot.usr.cCode;
                                                sno++;
                                            }

                                        }
                                    }
                                    if (termsupd.Count() > 0)
                                    {
                                        sno = termsupd.Max(a => a.Sno);
                                    }
                                    else
                                    {
                                        sno = 0;
                                    }
                                    sno++;
                                    if (tot.terms != null)
                                    {
                                        if (tot.terms.Count() > 0)
                                        {
                                            foreach (var term in tot.terms)
                                            {
                                                term.RecordId = tot.header.RecordId;
                                                term.Sno = sno;
                                                term.BranchId = tot.usr.bCode;
                                                term.CustomerCode = tot.usr.cCode;
                                                sno++;
                                            }

                                        }
                                    }
                                    if (linesupd.Count() > 0)
                                    {
                                        db.PurQuotationDet.RemoveRange(linesupd);
                                    }
                                    if (taxesupd.Count() > 0)
                                    {
                                        db.PurQuotationTaxes.RemoveRange(taxesupd);
                                    }
                                    if (termsupd.Count() > 0)
                                    {
                                        db.PurQuotationTerms.RemoveRange(termsupd);
                                    }

                                    db.PurQuotationDet.AddRange(tot.lines);
                                    if (tot.taxes.Count() > 0)
                                    {
                                        db.PurQuotationTaxes.AddRange(tot.taxes);
                                    }
                                    if (tot.terms.Count() > 0)
                                    {
                                        db.PurQuotationTerms.AddRange(tot.terms);
                                    }
                                    if (tot.header.RefQuotationId > 0)
                                    {
                                        var enquiry = db.PurPurchaseEnquiryUni
                                                        .Where(e => e.RecordId == tot.header.RefQuotationId
                                                                 && e.BranchId == tot.usr.bCode
                                                                 && e.CustomerCode == tot.usr.cCode)
                                                        .FirstOrDefault();

                                        if (enquiry != null)
                                        {
                                            tot.header.RefQuotation = enquiry.Seq;  // Save Enquiry Sequence number
                                        }
                                    }
                                    db.SaveChanges();

                                    TransactionsAudit audit = new TransactionsAudit();
                                    audit.TraId = (int)tot.header.RecordId;
                                    audit.Descr = "A new Quotation modified with Id of " + tot.header.Seq + " from the supplier " + tot.header.Vendorname;
                                    audit.Usr = tot.usr.uCode;
                                    audit.Tratype = 2;
                                    audit.Transact = "PUR_QUO";
                                    audit.TraModule = "PUR";
                                    audit.Syscode = "Purchase Quotation";
                                    audit.BranchId = tot.usr.bCode;
                                    audit.CustomerCode = tot.usr.cCode;
                                    audit.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(audit);
                                    db.SaveChanges();
                                    msg = "OK";
                                }
                                else
                                {
                                    msg = "No record found";
                                }
                                break;
                            case 3:
                                var headerdel = db.PurQuotationUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if (headerdel != null)
                                {
                                    var linesdel = db.PurQuotationDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    var termsdel = db.PurQuotationTerms.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    var taxesdel = db.PurQuotationTaxes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (linesdel.Count() > 0)
                                    {
                                        db.PurQuotationDet.RemoveRange(linesdel);
                                    }
                                    if (termsdel.Count() > 0)
                                    {
                                        db.PurQuotationTerms.RemoveRange(termsdel);
                                    }
                                    if (taxesdel.Count() > 0)
                                    {
                                        db.PurQuotationTaxes.RemoveRange(taxesdel);
                                    }
                                    db.PurQuotationUni.Remove(headerdel);
                                    db.SaveChanges();

                                    TransactionsAudit audit = new TransactionsAudit();
                                    audit.TraId = (int)tot.header.RecordId;
                                    audit.Descr = "A new Quotation deleted with Id of " + tot.header.Seq + " from the supplier " + tot.header.Vendorname;
                                    audit.Usr = tot.usr.uCode;
                                    audit.Tratype = 3;
                                    audit.Transact = "PUR_QUO";
                                    audit.TraModule = "PUR";
                                    audit.Syscode = "Purchase Quotation";
                                    audit.BranchId = tot.usr.bCode;
                                    audit.CustomerCode = tot.usr.cCode;
                                    audit.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(audit);
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
                        msg = tramsg;
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
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
        private string findSeq(UserInfo usr)
        {
            int x = 0;
            General g = new General();
            string str = "PQ" + g.right(DateTime.Now.Year.ToString(), 2) + "-";
            var det = db.PurQuotationUni.Where(a => a.Seq.Contains(str) && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(det != null)
            {
                x = g.valInt(g.right(det, 5));
            }
            x++;

            return str + g.zeroMake(x,5);
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurQuotations/SetPurQuotationForApproval")]
        public TransactionResult SetPurQuotationForApproval([FromBody] GeneralInformation inf)
        {
            TransactionResult result = new TransactionResult();
            string msg = "";
            try
            {
                if(ac.screenCheck(inf.usr,2,2,4,98))
                {
                    var header = db.PurQuotationUni.Where(a => a.RecordId == inf.recordId && a.Pos == 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(header != null)
                    {
                        header.Pos = 3;
                        header.ApprovedUsr = inf.usr.uCode;
                        header.ApprovedDat = ac.getPresentDateTime();
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
                    msg = "You are not authorised for this transaction";
                }
            }
            catch(Exception ee)
            {
                msg = ee.Message;
            }
            result.result = msg;
            return result;
        }


    }
}
