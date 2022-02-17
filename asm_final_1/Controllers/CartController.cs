using asm_final_1.Models;
using asm_final_1.Models.OthersModels;
using asm_final_1.Utils;
using asm_rewrite.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace asm_final_1.Controllers
{
    public class CartController : Controller
    {
        private readonly AsmContext context;
        public CartController(AsmContext context)
        {
            this.context = context;
        }

        [Route("cart")]
        public IActionResult Index()
        {
            var categories = context.Categories.ToList();
            ViewBag.categories = categories;

            var cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.total = cart != null ? cart.Sum(item => item.Product.HotPrice != null ? item.Product.HotPrice * item.Quantity : item.Product.Price * item.Quantity) : 0;

            return View();
        }

        private int CheckIsProductExists(int id)
        {
            List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");

            if (cart == null) return -1;

            for (int i = 0; i < cart.Count(); i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        [Route("addToCart/{id}")]
        public async Task<IActionResult> AddToCart(int id)
        {
            var currentProduct = await context.Products.SingleAsync(p => p.Id == id);

            if (CustomSessionExtensions.GetSessionData<List<Product>>(HttpContext.Session, "cart") == null)
            {
                var newItem = new Item { Product = context.Products.Single(p => p.Id == id), Quantity = 1 };

                List<Item> cart = new List<Item>();
                cart.Add(newItem);
                CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
                int index = CheckIsProductExists(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    var newItem = new Item { Product = context.Products.Single(p => p.Id == id), Quantity = 1 };
                    cart.Add(newItem);
                }
                CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);
            }

            TempData["add-to-cart__alert"] = AlertExtensions.ShowAlert(Alerts.Success, "Đã thêm sản phẩm vào giỏ hàng");

            return RedirectToAction("ProductDetail", "Product", new { name = currentProduct.Name });
        }

        [Route("buy/{id}")]
        public IActionResult Buy(int id)
        {
            if (CustomSessionExtensions.GetSessionData<List<Product>>(HttpContext.Session, "cart") == null)
            {
                var newItem = new Item { Product = context.Products.Single(p => p.Id == id), Quantity = 1 };

                List<Item> cart = new List<Item>();
                cart.Add(newItem);
                CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
                int index = CheckIsProductExists(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    var newItem = new Item { Product = context.Products.Single(p => p.Id == id), Quantity = 1 };
                    cart.Add(newItem);
                }
                CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");
            
            if(cart != null)
            {
                int index = CheckIsProductExists(id);
                cart.RemoveAt(index);
                CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Index");
        }

        [Route("decrease/{id}")]
        public IActionResult Decrease(int id)
        {
            List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");

            int index = CheckIsProductExists(id);

            var currentProduct = cart[index];

            if (currentProduct.Quantity > 1) currentProduct.Quantity--;
            else 
                cart.Remove(currentProduct);
            

            CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", cart);

            return RedirectToAction("index");
        }

        [Route("cart/checkout")]
        public IActionResult Checkout()
        {
            if(User.Identity.IsAuthenticated)
            {
                List<Item> cart = CustomSessionExtensions.GetSessionData<List<Item>>(HttpContext.Session, "cart");

                Console.WriteLine("run");

                if(cart != null)
                {
                    CustomSessionExtensions.SetSessionData(HttpContext.Session, "cart", null);
                    TempData["checkout__alert--success"] = AlertExtensions.ShowAlert(Alerts.Success, "Thanh toán thành công");
                    return RedirectToAction("index");
                }
            }

            TempData["checkout__alert--error"] = "Vui lòng đăng nhập để thanh toán";

            return RedirectToAction("signIn", "account");
        }
    }
}
