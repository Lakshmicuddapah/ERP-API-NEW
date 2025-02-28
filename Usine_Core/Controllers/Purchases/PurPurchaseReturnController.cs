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
using System.Data;
using System.Data.SqlClient;

namespace Usine_Core.Controllers.Purchases
{
    public class PurPurchaseReturnTotal
    {
        public PurPurchaseReturnsUni header { get; set; }
        public List<PurPurchaseReturnsDet> lines { get; set; }
        public List<PurPurchaseReturnTaxes> taxes { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class PurchaseReturnRequirements
    {
        public string seq { get; set; }
        public DateTime dat { get; set; }
        public List<AdmTaxes> taxes { get; set; }
      }
    public class PurchaseInformation
    {
        public int? recordId { get; set; }
        public int? sno { get; set; }
        public int? itemid { get; set; }
        public string gin { get; set; }
        public string itemname { get; set; }
        public string batchno { get; set; }
        public string manudate { get; set; }
        public string expdate { get; set; }
        public double? qty { get; set; }
        public double? qtyout { get; set; }
        public double? available { get; set; }
        public double? rat { get; set; }
        public int? umid { get; set; }
        public string um { get; set; }
        public double? conversion { get; set; }
    }
    public class PurchaseInformationTotal
    {
        public PurPurchasesUni header { get; set; }
        public List<PurchaseInformation> lines { get; set; }
        public double? prevPurchaseReturn { get; set; }
        public double? prevCreditNotes { get; set; }
    }
    public class PurPurchaseReturnController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/GetPurchaseReturnRequirements")]
        public PurchaseReturnRequirements GetPurchaseReturnRequirements([FromBody] UserInfo usr)
        {
            PurchaseReturnRequirements tot = new PurchaseReturnRequirements();

            tot.seq = findSeq(usr);
            tot.dat = ac.getPresentDateTime();
              tot.taxes = db.AdmTaxes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.TaxCode).ThenBy(c => c.TaxPer).ToList();
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/GetPurchaseReturns")]
        public List<PurPurchaseReturnsUni> GetPurchaseReturns([FromBody] GeneralInformation inf)
        {
            try
            {
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);
                var list1 =  db.PurPurchaseReturnsUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                var list2 = db.PurPurchasesUni.Where(a => a.CustomerCode == inf.usr.cCode).ToList();
                 var detail = (from a in list1 
                              join b in list2 on a.RefMir equals b.RecordId
                               select new PurPurchaseReturnsUni
                              {
                                  RecordId = a.RecordId,
                                  Seq = a.Seq,
                                  BranchId = b.Seq,
                                  Webid = b.Invoiceno,
                                  Vendorname = a.Vendorname,
                                  Mobile = a.Mobile,
                                  Baseamt = a.Baseamt,
                                  Discount = a.Discount,
                                  Others = a.Others,
                                  TotalAmt = a.TotalAmt
                              }).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
                return detail;
            }
            catch(Exception ee)
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/GetPurchaseReturn")]
        public PurPurchasesTotal GetPurchaseReturn([FromBody] GeneralInformation inf)
        {
            PurPurchasesTotal tot = new PurPurchasesTotal();
            string msg = "";
            try
            {
                UsineContext db1 = new UsineContext();
                List<string> gins = db1.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.Gin).ToList();
                var det = db1.InvMaterialManagement.Where(a => gins.Contains(a.Gin) && a.TransactionType > 100 && a.Qtyout > 0 && a.CustomerCode == inf.usr.cCode);


                tot.header = (from a in db.PurPurchaseReturnsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                              select new PurPurchasesUni
                              {
                                  RecordId=a.RecordId,
                                  Seq=a.Seq,
                                  Vendorid=a.Vendorid,
                                  Vendorname=a.Vendorname,
                                  Addr=a.Addr,
                                  Country=a.Country,
                                  Stat=a.Stat,
                                  District=a.District,
                                  City=a.City,
                                  Zip=a.Zip,
                                  Mobile=a.Mobile,
                                  Tel=a.Tel,
                                  Fax=a.Fax,
                                  Email=a.Email,
                                  Webid=a.Webid,
                                  Baseamt=a.Baseamt,
                                  Discount=a.Discount,
                                  Taxes=a.Taxes,
                                  Others=a.Others,
                                  TotalAmt=a.TotalAmt
                              }).FirstOrDefault();

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

                  //  tot.taxes = db.PurPurchaseTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
                    msg = "OK";
                
                tot.result = msg;
                return tot;
            }
            catch (Exception ee)
            {
                tot.result = ee.Message;
                return tot;
            }

        }

