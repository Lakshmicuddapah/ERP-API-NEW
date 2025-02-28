using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Usine_Core.Controllers.CRM;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.ModelsAdmin;
using Microsoft.EntityFrameworkCore;

namespace Usine_Core.Controllers.sales
{
    public class SalSalesTotal
    {
        public SalSalesUni header { get; set; }
        public List<SalSalesDet> lines { get; set; }
        public List<SalSalesTaxes> taxes { get; set; }
        public dynamic linedetails { get; set; }
        public int? traCheck { get; set; }
        public String result { get; set; }
        public UserInfo usr { get; set; }

    }
    public class MaterialDetails
    {
        public int? soid { get; set; }
        public int? sno { get; set; }
        public string sono { get; set; }
        public int? partyid { get; set; }
        public string partyname { get; set; }
        public int? itemid { get; set; }
        public string itemname { get; set; }
        public double? qty { get; set; }
        public string um { get; set; }
        public double? dispatched { get; set; }
        public double? pending { get; set; }
        public double? available { get; set; }
        public double? tobedispatched { get; set; }
        public double? rat { get; set; }
        public double? discper { get; set; }
    }
    public class SaleRequirements
    {
        public string seq { get; set; }
        public DateTime dat { get; set; }
        public List<InvStores> stores { get; set; }
        public List<MisCountryMaster> countries { get; set; }
        public List<Purpurchasetypes> saltypes { get; set; }
        public List<PartyDetails> customers { get; set; }
        public List<PartyAddresses> addresses { get; set; }
        public List<MaterialDetails> materials { get; set; }
        public dynamic units { get; set; }
        public List<CrmSaleOrderUni> orders { get; set; }
        public List<AdmTaxes> taxes { get; set; }
        public List<PurSetup> sets { get; set; }
    }
    public class SaleDespatchesController : ControllerBase
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();
        PrismProductsAdminContext dc = new PrismProductsAdminContext();

