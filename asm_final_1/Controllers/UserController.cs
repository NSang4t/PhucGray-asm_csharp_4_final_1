using asm_final_1.Models;
using asm_final_1.Models.OthersModels;
using asm_final_1.Utils;
using asm_rewrite.Enums;
using Microsoft.AspNetCore.Authorization;
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

        // USERS - GET
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

        // ADD USER - GET
        [Route("admin/manage-users/add")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddUser()
        {
            var roles = await context.Roles.ToListAsync();

            ViewBag.roles = roles;

            return View();
        }

        // ADD USER - POST
        [Route("admin/manage-users/add")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUser(User user)
        {
            var existsPhone = await context.Users.SingleOrDefaultAsync(u => u.Phone == user.Phone);
            var existsEmail = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

            if (existsEmail != null)
            {
                TempData["add-user__alert--danger"] = 
                    AlertExtensions.ShowAlert(Alerts.Danger, "Email đã tồn tại");
            }
            else if (existsPhone != null)
            {
                TempData["add-user__alert--danger"] = 
                    AlertExtensions.ShowAlert(Alerts.Danger, "Số điện thoại đã tồn tại");
            }
            else if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    LastName = user.LastName ?? null,
                    FirstName = user.FirstName,
                    Phone = user.Phone,
                    Email = user.Email,
                    Password = user.Password,
                    RoleId = user.RoleId,
                    Address = user.Address ?? null,
                    Image = null
                };

                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();

                TempData["add-user__alert--success"] 
                    = AlertExtensions.ShowAlert(Alerts.Success, "Thêm nhân viên thành công");

                return RedirectToAction("addUser", "user");
            }

            return View(user);
        }

        // UPDATE USER - GET
        [Route("admin/manage-users/update/{id}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var currentUser = await context.Users.FindAsync(id);
            var roles = await context.Roles.ToListAsync();

            ViewBag.currentUser = currentUser;
            ViewBag.roles = roles;

            return View();
        }

        // UPDATE USER - POST
        [Route("admin/manage-users/update/{id}")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            var currentUser = await context.Users.FindAsync(id);
            var existsEmail = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);
            var existsPhone = await context.Users.SingleOrDefaultAsync(u => u.Phone == user.Phone);

            if (user.Email != currentUser.Email && existsEmail != null)
            {
                TempData["update-user__alert--danger"] = AlertExtensions.ShowAlert(Alerts.Danger, "Email đã tồn tại");
            }
            else if (user.Phone != currentUser.Phone && existsPhone != null)
            {
                TempData["update-user__alert--danger"] = AlertExtensions.ShowAlert(Alerts.Danger, "Số điện thoại đã tồn tại");
            }
            else if (ModelState.IsValid && currentUser != null)
            {
                currentUser.LastName = user.LastName;
                currentUser.FirstName = user.FirstName;
                currentUser.Email = user.Email;
                currentUser.Password = user.Password;
                currentUser.Address = user.Address;
                currentUser.Phone = user.Phone;
                currentUser.RoleId = user.RoleId;
                currentUser.IsDeleted = user.IsDeleted;

                await context.SaveChangesAsync();

                TempData["update-user__alert--success"] = AlertExtensions.ShowAlert(Alerts.Success, "Sửa sản phẩm thành công");

                return RedirectToAction("updateUser", "user");
            }

            var roles = await context.Roles.ToListAsync();

            ViewBag.currentUser = currentUser;
            ViewBag.roles = roles;

            return View(user);
        }

        // USER - DELETE
        [Route("admin/manage-users/delete/${id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await context.Users.FindAsync(id);
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return RedirectToAction("users", "user");
        }
    }
}
