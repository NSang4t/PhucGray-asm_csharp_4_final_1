using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Controllers
{
    public class AccountController : Controller
    {
        [Route("sign-in")]
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [Route("sign-up")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
    }
}
