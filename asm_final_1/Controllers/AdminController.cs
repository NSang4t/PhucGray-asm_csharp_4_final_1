using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Controllers
{
    public class AdminController : Controller
    {
        [Route("admin")]
        [Authorize(Policy = "DenyCustomer")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
