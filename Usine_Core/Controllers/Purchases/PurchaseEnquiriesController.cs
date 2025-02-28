using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;

using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Authorization;
using Usine_Core.others;
using Usine_Core.Controllers.Accounts;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;

namespace Usine_Core.Controllers.Purchases
{
    public class PurchaseEnquiryInformation
    {
        public PurPurchaseEnquiryUni header { get; set; }
        public dynamic lines { get; set; }
        public List<PurPurchaseEnquiryNotes> notes { get; set; }
        public string result { get; set; }
    }
    public class SuppDets
    {
       public int supcode { get; set; }
        public string addr { get; set; }
    }
    public class PurPurchaseEnquiryTotal
    {
        public PurPurchaseEnquiryUni header { get; set; }
        public List<SuppDets> supps { get; set; }
        public List<PurPurchaseEnquiryDet> lines { get; set; }
        public List<PurPurchaseEnquiryNotes> notes { get; set; }
        public int? traCheck { get; set; }
        public UserInfo usr { get; set; }
    }

    public class PurchaseEnquiriesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        private readonly IHostingEnvironment ho;
        public PurchaseEnquiriesController(IHostingEnvironment hostingEnvironment)
        {
            ho = hostingEnvironment;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/GetPurchaseEnquiryRequirements")]
        public PurchaseRequestRequirements GetPurchaseEnquiryRequirements([FromBody] UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            DateTime dat2 = DateTime.Parse(ac.strDate(ac.getPresentDateTime())).AddDays(1);
            PurchaseRequestRequirements tot = new PurchaseRequestRequirements();
            tot.seq = findEnquirySeq(usr);
            tot.dat = ac.strDate(ac.getPresentDateTime());
           
            tot.suppliers = (from a in db.PartyDetails.Where(a => a.PartyType == "SUP" && a.Statu==1 && a.CustomerCode == usr.cCode)
                             join b in db.PurSupplierGroups.Where(a => a.CustomerCode == usr.cCode) on a.PartyGroup equals b.RecordId
                             select new
                             {
                                 recordid = a.RecordId,
                                 supplier = a.PartyName,
                                 partygroup = b.SGrp,
                                
                             }).OrderBy(b => b.supplier).ToList();
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).ToList();

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
                                 stock = subdet == null ? 0 : (subdet.qty==null?0:subdet.qty),
                                 um = a.Um,
                             }).OrderBy(b => b.itemname).ToList();

            tot.materialunits = (from a in db.InvMaterialUnits.Where(a => a.CustomerCode == usr.cCode)
                                 join b in db.InvUm.Where(a => a.CustomerCode == usr.cCode) on a.Um equals b.RecordId
                                 select new
                                 {
                                     recordid = a.RecordId,
                                     sno = a.Sno,
                                     umid = b.RecordId,
                                     um = b.Um
                                 }).OrderBy(a => a.recordid).ThenBy(b => b.sno).ToList();
            tot.purchasenotes = (from a in db.PurTerms.Where(a => a.CustomerCode == usr.cCode)
                                 select new
                                 {
                                     sno = a.Slno,
                                     term = a.Term,
                                     chk=false
                                 }
                               ).OrderBy(b => b.sno).ToList();
            return tot;
        }
        private string findEnquirySeq(UserInfo usr)
        {
            DateTime dat1 = ac.getFinancialStart(ac.getPresentDateTime(), usr);
            General g = new General();
            string str = "PE" + g.right(dat1.Year.ToString(), 2) + "-";
            int x = 0;
            UsineContext db1 = new UsineContext();
            var det = db1.PurPurchaseEnquiryUni.Where(a => a.Seq.Contains(str) && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if(det != null)
            {
                x = g.valInt(g.right(det, 5));
            }
            x++;
            return str + g.zeroMake(x, 5);

        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/GetPurchaseEnquiries")]
        public List<PurPurchaseEnquiryUni> GetPurchaseEnquiries([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);

            return db.PurPurchaseEnquiryUni.Where(a => a.Dat >= dat1 && a.Dat < dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/GetPurchaseEnquiry")]
        public PurchaseEnquiryInformation GetPurchaseEnquiry([FromBody] GeneralInformation inf)
        {
            PurchaseEnquiryInformation tot = new PurchaseEnquiryInformation();
            if (purEnquiryPreviousCheck(inf))
            {
              
                tot.header = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                tot.lines = (from a in 
                                 (from p in db.PurPurchaseEnquiryDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                  join q in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on p.Uom equals q.RecordId
                                  select new
                                  {
                                      p.RecordId,
                                      p.Sno,
                                      p.ItemId,
                                      p.Uom,
                                      q.Um,
                                      p.ItemDescription,
                                      p.Qty
                                  })
                             join b in db.InvMaterials.Where(a => a.CustomerCode == inf.usr.cCode) on a.ItemId equals b.RecordId
                             select new
                             {
                                 recordid = a.RecordId,
                                 sno = a.Sno,
                                 itemid = a.ItemId,
                                 itemname = b.ItemName,
                                 umid=a.Uom,
                                 um = a.Um,
                                 itemdescription = a.ItemDescription,
                                 qty = a.Qty
                             }).OrderBy(b => b.sno).ToList();
                tot.notes = db.PurPurchaseEnquiryNotes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                tot.result = "OK";
                return tot;
            }
            else
            {
                tot.result = "There is quotation against this enquiry modification or deletion not possible";
                return null;
            }
        }
        private Boolean purEnquiryPreviousCheck(GeneralInformation inf)
        {
            return true;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/setPurchaseEnquiry")]
        public PurchaseTransactionResult setPurchaseEnquiry([FromBody]PurPurchaseEnquiryTotal tot)
        {
            string msg = "";
            int? sno = 1;
            UsineContext db1 = new UsineContext();
            General gg = new General();
            var statuscheck = db.PurSetup.Where(a => a.SetupCode == "pur_enq" && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
            int? statu = 2;
            if(statuscheck != null)
            {
                statu = gg.valInt(statuscheck.SetupValue) == 1 ? 2 : 1;
            }
            var traresult = ac.transactionCheck("Inventory", ac.getPresentDateTime(), tot.usr);
            if (traresult == "OK")
            {
                try
                {
                    if (ac.screenCheck(tot.usr, 2, 2, 3, (int)tot.traCheck))
                    {
                        switch(tot.traCheck)
                        {
                            case 1:
                                foreach(var sup in tot.supps)
                                {
                                    using (var txn = db.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            var supplier = db1.PartyDetails.Where(a => a.RecordId == sup.supcode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                            var addr = db1.PartyAddresses.Where(a => a.AddressName == sup.addr && a.RecordId == sup.supcode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                            if (supplier != null)
                                            {
                                                var header = new PurPurchaseEnquiryUni();
                                                header.Seq = findEnquirySeq(tot.usr);
                                                header.Dat = ac.DateAdjustFromFrontEnd(tot.header.Dat.Value);
                                                header.Usr = tot.usr.uCode;
                                                header.Validity = ac.DateAdjustFromFrontEnd(tot.header.Validity.Value);
                                                header.Supid = supplier.RecordId;
                                                header.Supplier = supplier.PartyName;
                                                if (addr != null)
                                                {
                                                    header.Addr = addr.Addres;
                                                    header.Country = addr.Country;
                                                    header.Stat = addr.Stat;
                                                    header.District = addr.District;
                                                    header.City = addr.City;
                                                    header.Zip = addr.Zip;
                                                    header.Mobile = addr.Mobile;
                                                    header.Tel = addr.Tel;
                                                    header.Fax = addr.Fax;
                                                    header.Email = addr.Email;
                                                    header.Web = addr.Webid;
                                                }
                                                header.Pos = statu;
                                                header.PrintCount = 0;
                                                header.MailCount = 0;
                                                header.BranchId = tot.usr.bCode;
                                                header.CustomerCode = tot.usr.cCode;
                                                db.PurPurchaseEnquiryUni.Add(header);
                                                db.SaveChanges();
                                                List<PurPurchaseEnquiryDet> lines = new List<PurPurchaseEnquiryDet>();
                                                foreach (PurPurchaseEnquiryDet line in tot.lines)
                                                {
                                                    lines.Add(new PurPurchaseEnquiryDet
                                                    {
                                                        RecordId=header.RecordId,
                                                        Sno=sno,
                                                        ItemId=line.ItemId,
                                                        ItemDescription=line.ItemDescription,
                                                        Qty=line.Qty,
                                                        Uom=line.Uom,
                                                        BranchId=tot.usr.bCode,
                                                        CustomerCode=tot.usr.cCode
                                                    });
                                                    sno++;
                                                }
                                                db.PurPurchaseEnquiryDet.AddRange(lines);

                                                List<PurPurchaseEnquiryNotes> notes = new List<PurPurchaseEnquiryNotes>();  
                                                sno = 1;
                                                if (tot.notes != null)
                                                {
                                                    if (tot.notes.Count() > 0)
                                                    {
                                                        foreach (PurPurchaseEnquiryNotes note in tot.notes)
                                                        {
                                                            notes.Add(new PurPurchaseEnquiryNotes
                                                            {
                                                                RecordId = header.RecordId,
                                                                Sno = sno,
                                                                Note = note.Note,
                                                                BranchId = tot.usr.bCode,
                                                                CustomerCode = tot.usr.cCode
                                                            });

                                                            sno++;
                                                        }

                                                        db.PurPurchaseEnquiryNotes.AddRange(notes);
                                                    }
                                                }

                                                PurPurchaseEnquiryUniHistory audheader = new PurPurchaseEnquiryUniHistory();
                                                audheader.AuditDat = ac.getPresentDateTime();
                                                audheader.AuditType = 1;
                                                audheader.RecordId = tot.header.RecordId;
                                                audheader.Seq = tot.header.Seq;
                                                audheader.Dat = tot.header.Dat;
                                                audheader.Usr = tot.usr.uCode;
                                                audheader.Validity = tot.header.Validity;
                                                audheader.Reference = tot.header.Reference;
                                                audheader.Supid = supplier.RecordId;
                                                audheader.Supplier = supplier.PartyName;
                                                if (addr != null)
                                                {
                                                    audheader.Addr = addr.Addres;
                                                    audheader.Country=addr.Country;
                                                    audheader.Stat = addr.Stat;
                                                    audheader.District = addr.District;
                                                    audheader.City = addr.City;
                                                    audheader.Zip = addr.Zip;
                                                    audheader.Mobile = addr.Mobile;
                                                    audheader.Tel = addr.Tel;
                                                    audheader.Fax = addr.Fax;
                                                    audheader.Email = addr.Email;
                                                    audheader.Web = addr.Webid;
                                                }
                                                audheader.Pos = statu   ;
                                                audheader.PrintCount = 0;
                                                audheader.MailCount = 0;
                                                audheader.BranchId = tot.usr.bCode;
                                                audheader.CustomerCode = tot.usr.cCode;
                                                db.PurPurchaseEnquiryUniHistory.Add(audheader);
                                                db.SaveChanges();

                                                sno = 1;
                                                List<PurPurchaseEnquiryDetHistory> audlines = new List<PurPurchaseEnquiryDetHistory>();
                                                foreach (PurPurchaseEnquiryDet line in tot.lines)
                                                {
                                                    audlines.Add(new PurPurchaseEnquiryDetHistory
                                                    {
                                                        AuditId=audheader.AuditId,
                                                        AuditDat=ac.getPresentDateTime(),
                                                        AuditType=1,
                                                        RecordId = header.RecordId,
                                                        Sno = (int)sno,
                                                        ItemId = line.ItemId,
                                                        ItemDescription = line.ItemDescription,
                                                        Qty = line.Qty,
                                                        uom=line.Uom,
                                                        BranchId = tot.usr.bCode,
                                                        CustomerCode = tot.usr.cCode
                                                    });
                                                    sno++;
                                                }
                                                db.PurPurchaseEnquiryDetHistory.AddRange(audlines);
                                                if (tot.notes != null)
                                                {
                                                    if (tot.notes.Count() > 0)
                                                    {
                                                        List<PurPurchaseEnquiryNotesHistory> audnotes = new List<PurPurchaseEnquiryNotesHistory>();
                                                        sno = 1;
                                                        foreach (PurPurchaseEnquiryNotes note in tot.notes)
                                                        {
                                                            audnotes.Add(new PurPurchaseEnquiryNotesHistory
                                                            {
                                                                AuditId = audheader.AuditId,
                                                                AuditDat = ac.getPresentDateTime(),
                                                                AuditType = 1,
                                                                RecordId = header.RecordId,
                                                                Sno = (int)sno,
                                                                Note = note.Note,
                                                                BranchId = tot.usr.bCode,
                                                                CustomerCode = tot.usr.cCode
                                                            });

                                                            sno++;
                                                        }
                                                        db.PurPurchaseEnquiryNotesHistory.AddRange(audnotes);
                                                    }
                                                }


                                                TransactionsAudit audit = new TransactionsAudit();
                                                audit.TraId = (int)header.RecordId;
                                                audit.Descr = "A new Enquriry created with Id of " + header.Seq;
                                                audit.Usr = tot.usr.uCode;
                                                audit.Tratype = 1;
                                                audit.Transact = "PUR_ENQ";
                                                audit.TraModule = "PUR";
                                                audit.Syscode = "Purchase Enquiry";
                                                audit.BranchId = tot.usr.bCode;
                                                audit.CustomerCode = tot.usr.cCode;
                                                audit.Dat = ac.getPresentDateTime();
                                                db.TransactionsAudit.Add(audit);
                                                db.SaveChanges();

                                                txn.Commit();
                                                msg = "OK";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            msg = ex.Message;
                                            txn.Rollback();
                                        }
                                    }
                                }
                                break;
                            case 2:
                                var supupd = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                                if(supupd != null)
                                {
                                    supupd.Usr = tot.usr.uCode;
                                    supupd.Validity = ac.DateAdjustFromFrontEnd(tot.header.Validity.Value);
                                    supupd.Supid = tot.header.Supid;
                                    supupd.Usr = tot.usr.uCode;
                                    supupd.Supplier = tot.header.Supplier;
                                    supupd.Addr = tot.header.Addr;
                                    supupd.Country = tot.header.Country;
                                    supupd.Stat = tot.header.Stat;
                                    supupd.District = tot.header.District;
                                    supupd.City = tot.header.City;
                                    supupd.Zip = tot.header.Zip;
                                    supupd.Mobile = tot.header.Mobile;
                                    supupd.Tel = tot.header.Tel;
                                    supupd.Fax = tot.header.Fax;
                                    supupd.Email = tot.header.Email;
                                    supupd.Web = tot.header.Web;
                                    //supupd.Pos = 1;

                                    sno = 0;
                                    var linedetupd = db.PurPurchaseEnquiryDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if(linedetupd.Count() > 0)
                                    {
                                        sno = linedetupd.Max(a => a.Sno);
                                        db.PurPurchaseEnquiryDet.RemoveRange(linedetupd);
                                    }
                                    sno++;

                                    List<PurPurchaseEnquiryDet> linesupd = new List<PurPurchaseEnquiryDet>();
                                    foreach (PurPurchaseEnquiryDet line in tot.lines)
                                    {
                                        linesupd.Add(new PurPurchaseEnquiryDet
                                        {
                                            RecordId = tot.header.RecordId,
                                            Sno = sno,
                                            ItemId = line.ItemId,
                                            ItemDescription = line.ItemDescription,
                                            Qty = line.Qty,
                                            Uom = line.Uom,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode
                                        });
                                        sno++;
                                    }
                                    db.PurPurchaseEnquiryDet.AddRange(linesupd);



                                    sno = 0;
                                    var linenoteupd = db.PurPurchaseEnquiryNotes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                    if (linenoteupd.Count() > 0)
                                    {
                                        sno = linenoteupd.Max(a => a.Sno);
                                        db.PurPurchaseEnquiryDet.RemoveRange(linedetupd);
                                    }
                                    sno++;

                                    List<PurPurchaseEnquiryNotes> notesupd = new List<PurPurchaseEnquiryNotes>();
                                    sno = 1;
                                    if (tot.notes != null)
                                    {
                                        if (tot.notes.Count() > 0)
                                        {
                                            foreach (PurPurchaseEnquiryNotes note in tot.notes)
                                            {
                                                notesupd.Add(new PurPurchaseEnquiryNotes
                                                {
                                                    RecordId = tot.header.RecordId,
                                                    Sno = sno,
                                                    Note = note.Note,
                                                    BranchId = tot.usr.bCode,
                                                    CustomerCode = tot.usr.cCode
                                                });

                                                sno++;
                                            }

                                            db.PurPurchaseEnquiryNotes.AddRange(notesupd);
                                        }
                                    }


                                    PurPurchaseEnquiryUniHistory audheaderupd = new PurPurchaseEnquiryUniHistory();
                                    audheaderupd.AuditDat = ac.getPresentDateTime();
                                    audheaderupd.AuditType = 2;
                                    audheaderupd.RecordId = tot.header.RecordId;
                                    audheaderupd.Seq = tot.header.Seq;
                                    audheaderupd.Dat = tot.header.Dat;
                                    audheaderupd.Usr = tot.usr.uCode;
                                    audheaderupd.Validity = tot.header.Validity;
                                    audheaderupd.Reference = tot.header.Reference;
                                    audheaderupd.Supid = tot.header.Supid;
                                    audheaderupd.Supplier = tot.header.Supplier;
                                    audheaderupd.Addr = tot.header.Addr;
                                    audheaderupd.Country=tot.header.Country;
                                    audheaderupd.Stat = tot.header.Stat;
                                    audheaderupd.District = tot.header.District;
                                    audheaderupd.City = tot.header.City;
                                    audheaderupd.Zip = tot.header.Zip;
                                    audheaderupd.Mobile = tot.header.Mobile;
                                    audheaderupd.Tel = tot.header.Tel;
                                    audheaderupd.Fax = tot.header.Fax;
                                    audheaderupd.Email = tot.header.Email;
                                    audheaderupd.Web = tot.header.Web;
                                    audheaderupd.Pos = 2;
                                    audheaderupd.PrintCount = 0;
                                    audheaderupd.MailCount = 0;
                                    audheaderupd.BranchId = tot.usr.bCode;
                                    audheaderupd.CustomerCode = tot.usr.cCode;
                                    db.PurPurchaseEnquiryUniHistory.Add(audheaderupd);
                                    db.SaveChanges();

                                    sno = 1;
                                    List<PurPurchaseEnquiryDetHistory> audlinesupd = new List<PurPurchaseEnquiryDetHistory>();
                                    foreach (PurPurchaseEnquiryDet line in tot.lines)
                                    {
                                        audlinesupd.Add(new PurPurchaseEnquiryDetHistory
                                        {
                                            AuditId = audheaderupd.AuditId,
                                            AuditDat = ac.getPresentDateTime(),
                                            AuditType = 1,
                                            RecordId = tot.header.RecordId,
                                            Sno = (int)sno,
                                            ItemId = line.ItemId,
                                            ItemDescription = line.ItemDescription,
                                            Qty = line.Qty,
                                            uom = line.Uom,
                                            BranchId = tot.usr.bCode,
                                            CustomerCode = tot.usr.cCode
                                        });
                                        sno++;
                                    }
                                    db.PurPurchaseEnquiryDetHistory.AddRange(audlinesupd);
                                    if (tot.notes != null)
                                    {
                                        if (tot.notes.Count() > 0)
                                        {
                                            List<PurPurchaseEnquiryNotesHistory> audnotes = new List<PurPurchaseEnquiryNotesHistory>();
                                            sno = 1;
                                            foreach (PurPurchaseEnquiryNotes note in tot.notes)
                                            {
                                                audnotes.Add(new PurPurchaseEnquiryNotesHistory
                                                {
                                                    AuditId = audheaderupd.AuditId,
                                                    AuditDat = ac.getPresentDateTime(),
                                                    AuditType = 2,
                                                    RecordId = tot.header.RecordId,
                                                    Sno = (int)sno,
                                                    Note = note.Note,
                                                    BranchId = tot.usr.bCode,
                                                    CustomerCode = tot.usr.cCode
                                                });

                                                sno++;
                                            }
                                            db.PurPurchaseEnquiryNotesHistory.AddRange(audnotes);
                                        }
                                    }


                                    TransactionsAudit auditupd = new TransactionsAudit();
                                    auditupd.TraId = (int)tot.header.RecordId;
                                    auditupd.Descr = "Enquriry id " + tot.header.Seq + " Modified ";
                                    auditupd.Usr = tot.usr.uCode;
                                    auditupd.Tratype = 2;
                                    auditupd.Transact = "PUR_ENQ";
                                    auditupd.TraModule = "PUR";
                                    auditupd.Syscode = "Purchase Enquiry";
                                    auditupd.BranchId = tot.usr.bCode;
                                    auditupd.CustomerCode = tot.usr.cCode;
                                    auditupd.Dat = ac.getPresentDateTime();
                                    db.TransactionsAudit.Add(auditupd);
                                    db.SaveChanges();
                                    msg = "OK";

                                }
                                break;
                            case 3:


                                using (var txn = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var enqdel = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).FirstOrDefault();

                                        var linesdel = db.PurPurchaseEnquiryDet.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();
                                        var notesdel = db.PurPurchaseEnquiryNotes.Where(a => a.RecordId == tot.header.RecordId && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).ToList();



                                        PurPurchaseEnquiryUniHistory audheaderdel = new PurPurchaseEnquiryUniHistory();
                                        audheaderdel.AuditDat = ac.getPresentDateTime();
                                        audheaderdel.AuditType = 3;
                                        audheaderdel.RecordId = tot.header.RecordId;
                                        audheaderdel.Seq = tot.header.Seq;
                                        audheaderdel.Dat = enqdel.Dat;
                                        audheaderdel.Usr = enqdel.Usr;
                                        audheaderdel.Validity = enqdel.Validity;
                                        audheaderdel.Reference = enqdel.Reference;
                                        audheaderdel.Supid = enqdel.Supid;
                                        audheaderdel.Supplier = enqdel.Supplier;
                                        audheaderdel.Addr = enqdel.Addr;
                                        audheaderdel.Country= enqdel.Country;
                                        audheaderdel.Stat = enqdel.Stat;
                                        audheaderdel.District = enqdel.District;
                                        audheaderdel.City = enqdel.City;
                                        audheaderdel.Zip = enqdel.Zip;
                                        audheaderdel.Mobile = enqdel.Mobile;
                                        audheaderdel.Tel = enqdel.Tel;
                                        audheaderdel.Fax = enqdel.Fax;
                                        audheaderdel.Email = enqdel.Email;
                                        audheaderdel.Web = enqdel.Web;
                                        audheaderdel.Pos = 3;
                                        audheaderdel.PrintCount = 0;
                                        audheaderdel.MailCount = 0;
                                        audheaderdel.BranchId = tot.usr.bCode;
                                        audheaderdel.CustomerCode = tot.usr.cCode;
                                        db.PurPurchaseEnquiryUniHistory.Add(audheaderdel);
                                        db.SaveChanges();

                                        sno = 1;
                                        List<PurPurchaseEnquiryDetHistory> audlinesdel = new List<PurPurchaseEnquiryDetHistory>();
                                        foreach (PurPurchaseEnquiryDet line in tot.lines)
                                        {
                                            audlinesdel.Add(new PurPurchaseEnquiryDetHistory
                                            {
                                                AuditId = audheaderdel.AuditId,
                                                AuditDat = ac.getPresentDateTime(),
                                                AuditType = 1,
                                                RecordId = tot.header.RecordId,
                                                Sno = (int)sno,
                                                ItemId = line.ItemId,
                                                ItemDescription = line.ItemDescription,
                                                Qty = line.Qty,
                                                uom = line.Uom,
                                                BranchId = tot.usr.bCode,
                                                CustomerCode = tot.usr.cCode
                                            });
                                            sno++;
                                        }
                                        db.PurPurchaseEnquiryDetHistory.AddRange(audlinesdel);
                                        if (tot.notes != null)
                                        {
                                            if (tot.notes.Count() > 0)
                                            {
                                                List<PurPurchaseEnquiryNotesHistory> audnotesdel = new List<PurPurchaseEnquiryNotesHistory>();
                                                sno = 1;
                                                foreach (PurPurchaseEnquiryNotes note in tot.notes)
                                                {
                                                    audnotesdel.Add(new PurPurchaseEnquiryNotesHistory
                                                    {
                                                        AuditId = audheaderdel.AuditId,
                                                        AuditDat = ac.getPresentDateTime(),
                                                        AuditType = 2,
                                                        RecordId = tot.header.RecordId,
                                                        Sno = (int)sno,
                                                        Note = note.Note,
                                                        BranchId = tot.usr.bCode,
                                                        CustomerCode = tot.usr.cCode
                                                    });

                                                    sno++;
                                                }
                                                db.PurPurchaseEnquiryNotesHistory.AddRange(audnotesdel);
                                            }
                                        }
                                        db.PurPurchaseEnquiryDet.RemoveRange(linesdel);
                                        db.PurPurchaseEnquiryNotes.RemoveRange(notesdel);
                                        db.PurPurchaseEnquiryUni.Remove(enqdel);
                                        
                                        TransactionsAudit auditdel = new TransactionsAudit();
                                        auditdel.TraId = (int)tot.header.RecordId;
                                        auditdel.Descr = "Enquriry id " + tot.header.Seq + " Deleted ";
                                        auditdel.Usr = tot.usr.uCode;
                                        auditdel.Tratype = 3;
                                        auditdel.Transact = "PUR_ENQ";
                                        auditdel.TraModule = "PUR";
                                        auditdel.Syscode = "Purchase Enquiry";
                                        auditdel.BranchId = tot.usr.bCode;
                                        auditdel.CustomerCode = tot.usr.cCode;
                                        auditdel.Dat = ac.getPresentDateTime();
                                        db.TransactionsAudit.Add(auditdel);
                                        db.SaveChanges();
                                        txn.Commit();
                                        msg = "OK";
                                    }
                                    catch(Exception ee)
                                    {
                                        msg = ee.Message;
                                        txn.Rollback();
                                    }

                                }

   


                                break;
                        }
                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = traresult;
            }


            PurchaseTransactionResult resu = new PurchaseTransactionResult();
            resu.result = msg;
            return resu;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/PrintPurchaseEnquiry")]
        public VoucherResult PrintPurchaseEnquiry([FromBody] GeneralInformation inf)
        {
            VoucherResult result = new VoucherResult();
            string msg = "";
            string filename = "";
            String str = ho.WebRootPath + "     " + ho.ContentRootPath;
            DateTime dat = DateTime.Now;
            filename = inf.usr.uCode + "PURENQUIRY" + dat.Second.ToString() + dat.Minute.ToString() + dat.Hour.ToString()  + inf.usr.cCode + inf.usr.bCode + ".pdf";
            LoginControlController ll = new LoginControlController();
            UserAddress addr = ll.makeBranchAddress(inf.usr.bCode, inf.usr.cCode);
            int pagesize = 15;

            PurPurchaseEnquiryUni header = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            var lines = (from p in (from a in db.PurPurchaseEnquiryDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                                    join b in db.InvMaterials.Where(a => a.CustomerCode == inf.usr.cCode) on a.ItemId equals b.RecordId
                                    select new
                                    {
                                        a.Sno,
                                        a.ItemId,
                                        b.ItemName,
                                        a.ItemDescription,
                                        a.Qty,
                                        a.Uom
                                    })
                         join
q in db.InvUm.Where(a => a.CustomerCode == inf.usr.cCode) on p.Uom equals q.RecordId
                         select new
                         {
                             p.Sno,
                             p.ItemId,
                             p.ItemName,
                             p.ItemDescription,
                             p.Qty,
                             q.Um
                         }).OrderBy(b => b.Sno).ToList();


            using (FileStream ms = new FileStream(ho.WebRootPath + "\\Reps\\" + filename, FileMode.Append, FileAccess.Write))
            {
                Document document = new Document(PageSize.A4, 75f, 40f, 20f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();
                PdfPTable ptot = new PdfPTable(1);
                float[] widths = new float[] { 550f };
                ptot.SetWidths(widths);
                ptot.TotalWidth = 550f;
                ptot.LockedWidth = true;
                iTextSharp.text.Font fn;
                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 14, Font.BOLD));


                PdfPCell plC = new PdfPCell(makeHeader(addr,header,1,1));
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                 ptot.AddCell(plC);

                PdfPTable pbody = new PdfPTable(4);
                pbody.SetWidths(new float[] { 50f,350f, 90f,60f, });
                pbody.TotalWidth = 550f;
                pbody.LockedWidth = true;

                fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.BOLD));


                plC = new PdfPCell(new Phrase("#",fn));
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plC.BackgroundColor= BaseColor.LightGray;
                plC.BackgroundColor = BaseColor.LightGray;
                pbody.AddCell(plC);
                plC = new PdfPCell(new Phrase("Description", fn));
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plC.BackgroundColor = BaseColor.LightGray;
                plC.BackgroundColor = BaseColor.LightGray;
                pbody.AddCell(plC);
                plC = new PdfPCell(new Phrase("Qty", fn));
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plC.BackgroundColor = BaseColor.LightGray;
                plC.BackgroundColor = BaseColor.LightGray;
                pbody.AddCell(plC);
                plC = new PdfPCell(new Phrase("UOM", fn));
                plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                plC.BackgroundColor = BaseColor.LightGray;
                plC.BackgroundColor = BaseColor.LightGray;
                pbody.AddCell(plC);


                int sno = 1;
               
                foreach (var line in lines)
                {
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.NORMAL));
                    plC = new PdfPCell(new Phrase(sno.ToString(), fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(line.ItemName, fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(line.Qty.ToString(), fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(line.Um, fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(line.ItemDescription, fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase("", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                   pbody.AddCell(plC);

                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                   pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);

                    sno++;
                }

                while(sno <=25)
                {
                    fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.ITALIC));
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                     pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);
                    plC = new PdfPCell(new Phrase(" ", fn));
                    plC.BorderWidth = 0;
                    plC.BorderWidthRight = 1f;
                    plC.BorderWidthLeft = 1f;
                    plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                    pbody.AddCell(plC);

                    sno++;
                }

                plC = new PdfPCell(pbody);
               
                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                  ptot.AddCell(plC);

                plC = new PdfPCell(makeFooter());

                plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                ptot.AddCell(plC);

                document.Add(ptot);
                document.Close();
            }



                result.result = msg;
            result.filename = filename;
            return result;
        }
        private PdfPTable makeFooter()
        {
            PdfPTable footer = new PdfPTable(1);
            float[] widths = new float[] { 550f };
            footer.SetWidths(widths);
            footer.TotalWidth = 550f;
            footer.LockedWidth = true;
            iTextSharp.text.Font fn;

            PdfPCell plC;
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLDITALIC));
            for (int i = 1; i <= 4; i++)
            {
                plC = new PdfPCell(new Phrase(" ", fn));
                plC.BorderWidth = 0f;
                plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                 footer.AddCell(plC);
            }
            plC = new PdfPCell(new Phrase("Purchase Manager", fn));
            plC.BorderWidth = 0f;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            footer.AddCell(plC);

            return footer;
        }


        private PdfPTable makeHeader(UserAddress addr,PurPurchaseEnquiryUni header,int pagesize,int pages)
        {
            General g = new General();
            PdfPTable pheader = new PdfPTable(1);
            float[] widths = new float[] { 550f };
            pheader.SetWidths(widths);
            pheader.TotalWidth = 550f;
            pheader.LockedWidth = true;
            iTextSharp.text.Font fn;


            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
            PdfPCell plC = new PdfPCell(new Phrase("PURCHASE ENQUIRY", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            plC.BackgroundColor = BaseColor.LightGray;
            pheader.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 14, Font.BOLD));
            plC = new PdfPCell(new Phrase(addr.branchName, fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pheader.AddCell(plC);

            PdfPTable psub = new PdfPTable(2);
            psub.SetWidths(new float[] { 275f,275f });
            psub.TotalWidth = 550f;
            psub.LockedWidth = true;

            PdfPTable pright = new PdfPTable(2);
            pright.SetWidths(new float[] {100f, 175f });
            pright.TotalWidth = 275f;
            pright.LockedWidth = true;


            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
           
            plC = new PdfPCell(new Phrase("Enquiry #", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
             plC = new PdfPCell(new Phrase(header.Seq, fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase("Date", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(g.strDate(header.Dat.Value), fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pright.AddCell(plC);


            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase("GST", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(addr.fax, fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase("Validity", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pright.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(g.strDate(header.Validity.Value), fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pright.AddCell(plC);


            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase(addr.address + "\n" + addr.city + "\n" + addr.mobile , fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_TOP;
             psub.AddCell(plC);

            plC = new PdfPCell(pright);


           psub.AddCell(plC);

            plC = new PdfPCell(psub);
            pheader.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 10, Font.BOLD));
            plC = new PdfPCell(new Phrase("Supplier Details", fn));
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pheader.AddCell(plC);


            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 12, Font.BOLD));
            plC = new PdfPCell(new Phrase(header.Supplier, fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
             pheader.AddCell(plC);

            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase(header.Addr, fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pheader.AddCell(plC);
            fn = new iTextSharp.text.Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));
            plC = new PdfPCell(new Phrase(header.City + ", " + header.Stat, fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pheader.AddCell(plC);

            plC = new PdfPCell(new Phrase( " ", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pheader.AddCell(plC);
            plC = new PdfPCell(new Phrase(" ", fn));
            plC.BorderWidth = 0;
            plC.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            plC.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            pheader.AddCell(plC);
            return pheader;
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/sendEnquiryMail")]
        public TransactionResult sendEnquiryMail([FromBody] GeneralInformation inf)
        {
            string msg = "";
            try
            {
                UsineContext db = new UsineContext();
                string frmmail = "", pwd = "", cc = "", serv = "",   pic = "";
                int port = 0;
                General gg = new General();
                LoginControlController ll = new LoginControlController();
                var addr=ll.makeBranchAddress(inf.usr.bCode, inf.usr.cCode);
                var details = db.PurEmails.Where(a => a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                var header = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                var enqdetails = db.MisCoveringLetterDetails.Where(a => a.Typ == "PUR_ENQ" && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                Boolean b = header.MailCount == 0 ? ac.screenCheck(inf.usr, 2, 2, 3, 9) : ac.screenCheck(inf.usr, 2, 2, 3, 99);
                   
                if (details.Count() > 0 && header != null && enqdetails != null && b)
                {
                    frmmail = details.Where(a => a.SetupCode == "ema_frm").Select(b => b.SetupValue).FirstOrDefault();
                    pwd = details.Where(a => a.SetupCode == "ema_pwd").Select(b => b.SetupValue).FirstOrDefault();
                    cc = details.Where(a => a.SetupCode == "ema_cc").Select(b => b.SetupValue).FirstOrDefault();
                    serv = details.Where(a => a.SetupCode == "ema_ser").Select(b => b.SetupValue).FirstOrDefault();
                    port= gg.valInt(details.Where(a => a.SetupCode == "ema_por").Select(b => b.SetupValue).FirstOrDefault());
                    pic = details.Where(a => a.SetupCode == "ema_pic").Select(b => b.SetupValue).FirstOrDefault();



                    string body = "";
                    
                    body = body + " <table style=\"width: 80%;\">";
                    body = body + " <tr>";
                    body = body + " <td style=\"width: 50%;\">";
                    body = body + " <img src=\"" + enqdetails.Img + "\" width=\"200px\">";
                    body = body + " </td>";
                    body = body + " <td style=\"width: 50%;\">&nbsp;</td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\"></td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\" align=\"right\">" + ac.strDate(header.Dat.Value) + "</td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td style=\"width: 50%;\">From</td>";
                    body = body + " <td style=\"width: 50%;\">To</td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td style=\"width: 50%;\">" + addr.branchName + "</td>";
                    body = body + " <td style=\"width: 50%;\">" + header.Supplier + "</td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td style=\"width: 50%;\">" + addr.city + "</td>";
                    body = body + " <td style=\"width: 50%;\">" + header.City + "</td>";
                    body = body + " </tr>";

                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\">" + enqdetails.Salutation + " </td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\"> &nbsp; </td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\">" + enqdetails.Subjec + " </td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\"> &nbsp; </td>";
                    body = body + " </tr>";
                    body = body + " <tr>";
                    body = body + " <td colspan=\"2\">" + enqdetails.Body + " </td>";
                    body = body + " </tr>";
                    body = body + " </table>";
                   


                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(serv);
                    var res = PrintPurchaseEnquiry(inf);
                    string filename = res.filename;
                    mail.From = new MailAddress(frmmail);
                    mail.IsBodyHtml = true;
                     mail.To.Add(header.Email);
                    mail.Attachments.Add(new Attachment(ho.WebRootPath + "\\Reps\\" + filename));
                    mail.CC.Add(cc);
                    mail.Subject = "Enquiry Details";
                    
                    mail.Body = body;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.UseDefaultCredentials = true;
                    SmtpServer.Port = port;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(frmmail, pwd);




                    SmtpServer.SendAsync(mail, new object());
                    header.MailCount = header.MailCount + 1;
                    db.SaveChanges();
                    msg = "OK";


                }
                else
                {
                    msg = b?"Details not set for mail":"You are not authorised";
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

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/GetPurchaseEnquiriesForApprovals")]
        public List<PurPurchaseEnquiryUni> GetPurchaseEnquiriesForApprovals([FromBody] UserInfo usr)
        {
           
            return db.PurPurchaseEnquiryUni.Where(a => a.Pos==1 && a.BranchId ==  usr.bCode && a.CustomerCode ==  usr.cCode).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurchaseEnquiries/SetPurchaseEnquiryForApprovals")]
        public TransactionResult SetPurchaseEnquiryForApprovals([FromBody] GeneralInformation inf)
        {
            string msg = "";
            try
            {
                if(ac.screenCheck(inf.usr,2,2,3,98))
                {
                    var header = db.PurPurchaseEnquiryUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    header.Pos = 3;
                    db.SaveChanges();
                    msg = "OK";
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
            TransactionResult res = new TransactionResult();
            res.recordId = (int)inf.recordId;
            res.result = msg;
            return res;
          }

    }
}
