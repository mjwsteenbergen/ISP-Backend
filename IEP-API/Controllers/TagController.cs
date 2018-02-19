using IEPBackendCrawler;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEPAPI.Controllers
{
    [Route("api/tags")]
    public class TagController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            DelftContext context = new DelftContext();

            return Json(context.Tags);
        }
    }
}
