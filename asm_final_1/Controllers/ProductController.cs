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
            var products = await context.Products.Where(p => p.IsDeleted == false).ToListAsync();

            var topProducts = products.FindAll(p => p.IsTop);
            var bestSellerProducts = products.FindAll(p => p.IsBestSeller);
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
            var cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");

            ViewBag.currentProduct = currentProduct;
            ViewBag.categories = categories;
            ViewBag.cart = cart;
            return View();
        }

        // Product detail - GET
        [Route("/filter/{categoryId}")]
        public async Task<IActionResult> FilteredProducts(int categoryId)
        {
            var filteredProducts = await context.Products.Where(p => p.CategoryId == categoryId && p.IsDeleted == false).ToListAsync();
            var categoryNames = await context.Categories.Where(c => c.Id == categoryId).Select(c => c.Name).ToListAsync();
            var cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
            var categories = await context.Categories.ToListAsync();

            if (filteredProducts != null && categoryNames != null)
            {
                ViewBag.filteredProducts = filteredProducts;
                ViewBag.categoryName = categoryNames[0];
                ViewBag.cart = cart;
                ViewBag.categories = categories;
            }

            return View();
        }

        // Product detail - GET
        [Route("/search")]
        [HttpGet]
        public async Task<IActionResult> SearchProduct(string keyword = "")
        {
            var products = await context.Products.ToListAsync();
            var cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
            var categories = await context.Categories.ToListAsync();
            //

            if (keyword != null && keyword != "")
            {
                products = products.Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToList();
            }
            else
            {
                products = context.Products.ToList();
            }

            ViewBag.cart = cart;
            ViewBag.categories = categories;
            ViewBag.products = products;
            ViewBag.keyword = keyword;

            return View();
        }

        // ------------------------------ADMIN----------------------

        // products - GET
        [Route("admin/manage-products")]
        [HttpGet]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> Products(string keyword = "", int page = 1)
        {
            var products = await context.Products.ToListAsync();
            //

            if (keyword != null && keyword != "")
            {
                products = products.Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToList();
            }
            else
            {
                products = context.Products.ToList();
            }

            //
            int pageSize = 5;

            if (page < 1) page = 1;

            int totalItems = products.Count();

            var pager = new Pager(totalItems, page, pageSize);

            int skip = (page - 1) * pageSize;

            var data = products.Skip(skip).Take(pager.PageSize).ToList();

            ViewBag.products = data;
            ViewBag.pager = pager;
            ViewBag.keyword = keyword;

            return View();
        }

        // Add product - GET
        [Route("admin/manage-products/add")]
        [HttpGet]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> AddProduct()
        {
            var categories = await context.Categories.ToListAsync();

            ViewBag.categories = categories;

            return View();
        }

        // Add product - POST
        [Route("admin/manage-products/add")]
        [HttpPost]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var existsName = await context.Products.SingleOrDefaultAsync(p => p.Name == product.Name);

            if (existsName != null)
            {
                TempData["add-product__alert"] = AlertExtensions.ShowAlert(Alerts.Danger, "Tên sản phẩm đã tồn tại");
            }

            else if (ModelState.IsValid)
            {
                string imgPath = FileExtension.UploadFile(product.ImageFile, webHostEnvironment);

                var newProduct = new Product
                {
                    Name = product.Name,
                    CategoryId = product.CategoryId,
                    Alias = product.Alias,
                    Brand = product.Brand,
                    ImportPrice = product.ImportPrice,
                    Price = product.Price,
                    HotPrice = product.HotPrice != null ? product.HotPrice : null,
                    Quantity = product.Quantity,
                    Description = product.Description,
                    Image = imgPath,
                    IsDeleted = product.IsDeleted,
                    IsTop = product.IsTop,
                    IsBestSeller = product.IsBestSeller,
                    IsFreeShip = product.IsFreeShip
                };

                await context.Products.AddAsync(newProduct);
                await context.SaveChangesAsync();

                TempData["add-product__alert"] = AlertExtensions.ShowAlert(Alerts.Success, "Thêm sản phẩm thành công");

                return RedirectToAction("AddProduct", "Product");
            }

            var categories = context.Categories.ToList();

            ViewBag.categories = categories;

            return View(product);
        }

        // Update product - GET
        [Route("admin/manage-products/update/{id}")]
        [HttpGet]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var currentProduct = await context.Products.FindAsync(id);
            var categories = await context.Categories.ToListAsync();
            var currentCategory = categories.Find(c => c.Id == currentProduct.CategoryId);

            ViewBag.categories = categories;
            ViewBag.currentProduct = currentProduct;
            ViewBag.currentCategory = currentCategory;

            return View();
        }

        // Update product - POST
        [Route("admin/manage-products/update/{id}")]
        [HttpPost]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var currentProduct = await context.Products.FindAsync(id);
            var existsName = await context.Products.SingleOrDefaultAsync(p => p.Name == product.Name);

            if (product.Name != currentProduct.Name && existsName != null)
            {
                TempData["update-product__alert"] = AlertExtensions.ShowAlert(Alerts.Danger, "Tên sản phẩm đã tồn tại");
            }
            else if (ModelState.IsValid && currentProduct != null)
            {
                string imgPath = product.ImageFile != null
                    ? FileExtension.UploadFile(product.ImageFile, webHostEnvironment)
                    : currentProduct.Image;

                currentProduct.Name = product.Name;
                currentProduct.CategoryId = product.CategoryId;
                currentProduct.Alias = product.Alias;
                currentProduct.Brand = product.Brand;
                currentProduct.ImportPrice = product.ImportPrice;
                currentProduct.Price = product.Price;
                currentProduct.HotPrice = product.HotPrice != null ? product.HotPrice : null;
                currentProduct.Quantity = product.Quantity;
                currentProduct.Description = product.Description;
                currentProduct.Image = imgPath;
                currentProduct.IsDeleted = product.IsDeleted;
                currentProduct.IsTop = product.IsTop;
                currentProduct.IsBestSeller = product.IsBestSeller;
                currentProduct.IsFreeShip = product.IsFreeShip;

                await context.SaveChangesAsync();

                TempData["update-product__alert"] = AlertExtensions.ShowAlert(Alerts.Success, "Sửa sản phẩm thành công");

                return RedirectToAction("updateProduct", "Product");
            }

            var categories = await context.Categories.ToListAsync();
            var currentCategory = categories.Find(c => c.Id == currentProduct.CategoryId);

            ViewBag.categories = categories;
            ViewBag.currentProduct = currentProduct;
            ViewBag.currentCategory = currentCategory;

            return View(product);
        }

        // Product - DELETE
        [Route("admin/manage-products/delete/${id}")]
        [Authorize(Policy = "DenyCustomer")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);
            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return RedirectToAction("products", "product");
        }
    }
}
