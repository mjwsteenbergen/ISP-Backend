using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IEPBackendCrawler;
using Microsoft.AspNetCore.Mvc;

namespace IEP_API.Controllers
{
    [Route("api/person")]
    public class PersonController : Controller
    {
        // GET api/values
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            DelftContext context = new DelftContext();
            return Json(context.Persons);
        }

        // GET api/values/5
        [HttpGet("{email}")]
        public async Task<JsonResult> GetAsync(string email)
        {
            DelftContext context = new DelftContext();
            return Json(context.Persons.FirstOrDefault(i => i.Email == email));
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
