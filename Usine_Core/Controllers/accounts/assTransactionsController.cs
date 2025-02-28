using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Complete_Solutions_Core.Controllers.Accounts
{
    [Produces("application/json")]
    [Route("api/assTransactions")]
    public class assTransactionsController : Controller
    {
        // GET: api/assTransactions
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/assTransactions/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/assTransactions
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/assTransactions/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