        private static Random random = new Random();

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/GetSaleRequirements")]
        public SaleRequirements GetSaleRequirements([FromBody] UserInfo usr)
        {
            SaleRequirements tot = new SaleRequirements();

            tot.seq = findSeq(usr);
            tot.dat = ac.getPresentDateTime();
            var dat1 = ac.getFinancialStart(tot.dat, usr);
            var dat2 = dat1.AddYears(1);
            tot.addresses = db.PartyAddresses.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ThenBy(c => c.Sno).ToList();
            tot.stores = db.InvStores.Where(a => a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
            tot.countries = db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.Cntname).ToList();
            tot.saltypes = db.Purpurchasetypes.Where(a => a.CustomerCode == usr.cCode).ToList();
            tot.customers = db.PartyDetails.Where(a => a.Statu == 1 && a.PartyType == "CUS" && a.CustomerCode == usr.cCode).OrderBy(b => b.PartyName).ToList();

            string quer = "";
            quer = quer + " select a.recordId,sno,seq,partyid,partyname,a.itemid,b.itemname,qty,b.um,dispatched,pending,";
            quer = quer + " available,case when pending > available then available else pending end dispatch,rat,discper from";
            quer = quer + " (select recordId, sno, seq, partyid, partyname, itemid, qty, dispatched, pending,";
            quer = quer + " case when available is null then 0 else available end available,rat,discper from";
            quer = quer + " (select recordId, sno, seq, partyid, partyname, itemid, qty, dispatched, (qty-dispatched) pending, um,rat,discper from";
            quer = quer + " (select a.recordId, a.sno, a.seq, a.partyid, a.partyname, a.itemid, a.qty, a.um, a.rat,a.discper,";
            quer = quer + " case when b.qty is null then 0 else b.qty end dispatched from";
            quer = quer + " (select a.recordId, a.sno, b.seq, b.partyid, b.partyname, a.itemid, a.qty, a.um, a.rat,discper from";
            quer = quer + " (select* from crmSaleOrderDet)a,";
            quer = quer + " (select * from crmSaleOrderUni where branchId = 'E001' and customerCode = " + usr.cCode + ")b";
            quer = quer + " where a.recordId = b.recordId)a left outer join";
            quer = quer + " (select* from salSalesdet where branchid= 'E001' and customerCode = " + usr.cCode + ")b";
            quer = quer + " on a.recordId = b.refsoid and a.sno = b.refsoline)x where qty > dispatched )a left outer join";
            quer = quer + " (select itemname, sum(qtyin-qtyout) available from invMaterialManagement where dat >= '1-Apr-24' and dat< '1-Apr-25'";
            quer = quer + " and branchId = 'E001' and customerCode = " + usr.cCode + " group by  itemname having sum(qtyin - qtyout) > 0)b on a.itemId = b.itemName)a,";
            quer = quer + " (select * from invMaterialCompleteDetails_view where customercode = " + usr.cCode + ")b where a.itemId = b.recordId";
            SqlCommand dc = new SqlCommand();
            DataBaseContext g = new DataBaseContext();
            General gg = new General();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            tot.materials = new List<MaterialDetails>();
            while (dr.Read())
            {
                tot.materials.Add(new MaterialDetails
                {
                    soid = gg.valInt(dr[0].ToString()),
                    sno = gg.valInt(dr[1].ToString()),
                    sono = dr[2].ToString(),
                    partyid = gg.valInt(dr[3].ToString()),
                    partyname = dr[4].ToString(),
                    itemid = gg.valInt(dr[5].ToString()),
                    itemname = dr[6].ToString(),
                    qty = gg.valNum(dr[7].ToString()),
                    um = dr[8].ToString(),
                    dispatched = gg.valNum(dr[9].ToString()),
                    pending = gg.valNum(dr[10].ToString()),
                    available = gg.valNum(dr[11].ToString()),
                    tobedispatched = gg.valNum(dr[12].ToString()),
                    rat = gg.valNum(dr[13].ToString()),
                    discper = gg.valNum(dr[14].ToString())

                });
            }
            dr.Close();
            g.db.Close();


            return tot;
        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/GetSales")]
        public List<SalSalesUni> GetPurchases([FromBody] GeneralInformation inf)
        {
            DateTime dat1 = DateTime.Parse(inf.frmDate);
            DateTime dat2 = DateTime.Parse(inf.frmDate).AddDays(1);
            return db.SalSalesUni.Where(a => a.Dat >= dat1 && a.Dat <= dat2 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).ToList();
        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/GetSale")]
        public SalSalesTotal GetSale([FromBody] GeneralInformation inf)
        {
            SalSalesTotal tot = new SalSalesTotal();
            string msg = "";
            try
            {
                UsineContext db1 = new UsineContext();

                tot.header = db.SalSalesUni.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();

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

                tot.taxes = db.SalSalesTaxes.Where(a => a.RecordId == inf.recordId && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).OrderBy(b => b.Sno).ToList();
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


        private static string passGen(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/SetSale")]
        public TransactionResult SetSale([FromBody] SalSalesTotal tot)
        {
            string msg = "";
            General gg = new General();
            AdminControl ac = new AdminControl();

            try
            {
                string tramsg = ac.transactionCheck("Inventory", tot.header.Dat, tot.usr);
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
                                        // Create sale header
                                        SalSalesUni header = new SalSalesUni
                                        {
                                            Seq = findSeq(tot.usr),
                                            Dat = tot.header.Dat,
                                            Usr = tot.usr.uCode,
                                            SaleType = tot.header.SaleType,
                                            PartyId = tot.header.PartyId,
                                            PartyName = tot.header.PartyName,
                                            Addr = tot.header.Addr,
                                            Country = tot.header.Country,
                                            Stat = tot.header.Stat,
                                            District = tot.header.District,
                                            City = tot.header.City,
                                            Zip = tot.header.Zip,
                                            Mobile = tot.header.Mobile,
                                            Tel = tot.header.Tel,
                                            Fax = tot.header.Fax,
                                            Email = tot.header.Email,
                                            Webid = tot.header.Webid,
                                            Baseamt = tot.header.Baseamt,
                                            Discount = tot.header.Discount,
                                            Taxes = tot.header.Taxes,
                                            Others = tot.header.Others,
                                            TotalAmt = tot.header.TotalAmt,
                                            Pos = 1,
                                            BranchId = tot.usr.bCode,
                                            RefSoid = tot.header.RefSoid,
                                            CustomerCode = tot.usr.cCode,
                                            PassCodeCheck = 1,
                                            PassCode = passGen(6),
                                            CurrencySymbol = tot.header.CurrencySymbol,
                                            CurrencyConversion = tot.header.CurrencyConversion
                                        };
                                        db.SalSalesUni.Add(header);
                                        db.SaveChanges();

                                        int sno = 1;
                                        List<InvMaterialManagement> mgts = new List<InvMaterialManagement>();
                                        string gin = tot.header.Dat.Value.Year.ToString().Substring(2, 2) + gg.zeroMake(tot.header.Dat.Value.Month, 2) + gg.zeroMake(tot.header.Dat.Value.Day, 2);

                                        // Process each sale line item
                                        foreach (SalSalesDet line in tot.lines)
                                        {
                                            var stockItems = db.InvMaterialManagement
                                            .Where(x => x.ItemName == line.ItemId
                                             && x.BranchId == tot.usr.bCode
                                             && x.CustomerCode == tot.usr.cCode
                                             && x.Qtyin > x.Qtyout) // Ensure stock is available
                                            .OrderBy(x => x.Dat) // Oldest stock first (FIFO)
                                            .ToList();


                                            if (stockItems == null || stockItems.Count == 0)
                                            {
                                                throw new Exception($"Item {line.ItemId} not found in inventory.");
                                            }

                                            int remainingQtyToDeduct = (int)line.Qty;

                                            foreach (var stockItem in stockItems)
                                            {
                                                int availableStock = (int)((stockItem.Qtyin ?? 0) - (stockItem.Qtyout ?? 0));

                                                if (availableStock <= 0) continue;

                                                if (remainingQtyToDeduct <= availableStock)
                                                {
                                                    stockItem.Qtyout += remainingQtyToDeduct;
                                                    db.Entry(stockItem).State = EntityState.Modified;
                                                    remainingQtyToDeduct = 0;
                                                    break;
                                                }
                                                else
                                                {
                                                    stockItem.Qtyout += availableStock;
                                                    db.Entry(stockItem).State = EntityState.Modified;
                                                    remainingQtyToDeduct -= availableStock;
                                                }
                                            }

                                            if (remainingQtyToDeduct > 0)
                                            {
                                                throw new Exception($"Insufficient stock for item {line.ItemName}. Short by {remainingQtyToDeduct} units.");
                                            }

                                            line.RecordId = header.RecordId;
                                            line.Sno = sno;
                                            line.BranchId = tot.usr.bCode;
                                            line.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }

                                        if (tot.lines.Count() > 0)
                                        {
                                            db.SalSalesDet.AddRange(tot.lines);
                                            db.InvMaterialManagement.AddRange(mgts);
                                        }

                                        sno = 1;
                                        foreach (SalSalesTaxes tax in tot.taxes)
                                        {
                                            tax.RecordId = header.RecordId;
                                            tax.Sno = sno;
                                            tax.BranchId = tot.usr.bCode;
                                            tax.CustomerCode = tot.usr.cCode;
                                            sno++;
                                        }
                                        db.SalSalesTaxes.AddRange(tot.taxes);


                                        double? advance = 0;

                                        var totalamt = header.TotalAmt - advance;
                                        if (totalamt > 0)
                                        {
                                            TotSalesSupportings support = new TotSalesSupportings();
                                            support.BillNo = header.RecordId;
                                            support.Amt = totalamt;
                                            support.BillType = "SAL";
                                            support.AccName = header.PartyId;
                                            support.SettleMode = "CREDIT";
                                            support.Usrname = tot.usr.uCode;
                                            support.SettleDate = header.Dat.Value;
                                            support.Branchid = tot.usr.bCode;
                                            support.CustomerCode = tot.usr.cCode;
                                            db.TotSalesSupportings.Add(support);

                                            PartyTransactions trans = new PartyTransactions();
                                            trans.TransactionId = header.RecordId;
                                            trans.TransactionType = "SAL";
                                            trans.Dat = header.Dat;
                                            trans.PartyId = header.PartyId;
                                            trans.PartyName = header.PartyName;
                                            trans.TransactionAmt = header.TotalAmt;
                                            trans.PendingAmount = totalamt;
                                            trans.ReturnAmount = 0;
                                            trans.CreditNote = 0;
                                            trans.PaymentAmount = 0;
                                            trans.Descriptio = "Sale of invoice " + header.Seq;
                                            trans.Username = tot.usr.uCode;
                                            trans.BranchId = tot.usr.bCode;
                                            trans.CustomerCode = tot.usr.cCode;
                                            db.PartyTransactions.Add(trans);

                                        }
                                        db.SaveChanges();
                                        if (ac.transactionCheck("Accounts", tot.header.Dat.Value, tot.usr) == "OK")
                                        {
                                            // makeAccounts(header, tot.taxes, advance, totalamt, tot.usr);
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
                            // Handle other cases if needed
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

            return new TransactionResult { result = msg };
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
                x = int.Parse(g.right(y, 4));

            }
            x++;
            return "MIR" + dat.Year.ToString().Substring(2, 2) + "-" + (dat.Month < 10 ? "0" : "") + dat.Month.ToString() + "/" + g.zeroMake(x, 4);
        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/GetPendingDispatches")]

        public List<SalSalesUni> GetPendingDispatches([FromBody] UserInfo usr)
        {
            return db.SalSalesUni.Where(a => a.PassCodeCheck == 1 && a.BranchId == usr.bCode && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();

        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/SetPendingDispatchClear")]
        public TransactionResult SetPendingDispatchClear([FromBody] GeneralInformation inf)
        {
            string msg = "";
            var det = db.SalSalesUni.Where(a => a.RecordId == inf.recordId && a.PassCodeCheck == 1 && a.BranchId == inf.usr.bCode && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
            if (det != null)
            {
                det.PassCodeCheck = -1;
                db.SaveChanges();
                msg = "OK";
            }
            else
            {
                msg = "No record found";
            }
            TransactionResult result = new TransactionResult();
            result.result = msg;
            return result;
        }

        [HttpPost]
        [Authorize]
        [Route("api/SaleDespatches/geneRatecode")]
        public IActionResult geneRatecode([FromBody] GeneralInformation inf)
        {
            var result = db.SalSalesUni.Where(x => x.RecordId == inf.recordId).FirstOrDefault();
            if (result != null)
            {
                result.PassCode = passGen(6);
                db.SalSalesUni.Update(result);
                db.SaveChanges();


                var clientDetails = dc.CustomerRegistrations.Where(x => x.CustomerId == inf.usr.cCode).FirstOrDefault();

                others.sendEmail sendEmail = new others.sendEmail();
                sendEmail.EmailSend("Pass Code", clientDetails.Email, "Dear Customer,\n\nPlease use the following Dispatch code once the Dispatch has been initiated. \n\nCode :" + result.PassCode + "\n\nUser:" + result.Usr + "\n\n" + "Invoice Code:" + result.Seq + "\n\nDo not share the Code with any one.\n\nThanks", null, "saikumar@cortracker360.com", "raviteja@cortracker360.com");

            }

            return Ok();

        }
    }
}
