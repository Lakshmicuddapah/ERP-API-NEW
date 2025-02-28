using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Controllers.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Usine_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.CRM
{
    public class BlankCoatings
    {
        public int coatid { get; set; }
        public string coat { get; set; }
    }
    public class BlanksPriceList
    {
        public List<SaleRxPriceList> prices { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
    public class BlanksDiscountsList
    {
        public List<SaleRxDiscountList> discounts { get; set; }
        public UserInfo usr { get; set; }
        public string result { get; set; }
    }
    public class CRMRXSalePricesController : Controller
    {
        UsineContext db = new UsineContext();
        AdminControl ac = new AdminControl();


        [HttpPost("api/CRMRXSalePrices/GetRXPriceNames")]
        [Authorize]
        public List<string> GetRXPriceNames([FromBody] UserInfo usr)
        {
            return db.CrmPriceListUni.Where(a => a.CustomerCode == usr.cCode).Select(a => a.PriceListName).Distinct().ToList();
        }

      
        [HttpPost("api/CRMRXSalePrices/GetRXDiscountNames")]
        [Authorize]
        public List<string> GetRXDiscountNames([FromBody] UserInfo usr)
        {
            return db.CrmDiscountListUni.Where(a => a.CustomerCode == usr.cCode).Select(a => a.DiscountListName).Distinct().ToList();
        }

        [HttpPost("api/CRMRXSalePrices/SetRXSalePriceList")]
        [Authorize]
        public BlanksPriceList SetRXSalePriceList([FromBody]BlanksPriceList tot)
        {
            string msg = "";
            if (ac.screenCheck(tot.usr, 7, 1, 1, 0))
            {
                if (tot.prices.Count > 0)
                {
                    try
                    {
                        foreach (SaleRxPriceList price in tot.prices)
                        {
                            price.BranchId = tot.usr.bCode;
                            price.CustomerCode = tot.usr.cCode;
                        }
                        var prices = db.SaleRxPriceList.Where(a => a.PriceListName == tot.prices[0].PriceListName && a.CustomerCode == tot.usr.cCode).ToList();
                        if (prices.Count() > 0)
                            db.SaleRxPriceList.RemoveRange(prices);
                        db.SaleRxPriceList.AddRange(tot.prices);
                        db.SaveChanges();

                        msg = "OK";
                    }
                    catch (Exception ee)
                    {
                        msg = ee.Message;
                    }
                    tot.result = msg;
                    return tot;
                }
                else
                {
                    tot.result = "No records found";
                    return tot;
                }
            }
            else
            {
                tot.result = "You are not authorised to set prices";
                return tot;
            }

        }



        [HttpPost("api/CRMRXSalePrices/SetRXSaleDiscountList")]
        [Authorize]
        public BlanksDiscountsList SetRXSaleDiscountList([FromBody] BlanksDiscountsList tot)
        {
            string msg = "";
            if (ac.screenCheck(tot.usr, 7, 1, 2, 0))
            {

                if (tot.discounts.Count > 0)
                {
                    try
                    {
                        foreach (SaleRxDiscountList price in tot.discounts)
                        {
                            price.BranchId = tot.usr.bCode;
                            price.CustomerCode = tot.usr.cCode;
                        }
                        var prices = db.SaleRxDiscountList.Where(a => a.PriceListName == tot.discounts[0].PriceListName && a.CustomerCode == tot.usr.cCode).ToList();
                        db.SaleRxDiscountList.RemoveRange(prices);
                        db.SaleRxDiscountList.AddRange(tot.discounts);
                        db.SaveChanges();

                        msg = "OK";
                    }
                    catch (Exception ee)
                    {
                        msg = ee.Message;
                    }
                    tot.result = msg;
                    return tot;
                }
                else
                {
                    tot.result = "No records found";
                    return tot;
                }
            }
            else
            {
                tot.result = "You are not authorised to set discounts";
                return tot;
            }

        }

        [HttpPost("api/CRMRXSalePrices/GetRXSalePriceList")]
        [Authorize]
        public List<SaleRxPriceList> GetRXSalePriceList([FromBody]GeneralInformation inf)
        {
            List<SaleRxPriceList> lst = new List<SaleRxPriceList>();

            string quer = "";
            quer = quer + " select a.product,a.coat,case when b.price is null then a.price else b.price end price,a.sno from";
            quer = quer + " (select* from ";
            quer = quer + " (select product,'UC' coat,0 price,customerCode,1 sno from blaProducts union all";
            quer = quer + " select product,'HC' coat,0 price,customerCode,2 sno from blaProducts union all";
            quer = quer + " select product,'OHC' coat,0 price,customerCode,4 sno from blaProducts union all";
            quer = quer + " select product,'HMC' coat,0 price,customerCode,3 sno from blaProducts )x where customerCode = " + inf.usr.cCode + ")a left outer join";
            quer = quer + " (select* from  SaleRxPriceList  where pricelistname='" + inf.detail + "' and customerCode=" + inf.usr.cCode + ")b on a.product = b.product and a.coat = b.coat order by a.product ,a.sno";

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            General gg = new General();
            while(dr.Read())
            {
                lst.Add(new SaleRxPriceList
                {
                    Product=dr[0].ToString(),
                    Coat=dr[1].ToString(),
                    Price=gg.valNum(dr[2].ToString()),
                    Taxtype="TAX12",
                    

                });
            }
            dr.Close();
            g.db.Close();
            return lst;
        }


        [HttpPost("api/CRMRXSalePrices/GetRXSaleDiscountList")]
        [Authorize]
        public List<SaleRxDiscountList> GetRXSaleDiscountList([FromBody] GeneralInformation inf)
        {

            List<SaleRxDiscountList> lst = new List<SaleRxDiscountList>();

            string quer = "";
            quer = quer + " select a.product,a.coat,case when b.discount is null then a.price else b.discount end price,a.sno from";
            quer = quer + " (select* from ";
            quer = quer + " (select product,'UC' coat,0 price,customerCode,1 sno from blaProducts union all";
            quer = quer + " select product,'HC' coat,0 price,customerCode,2 sno from blaProducts union all";
            quer = quer + " select product,'OHC' coat,0 price,customerCode,4 sno from blaProducts union all";
            quer = quer + " select product,'HMC' coat,0 price,customerCode,3 sno from blaProducts )x where customerCode = " + inf.usr.cCode + ")a left outer join";
            quer = quer + " (select * from  SaleRxDiscountList  where pricelistname='" + inf.detail + "' and customerCode=" + inf.usr.cCode + ")b on a.product = b.product and a.coat = b.coat order by a.product ,a.sno";

            DataBaseContext g = new DataBaseContext();
            SqlCommand dc = new SqlCommand();
            dc.Connection = g.db;
            g.db.Open();
            dc.CommandText = quer;
            SqlDataReader dr = dc.ExecuteReader();
            General gg = new General();
            while (dr.Read())
            {
                lst.Add(new SaleRxDiscountList
                {
                    Product = dr[0].ToString(),
                    Coat = dr[1].ToString(),
                    Discount = gg.valNum(dr[2].ToString()),

                });
            }
            dr.Close();
            g.db.Close();
            return lst;

        }
    }
}
