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

namespace Usine_Core.Controllers.crm
{
    public class SalSaleReturnTotal
    {
        public SalSaleReturnsUni header { get; set; }
        public List<SalSaleReturnsDet> lines { get; set; }
        public List<SalSaleReturnTaxes> taxes { get; set; }
        public dynamic linedetails { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class SaleReturnRequirements
    {
        public string seq { get; set; }
        public DateTime dat { get; set; }
        public List<AdmTaxes> taxes { get; set; }
    }
    public class SaleInformation
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
    public class SaleInformationTotal
    {
        public SalSalesUni header { get; set; }
        public List<SaleInformation> lines { get; set; }
        public double? prevSaleReturn { get; set; }
        public double? prevDebitNotes { get; set; }
    }
    public class CRMSaleReturnsController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();

        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/GetSaleReturnRequirements")]
        public SaleReturnRequirements GetSaleReturnRequirements([FromBody] UserInfo usr)
        {
            SaleReturnRequirements tot = new SaleReturnRequirements();

            tot.seq = findSeq(usr);
            tot.dat = ac.getPresentDateTime();
            tot.taxes = db.AdmTaxes.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.TaxCode).ThenBy(c => c.TaxPer).ToList();
            return tot;
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/GetSaleReturns")]
        public List<SalSaleReturnsUni> GetSaleReturns([FromBody] GeneralInformation inf)
        {
            try
            {
                DateTime dat1 = DateTime.Parse(inf.frmDate);
                DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);
                var list1 = db.SalSaleReturnsUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                var list2 = db.SalSalesUni.Where(a => a.CustomerCode == inf.usr.cCode).ToList();
                var detail = (from a in list1
                              join b in list2 on a.RefInvoice equals b.RecordId
                              select new SalSaleReturnsUni
                              {
                                  RecordId = a.RecordId,
                                  Seq = a.Seq,
                                  BranchId = b.Seq,
                                  PartyName = a.PartyName,
                                  Mobile = a.Mobile,
                                  Baseamt = a.Baseamt,
                                  Discount = a.Discount,
                                  Others = a.Others,
                                  TotalAmt = a.TotalAmt,
                                  Pos=a.Pos
                              }).OrderBy(b => b.Dat).ThenBy(c => c.RecordId).ToList();
                return detail;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/GetSaleReturn")]
        public SalSaleReturnTotal GetSaleReturn([FromBody] GeneralInformation inf)
        {
            SalSaleReturnTotal tot = new SalSaleReturnTotal();
            string msg = "";
            try
            {
                UsineContext db1 = new UsineContext();
                List<string> gins = db1.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == 105 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Select(b => b.Gin).ToList();
                var det = db1.InvMaterialManagement.Where(a => gins.Contains(a.Gin) && a.TransactionType > 100 && a.Qtyout > 0 && a.CustomerCode == inf.usr.cCode);


                tot.header = (from a in db.SalSaleReturnsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
                              select new SalSaleReturnsUni
                              {
                                  RecordId = a.RecordId,
                                  Seq = a.Seq,
                                  PartyId = a.PartyId,
                                  PartyName = a.PartyName,
                                  Addr = a.Addr,
                                  Country = a.Country,
                                  Stat = a.Stat,
                                  District = a.District,
                                  City = a.City,
                                  Zip = a.Zip,
                                  Mobile = a.Mobile,
                                  Tel = a.Tel,
                                  Fax = a.Fax,
                                  Email = a.Email,
                                  Webid = a.Webid,
                                  Baseamt = a.Baseamt,
                                  Discount = a.Discount,
                                  Taxes = a.Taxes,
                                  Others = a.Others,
                                  TotalAmt = a.TotalAmt
                              }).FirstOrDefault();

                tot.linedetails = (from a in db.SalSalesDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode)
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
            var xx = db.SalSaleReturnsUni.Where(a => a.Dat.Value.Month == dat.Month && a.Dat.Value.Year == dat.Year && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).Max(b => b.Seq);
            if (xx != null)
            {
                x = g.valInt(g.right(xx, 4));
            }
            x++;
            return "SRT" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/" + g.zeroMake(x, 4);
        }
        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/GetSales")]
        public List<SalSalesUni> GetSales([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.toDate).AddDays(1);
            return db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/GetSale")]
        public SaleInformationTotal GetSale([FromBody] GeneralInformation inf)
        {
            SaleInformationTotal tot = new SaleInformationTotal();
            tot.header = db.SalSalesUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            tot.lines = new List<SaleInformation>();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            string quer = "";

         /*   quer = quer + " select a.recordId,a.sno,itemid,a.gin,itemname,batchno,case when manudate is null then '' else dbo.strdate(manudate) end manudate,case when expdate is null then '' else dbo.strdate(expdate) end expdate, qty, qtyout, available, a.rat,umid,um,conversionfactor from";
            quer = quer + " (select recordId, sno, itemid, a.gin, itemname, batchno, manudate, expdate, qty, a.rat, (case when qtyout is null then 0 else qtyout end)/ conversionfactor qtyout,qty - ((case when qtyout is null then 0 else qtyout end)/ conversionfactor) available,umid,conversionfactor from";
            quer = quer + " (select a.recordId, a.sno, a.itemid, b.gin, a.itemname, a.batchno, a.manudate, a.expdate, a.qty, umid, conversionFactor, a.rat from";
            quer = quer + " (select a.recordId, a.sno, a.itemid, a.itemname, a.batchno, a.manudate, a.expdate, a.qty, a.um umid, b.conversionFactor, a.rat from";
            quer = quer + " (select* from salsalesdet where recordId = " + inf.recordId + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from invMaterialUnits where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId and a.um = b.um )a,";
            quer = quer + " (select transactionId, sno, gin from invMaterialManagement where branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")b where a.recordId = b.transactionId and a.sno = b.sno)a left outer join";
            quer = quer + " (select gin, sum(qtyout) qtyout from invMaterialManagement where gin in";
            quer = quer + " (select gin from invMaterialmanagement where transactionId= " + inf.recordId + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")";
            quer = quer + " and qtyout > 0 and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + " group by gin)b on a.gin = b.gin)a,";
            quer = quer + " (select * from invUM where customercode = " + inf.usr.cCode + ")b where a.umid = b.recordid";
         */
            quer = quer + " select a.recordId,a.itemId,store,b.itemname,a.batchno,case when manudate is null then '' else dbo.strdate(manudate) end manudate,";
            quer = quer + " case when expdate is null then '' else dbo.strdate(expdate) end expdate, qty, prevreturns, qtyavailable, rat, b.um from";
            quer = quer + " (select recordId, itemid, store, itemname, batchno, manudate, expdate, qty,0 prevreturns,qty - 0 qtyavailable,rat from salsalesdet where recordId = " + inf.recordId + " and branchid = '" + inf.usr.bCode + "' and customerCode = " + inf.usr.cCode + ")a,";
            quer = quer + " (select * from invMaterialCompleteDetails_view where customerCode = " + inf.usr.cCode + ")b where a.itemid = b.recordId  ";


            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            while (dr.Read())
            {
                tot.lines.Add(new SaleInformation
                {
                    recordId = gg.valInt(dr[0].ToString()),
                //    sno = gg.valInt(dr[1].ToString()),
                    itemid = gg.valInt(dr[1].ToString()),
                  //  gin = dr[3].ToString(),
                    itemname = dr[3].ToString(),
                    batchno = dr[4].ToString(),
                    manudate = dr[5].ToString(),
                    expdate = dr[6].ToString(),
                    qty = gg.valNum(dr[7].ToString()),
                    qtyout = gg.valNum(dr[8].ToString()),
                    available = gg.valNum(dr[9].ToString()),
                    rat = gg.valNum(dr[10].ToString()),
                      um = dr[11].ToString(),
                    conversion = 1
                });
            }
            dr.Close();
            g.db.Close();

            tot.prevSaleReturn = db.PartyTransactions.Where(a => a.OnTraId == inf.recordId && a.TransactionType == "SRT" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Sum(b => b.ReturnAmount);
            tot.prevDebitNotes = db.PartyTransactions.Where(a => a.OnTraId == inf.recordId && a.TransactionType == "SDR" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).Sum(b => b.CreditNote);


            return tot;
        }


        [HttpPost]
        [Authorize]
        [Route("api/CRMSaleReturns/SetSaleReturn")]
        public TransactionResult SetSaleReturn([FromBody] SalSaleReturnTotal tot)
        {
            String msg = "";
            General gg = new General();
            int? store = db.SalSalesDet.Where(a => a.RecordId == tot.header.RefInvoice && a.BranchId == tot.usr.bCode && a.CustomerCode == tot.usr.cCode).Select(b => b.Store).FirstOrDefault();
            AdminControl ac = new AdminControl();
            try
            {

                String tramsg = ac.transactionCheck("Inventory", tot.header.Dat, tot.usr);
                if (tramsg == "OK")
                {
                    switch (tot.traCheck)
                    {
                        case 1:

                            if (ac.screenCheck(tot.usr, 2, 3, 6, 1))
                            {
                                tot.header.Seq = findSeq(tot.usr);
                                tot.header.Dat = ac.getPresentDateTime();
                                tot.header.Pos = 1;
                                tot.header.BranchId = tot.usr.bCode;
                                tot.header.CustomerCode = tot.usr.cCode;
                                db.SalSaleReturnsUni.Add(tot.header);
                                db.SaveChanges();
                                int sno = 1;
                                List<InvMaterialManagement> mgts = new List<InvMaterialManagement>();

                                foreach (SalSaleReturnsDet line in tot.lines)
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
                                        Qtyin = line.Qty * line.Mrp,
                                        Qtyout = 0,
                                        Rat = line.Rat / line.Mrp,
                                        Descr = line.BranchId,
                                        TransactionType = 5,
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
                                    db.SalSaleReturnsDet.AddRange(tot.lines);
                                    db.InvMaterialManagement.AddRange(mgts);
                                }
                                sno = 1;
                                if (tot.taxes != null)
                                {
                                    foreach (SalSaleReturnTaxes tax in tot.taxes)
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
                                        db.SalSaleReturnTaxes.AddRange(tot.taxes);
                                }

                                var totalamt = tot.header.TotalAmt;
                                if (totalamt > 0)
                                {
                                    TotSalesSupportings support = new TotSalesSupportings();
                                    support.BillNo = tot.header.RecordId;
                                    support.Amt = totalamt;
                                    support.BillType = "SRT";
                                    support.AccName = tot.header.PartyId;
                                    support.SettleMode = "CREDIT";
                                    support.Usrname = tot.usr.uCode;
                                    support.SettleDate = tot.header.Dat.Value;
                                    support.Branchid = tot.usr.bCode;
                                    support.CustomerCode = tot.usr.cCode;
                                    db.TotSalesSupportings.Add(support);
                                }
                                PartyTransactions trans = new PartyTransactions();
                                trans.TransactionId = tot.header.RecordId;
                                trans.TransactionType = "SRT";
                                trans.Dat = ac.getPresentDateTime();
                                trans.PartyId = tot.header.PartyId;
                                trans.PartyName = tot.header.PartyName;
                                trans.TransactionAmt = totalamt;
                                trans.PendingAmount = 0;
                                trans.ReturnAmount = totalamt;
                                trans.CreditNote = 0;
                                trans.PaymentAmount = 0;
                                trans.Username = tot.usr.uCode;
                                trans.BranchId = tot.usr.bCode;
                                trans.CustomerCode = tot.usr.cCode;
                                trans.OnTraId = tot.header.RefInvoice;

                                //   TransactionsAudit audit = new TransactionsAudit();

                                db.SaveChanges();
                                msg = "OK";
                                if (ac.transactionCheck("Accounts", tot.header.Dat.Value, tot.usr) == "OK")
                                {
                               //     makeAccounts(tot.header, tot.taxes, tot.usr);
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
        [Route("api/CRMSaleReturns/DeleteSaleReturn")]
        public TransactionResult DeleteSaleReturn([FromBody] GeneralInformation inf)
        {
            string msg = "";
            if (ac.screenCheck(inf.usr, 2, 3, 6, 3))
            {
                try
                {

                    var support1 = db.TotSalesSupportings.Where(a => a.BillNo == inf.recordId && a.BillType == "SRT" && a.Branchid == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    var support2 = db.PartyTransactions.Where(a => a.TransactionId == inf.recordId && a.TransactionType == "SRT" && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (support1 != null)
                    {
                        db.TotSalesSupportings.Remove(support1);
                    }
                    if (support2 != null)
                    {
                        db.PartyTransactions.Remove(support2);
                    }
                    var linesDel = db.SalSaleReturnsDet.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if (linesDel.Count() > 0)
                    {
                        db.SalSaleReturnsDet.RemoveRange(linesDel);
                    }
                    var invDel = db.InvMaterialManagement.Where(a => a.TransactionId == inf.recordId && a.TransactionType == 5 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if (invDel.Count() > 0)
                    {
                        db.InvMaterialManagement.RemoveRange(invDel);
                    }
                   
                    var taxesDel = db.SalSaleReturnTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
                    if (taxesDel.Count() > 0)
                    {
                        db.SalSaleReturnTaxes.RemoveRange(taxesDel);
                    }
                    var headerDel = db.SalSaleReturnsUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (headerDel != null)
                    {
                        db.SalSaleReturnsUni.Remove(headerDel);
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
