using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asm_final_1.Controllers
{
    public class ProductController : Controller
    {
        // Home page
        [Route("/")]
        public IActionResult Home()
        {
            return View();
        }

        // Product detail
        [Route("/{alias}")]
        public async Task<IActionResult> ProductDetail(string alias)
        {
            return View();
        }

        // ---------------------------------ADMIN
        // products - GET
        [Route("admin/manage-products")]
        [HttpGet]
        public async Task<IActionResult> Products(string SearchText = "", int page = 1)
        {
            return View();
        }

        // Add product - GET
        [Route("admin/manage-products/add")]
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            return View();
        }

        // Update product - GET
        [Route("admin/manage-products/update/{id}")]
        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            return View();
        }

        // Product - DELETE
        [Route("admin/manage-products/delete/${id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return RedirectToAction("products", "product");
        }
    }
}