        public String findSeq(UserInfo usr)
        {
            UsineContext db = new UsineContext();
            General g = new General();
            int x = 0;
            AdminControl ac = new AdminControl();
            DateTime dat = ac.getPresentDateTime();
            var xx = db.PurPurchaseReturnsUni.Where(a => a.Dat.Value.Month == dat.Month && a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if (xx != null)
            {
                x = g.valInt(g.right(xx,4));
            }
            x++;
            return "PRT" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/" + g.zeroMake(x, 4);
        }
        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/GetPurchases")]
        public List<PurPurchasesUni> GetPurchases([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            return db.PurPurchasesUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/GetPurchase")]
        public PurchaseInformationTotal GetPurchase([FromBody] GeneralInformation inf)
        {
            PurchaseInformationTotal tot = new PurchaseInformationTotal();
               tot.header = db.PurPurchasesUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            tot.lines = new List<PurchaseInformation>();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            string quer = "";

            quer = quer + " select a.recordId,a.sno,itemid,a.gin,itemname,batchno,case when manudate is null then '' else dbo.strdate(manudate) end manudate,case when expdate is null then '' else dbo.strdate(expdate) end expdate, qty, qtyout, available, a.rat,umid,um,conversionfactor from";
            quer = quer + " (select recordId, sno, itemid, a.gin, itemname, batchno, manudate, expdate, qty, a.rat, (case when qtyout is null then 0 else qtyout end)/ conversionfactor qtyout,qty - ((case when qtyout is null then 0 else qtyout end)/ conversionfactor) available,umid,conversionfactor from";
            quer = quer + " (select a.recordId, a.sno, a.itemid, b.gin, a.itemname, a.batchno, a.manudate, a.expdate, a.qty, umid, conversionFactor, a.rat from";
            quer = quer + " (select a.recordId, a.sno, a.itemid, a.itemname, a.batchno, a.manudate, a.expdate, a.qty, a.um umid, b.conversionFactor, a.rat from";
            quer = quer + " (select* from purpurchasesdet where recordId = " + inf.recordId +  " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId and a.um = b.um )a,";
            quer = quer + " (select transactionId, sno, gin from invMaterialManagement where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.transactionId and a.sno = b.sno)a left outer join";
            quer = quer + " (select gin, sum(qtyout) qtyout from invMaterialManagement where gin in";
            quer = quer + " (select gin from invMaterialmanagement where transactionId= " + inf.recordId + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")";
            quer = quer + " and qtyout > 0 and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by gin)b on a.gin = b.gin)a,";
            quer = quer + " (select * from invUM where customercode = " + inf.usr.cCode + ")b where a.umid = b.recordid";



            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while(dr.Read())
            {
                tot.lines.Add(new PurchaseInformation
                {
                    recordId=gg.valInt(dr[0].ToString()),
                    sno=gg.valInt(dr[1].ToString()),    
                    itemid=gg.valInt(dr[2].ToString()),
                    gin=dr[3].ToString(),
                    itemname=dr[4].ToString(),
                    batchno=dr[5].ToString(),
                    manudate=dr[6].ToString(),
                    expdate=dr[7].ToString(),
                    qty=gg.valNum(dr[8].ToString()),
                    qtyout=gg.valNum(dr[9].ToString()),
                    available=gg.valNum(dr[10].ToString()),
                    rat=gg.valNum(dr[11].ToString()),
                    umid=gg.valInt(dr[12].ToString()),
                    um=dr[13].ToString(),
                    conversion=gg.valNum(dr[14].ToString())
                });
            }
            dr.Close();
            g.db.Close();

            tot.prevPurchaseReturn = db.PartyTransactions.Where(a => a.OnTraId == inf.recordId && a.TransactionType == "PRT" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Sum(b => b.ReturnAmount);
            tot.prevCreditNotes = db.PartyTransactions.Where(a => a.OnTraId == inf.recordId && a.TransactionType == "PCR" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Sum(b => b.CreditNote);


            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/SetPurchaseReturn")]
        public TransactionResult SetPurchaseReturn([FromBody] PurPurchaseReturnTotal tot)
        {
            String msg = "";
            General gg = new General();
            int? store = db.PurPurchasesDet.Where(a => a.RecordId == tot.header.RefMir && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.Store).FirstOrDefault();
            AdminControl ac = new AdminControl();
            try
            {

                String tramsg = ac.transactionCheck("Inventory", tot.header.Dat, tot.usr);
                if (tramsg == "OK")
                {
                    switch(tot.traCheck)
                    {
                        case 1:
                         
                    if (ac.screenCheck(tot.usr, 2, 3, 6, 1))
                    {
                        tot.header.Seq=findSeq(tot.usr);
                        tot.header.Dat = ac.getPresentDateTime();
                        tot.header.BranchId = tot.usr.bCode;
                        tot.header.CustomerCode = tot.usr.cCode;
                        db.PurPurchaseReturnsUni.Add(tot.header);
                        db.SaveChanges();
                        int sno = 1;
                        List<InvMaterialManagement> mgts = new List<InvMaterialManagement>();

                        foreach (PurPurchaseReturnsDet line in tot.lines)
                        {

                             
                                mgts.Add(new InvMaterialManagement
                                {
                                    TransactionId = tot.header.RecordId,
                                    Sno = sno,
                                    Gin = line.BranchId,
                                    ItemName = line.ItemId,
                                    Dat = tot.header.Dat,
                                    BatchNo = line.Batchno,
                                    Manudate = line.Manudate,
                                    Expdate = line.Expdate,
                                    Store = store,
                                    Qtyin = 0,
                                    Qtyout = line.Qty * line.Mrp,
                                    Rat = line.Rat / line.Mrp,
                                    Descr = line.BranchId,
                                    TransactionType = 101,
                                    BranchId = tot.usr.bCode,
                                    CustomerCode = tot.usr.cCode
                                });
                             
                            line.RecordId = tot.header.RecordId;
                            line.Sno = sno;
                            line.BranchId = tot.usr.bCode;
                            line.CustomerCode = tot.usr.cCode;
                             sno++;
                        }
                        if (tot.lines.Count() > 0)
                        {
                            db.PurPurchaseReturnsDet.AddRange(tot.lines);
                            db.InvMaterialManagement.AddRange(mgts);
                        }
                        sno = 1;
                        if (tot.taxes != null)
                        {
                            foreach (PurPurchaseReturnTaxes tax in tot.taxes)
                            {
                                tax.RecordId = tot.header.RecordId;
                                tax.Sno = sno;
                                tax.BranchId = tot.usr.bCode;
                                tax.CustomerCode = tot.usr.cCode;
                                sno++;
                            }
                        }
                                if (tot.taxes != null)
                                {
                                    if (tot.taxes.Count() > 0)
                                        db.PurPurchaseReturnTaxes.AddRange(tot.taxes);
                                }

                        var totalamt = tot.header.TotalAmt;
                        if (totalamt > 0)
                        {
                            TotSalesSupportings support = new TotSalesSupportings();
                            support.BillNo = tot.header.RecordId;
                            support.Amt = totalamt;
                            support.BillType = "PRT";
                            support.AccName = tot.header.Vendorid;
                            support.SettleMode = "CREDIT";
                            support.Usrname = tot.usr.uCode;
                            support.SettleDate = tot.header.Dat.Value;
                            support.Branchid = tot.usr.bCode;
                            support.CustomerCode = tot.usr.cCode;
                            db.TotSalesSupportings.Add(support);
                        }
                                PartyTransactions trans = new PartyTransactions();
                                trans.TransactionId = tot.header.RecordId;
                                trans.TransactionType = "PRT";
                                trans.Dat = ac.getPresentDateTime();
                                trans.PartyId = tot.header.Vendorid;
                                trans.PartyName = tot.header.Vendorname;
                                trans.TransactionAmt = totalamt;
                                trans.PendingAmount = 0;
                                trans.ReturnAmount = totalamt;
                                trans.CreditNote = 0;
                                trans.PaymentAmount = 0;
                                trans.Username = tot.usr.uCode;
                                trans.BranchId = tot.usr.bCode;
                                trans.CustomerCode = tot.usr.cCode;
                                trans.OnTraId = tot.header.RefMir;

                             //   TransactionsAudit audit = new TransactionsAudit();

                        db.SaveChanges();
                        msg = "OK";
                        if (ac.transactionCheck("Accounts", tot.header.Dat.Value, tot.usr) == "OK")
                        {
                            makeAccounts(tot.header, tot.taxes,   tot.usr);
                        }

                    }
                    else
                    {
                        msg = "You are not authorised for this transaction";
                    }

                            break;
                        case 3:
                           

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

        [HttpPost]
        [Authorize]
        [Route("api/PurPurchaseReturn/DeletePurchaseReturn")]
        public TransactionResult DeletePurchaseReturn([FromBody] GeneralInformation inf)
        {
            string msg = "";
            if (ac.screenCheck(inf.usr, 2, 3, 6, 3))
            {
                try
                {

                    var support1 = db.TotSalesSupportings.Where(a => a.BillNo == inf.recordId && a.BillType == "PRT" && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    var support2 = db.PartyTransactions.Where(a => a.TransactionId == inf.recordId && a.TransactionType == "PRT" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if(support1 != null)
                    {
                        db.TotSalesSupportings.Remove(support1);
                    }
                    if(support2 != null)
                    {
                        db.PartyTransactions.Remove(support2);
                    }
                    var linesDel = db.PurPurchaseReturnsDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if (linesDel.Count() > 0)
                    {
                        db.PurPurchaseReturnsDet.RemoveRange(linesDel);
                    }
                    var invDel = db.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == 101 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if(invDel.Count() > 0)
                    {
                        db.InvMaterialManagement.RemoveRange(invDel);
                    }
                    var accountheaderdel = db.FinexecUni.Where(a => a.Tratype == "PUR_PRT" && a.Traref == inf.recordId.ToString() && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (accountheaderdel != null)
                    {
                        var accountslinesDel = db.FinexecDet.Where(a => a.RecordId == accountheaderdel.RecordId && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                        if (accountslinesDel.Count() > 0)
                            db.FinexecDet.RemoveRange(accountslinesDel);
                        db.FinexecUni.Remove(accountheaderdel);
                    }
                    var taxesDel = db.PurPurchaseReturnTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if (taxesDel.Count() > 0)
                    {
                        db.PurPurchaseReturnTaxes.RemoveRange(taxesDel);
                    }
                    var headerDel = db.PurPurchaseReturnsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (headerDel != null)
                    {
                        db.PurPurchaseReturnsUni.Remove(headerDel);
                    }
                    db.SaveChanges();
                    msg = "OK";
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
        private void makeAccounts(PurPurchaseReturnsUni header, List<PurPurchaseReturnTaxes> taxes, UserInfo usr)
        {
            UsineContext db1 = new UsineContext();
            UsineContext db2 = new UsineContext();
            UsineContext db3 = new UsineContext();
            int chk = 0;
            try
            {
               /* var headerupd = db2.FinexecUni.Where(a => a.Traref == header.RecordId.ToString() && a.Tratype == "PUR_PRT" && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).FirstOrDefault();
                if (headerupd != null)
                {
                    var linesupd = db3.FinexecDet.Where(a => a.RecordId == headerupd.RecordId && a.Branchid == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                    if (linesupd.Count > 0)
                    {
                        db3.FinexecDet.RemoveRange(linesupd);
                        db3.SaveChanges();
                    }
                    db2.FinexecUni.Remove(headerupd);
                    db2.SaveChanges();
                }*/

                var accounts = db1.AccountsAssign.Where(a => a.Module == "PUR" && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).ToList();
                List<FinexecDet> lines = new List<FinexecDet>();
                var account = accounts.Where(a => a.Transcode == "PUR_PRT").FirstOrDefault();

                if (account != null)
                {
                    lines.Add(new FinexecDet
                    {
                        Sno = 1,
                        Accname = account.Account,
                        Cre = header.Baseamt * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                        Deb =0,
                        Branchid = usr.bCode,
                        CustomerCode = usr.cCode,
                        Dat = header.Dat
                    });
                }
                else
                {
                    chk = 1;
                }

               

            
                int sno = 6;
               
                foreach (var tax in taxes)
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
                                Cre = tax.Taxvalue * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                                Deb = 0,
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


                if (header.TotalAmt != 0)
                {
                    var totalamt = header.TotalAmt;
                    account = accounts.Where(a => a.Transcode == "PUR_CRP").FirstOrDefault();
                    if (account != null)
                    {

                        lines.Add(new FinexecDet
                        {
                            Sno = 22,
                            Accname = account.Account,
                            Deb = (totalamt > 0 ? totalamt : 0) * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
                            Cre = (totalamt < 0 ? (double)(Math.Abs((decimal)totalamt)) : 0) * (header.CurrencyConversion == null ? 1 : header.CurrencyConversion),
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
                if (chk == 0)
                {
                    acheader.Seq = finAccSeq(usr, header.Dat.Value);
                    acheader.Dat = header.Dat;
                    acheader.Narr = "Purchase Return Detail of Seq " + header.Seq;
                    acheader.Tratype = "PUR_PRT";
                    acheader.Traref = header.RecordId.ToString();
                    acheader.Vouchertype = "PURCHASE RETURN";
                    acheader.Branchid = usr.bCode;
                    acheader.CustomerCode = usr.cCode;
                    acheader.Usr = usr.uCode;
                    db1.FinexecUni.Add(acheader);
                    db1.SaveChanges();

                    foreach (var lin in lines)
                    {
                        lin.RecordId = acheader.RecordId;
                    }
                    db1.FinexecDet.AddRange(lines);
                    db1.SaveChanges();

                }
            }
            catch (Exception ee)
            {

            }
        }
        private int? finAccSeq(UserInfo usr, DateTime dat)
        {
            UsineContext db = new UsineContext();
            AdminControl ac = new AdminControl();
            DateTime dat1 = dat;
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
