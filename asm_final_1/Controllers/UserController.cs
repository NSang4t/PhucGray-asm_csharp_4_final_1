using asm_final_1.Models;
using asm_final_1.Models.OthersModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Controllers
{
    public class UserController : Controller
    {
        private readonly AsmContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public UserController(AsmContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        // ------------------------------ADMIN----------------------

        // products - GET
        [Route("admin/manage-users")]
        [HttpGet]

        public async Task<IActionResult> Users(string SearchText = "", int page = 1)
        {
            List<User> users = await context.Users.ToListAsync();
            //

            if (SearchText != null && SearchText != "")
            {
                users = users.Where(p => p.FirstName.ToLower().Contains(SearchText.ToLower())).ToList();
            }
            else
            {
                users = context.Users.ToList();
            }

            //
            int pageSize = 5;

            if (page < 1) page = 1;

            int totalItems = users.Count();

            var pager = new Pager(totalItems, page, pageSize);

            int skip = (page - 1) * pageSize;

            var data = users.Skip(skip).Take(pager.PageSize).ToList();

            ViewBag.users = data;
            ViewBag.pager = pager;

            //
            var roles = await context.Roles.ToListAsync();
            ViewBag.roles = roles;

            return View();
        }
    }
}
