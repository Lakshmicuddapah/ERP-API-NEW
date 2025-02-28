using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usine_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Usine_Core.Controllers.Admin
{
    public class countryDetailTotal
    {
        public MisCountryMaster country { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class StateDetailTotal
    {
        public MisStateMaster stat { get; set; }
        public int? traCheck { get; set; }
        public string result { get; set; }
        public UserInfo usr { get; set; }
    }
    public class CountriesController : ControllerBase
    {
        UsineContext db = new UsineContext();

        [HttpPost]
        [Route("api/countries/GetTotalCountries")]
        public List<MisCountryMaster> GetTotalCountries([FromBody] UserInfo usr)
        {
            return db.MisCountryMaster.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }
        [HttpPost]
        [Route("api/countries/GetActiveCountries")]
        public List<MisCountryMaster> GetActiveCountries([FromBody] UserInfo usr)
        {
            return db.MisCountryMaster.Where(a => a.Statu > 0 && a.CustomerCode == usr.cCode).OrderBy(b => b.RecordId).ToList();
        }

        [HttpPost]
        [Route("api/countries/SetCountry")]
        [Authorize]
        public countryDetailTotal SetCountry([FromBody] countryDetailTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();

            try
            {
                switch (tot.traCheck)
                {
                    case 1:
                        if (dupCheck(tot))
                        {
                            if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
                            {
                                MisCountryMaster cnt = new MisCountryMaster();
                                cnt.Cntname = tot.country.Cntname;
                                cnt.Curr = tot.country.Curr;
                                cnt.CurrSymbol = tot.country.CurrSymbol;
                                cnt.ConversionFactor = tot.country.ConversionFactor;
                                cnt.Statu = tot.country.Statu;
                                cnt.Coins = tot.country.Coins;
                                cnt.CustomerCode = tot.usr.cCode;
                                db.MisCountryMaster.Add(cnt);
                                db.SaveChanges();
                                msg = "OK";
                            }
                            else
                            {
                                msg = "You are not authorised to add countries";
                            }

                        }
                        else
                        {
                            msg = "This country name is already existed";
                        }
                        break;
                    case 2:
                        if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
                        {
                            var cnt = db.MisCountryMaster.Where(a => a.RecordId == tot.country.RecordId && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
                            cnt.Cntname = tot.country.Cntname;
                            cnt.Curr = tot.country.Curr;
                            cnt.CurrSymbol = tot.country.CurrSymbol;
                            cnt.ConversionFactor = tot.country.ConversionFactor;
                            cnt.Coins = tot.country.Coins;
                            cnt.Statu = tot.country.Statu;
                            db.SaveChanges();
                            msg = "OK";
                        }
                        else
                        {
                            msg = "You are not authorised to modify countries";
                        }
                        break;
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }

            tot.result = msg;
            return tot;
        }
        private Boolean dupCheck(countryDetailTotal tot)
        {
            Boolean b = false;
            var x = db.MisCountryMaster.Where(a => a.Cntname == tot.country.Cntname && a.CustomerCode == tot.usr.cCode).FirstOrDefault();
            if (x == null)
            {
                b = true;
            }
            return b;
        }

        [HttpPost]
        [Route("api/countries/GetTotalStates")]
        public List<MisStateMaster> GetTotalStates([FromBody] UserInfo usr)
        {
            return db.MisStateMaster.Where(a => a.CustomerCode == usr.cCode).OrderBy(b => b.StateName).ToList();
        }
        [HttpPost]
        [Route("api/countries/SetState")]
        [Authorize]
        public StateDetailTotal SetState([FromBody] StateDetailTotal tot)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(tot.usr, -1, -1, -1, -1))
                {
                    MisStateMaster sta = new MisStateMaster();
                    sta.Cntname = tot.stat.Cntname;
                    sta.StateName = tot.stat.StateName;
                    sta.CustomerCode = tot.usr.cCode;
                    db.MisStateMaster.Add(sta);
                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for state transactions";
                }
            }
            catch
            {

            }
            tot.result = msg;
            return tot;
        }
        [HttpPost]
        [Route("api/countries/UpdateState")]
        [Authorize]
        public TransactionResult UpdateState([FromBody] GeneralInformation inf)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(inf.usr, -1, -1, -1, -1))
                {
                    MisStateMaster sta = new MisStateMaster();
                    sta = db.MisStateMaster.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (sta != null)
                    {
                        sta.StateName = inf.detail;
                    }

                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult tot = new TransactionResult();
            tot.result = msg;
            return tot;
    }
        [Route("api/countries/DeleteState")]
        [Authorize]
        public TransactionResult DeleteState([FromBody] GeneralInformation inf)
        {
            String msg = "";
            AdminControl ac = new AdminControl();
            try
            {
                if (ac.screenCheck(inf.usr, -1, -1, -1, -1))
                {
                    MisStateMaster sta = new MisStateMaster();
                    sta = db.MisStateMaster.Where(a => a.RecordId == inf.recordId && a.CustomerCode == inf.usr.cCode).FirstOrDefault();
                    if (sta != null)
                    {
                        db.MisStateMaster.Remove(sta);
                    }

                    db.SaveChanges();
                    msg = "OK";
                }
                else
                {
                    msg = "You are not authorised for this transaction";
                }
            }
            catch (Exception ee)
            {
                msg = ee.Message;
            }
            TransactionResult tot = new TransactionResult();
            tot.result = msg;
            return tot;
    }
    }
}

