using asm_final_1.Models;
using asm_final_1.Models.OthersModels;
using asm_final_1.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Controllers
{
    public class ProductController : Controller
    {
        private readonly AsmContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(AsmContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        // Home page
        [Route("/")]
        public async Task<IActionResult> Home()
        {
            var topProducts = await context.Products.Where(p => p.IsTop).ToListAsync();
            var bestSellerProducts = await context.Products.Where(p => p.IsBestSeller).ToListAsync();
            var categories = await context.Categories.ToListAsync();
            var cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");

            ViewBag.topProducts = topProducts;
            ViewBag.bestSellerProducts = bestSellerProducts;
            ViewBag.categories = categories;
            ViewBag.cart = cart;

            return View();
        }

        // Product detail - GET
        [Route("/{alias}")]
        public async Task<IActionResult> ProductDetail(string alias)
        {
            var currentProduct = await context.Products.SingleAsync(p => p.Alias == alias);
            var categories = await context.Categories.ToListAsync();

            ViewBag.currentProduct = currentProduct;
            ViewBag.categories = categories;

            return View();
        }
    }
}
