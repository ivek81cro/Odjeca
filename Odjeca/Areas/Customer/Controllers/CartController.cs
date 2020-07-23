using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Odjeca.Data;
using Odjeca.Models;
using Odjeca.Models.ViewModels;
using Odjeca.Utility;
using Stripe;

namespace Odjeca.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private UserManager<IdentityUser> _userManager;

        [BindProperty]
        public OrderDetatilsCart DetailCart { get; set; }

        public CartController(ApplicationDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            DetailCart = new OrderDetatilsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                DetailCart.listCart = cart.ToList();
            }

            foreach (var list in DetailCart.listCart)
            {
                list.StoreItem = await _db.StoreItem.FirstOrDefaultAsync(m => m.Id == list.StoreItemId);
                DetailCart.OrderHeader.OrderTotal = DetailCart.OrderHeader.OrderTotal + (list.StoreItem.Price * list.Count);
                list.StoreItem.Description = SD.ConvertToRawHtml(list.StoreItem.Description);
                if (list.StoreItem.Description.Length > 100)
                {
                    list.StoreItem.Description = list.StoreItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailCart.OrderHeader.OrderTotalOriginal = DetailCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Discount.Where(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailCart.OrderHeader.OrderTotalOriginal);
            }

            return View(DetailCart);
        }

        public async Task<IActionResult> Summary()
        {

            DetailCart = new OrderDetatilsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();
            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                DetailCart.listCart = cart.ToList();
            }

            foreach (var list in DetailCart.listCart)
            {
                list.StoreItem = await _db.StoreItem.FirstOrDefaultAsync(m => m.Id == list.StoreItemId);
                DetailCart.OrderHeader.OrderTotal = DetailCart.OrderHeader.OrderTotal + (list.StoreItem.Price * list.Count);

            }
            DetailCart.OrderHeader.OrderTotalOriginal = DetailCart.OrderHeader.OrderTotal;
            DetailCart.OrderHeader.PickupName = applicationUser.Name;
            DetailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            DetailCart.OrderHeader.PickUpTime = DateTime.Now;


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Discount.Where(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailCart.OrderHeader.OrderTotalOriginal);
            }


            return View(DetailCart);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailCart.listCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

            DetailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            DetailCart.OrderHeader.OrderDate = DateTime.Now;
            DetailCart.OrderHeader.UserId = claim.Value;
            DetailCart.OrderHeader.Status = SD.PaymentStatusPending;
            DetailCart.OrderHeader.PickUpTime = Convert.ToDateTime(DetailCart.OrderHeader.PickUpDate.ToShortDateString() +
                " " + DetailCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _db.OrderHeader.Add(DetailCart.OrderHeader);
            await _db.SaveChangesAsync();

            DetailCart.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in DetailCart.listCart)
            {
                item.StoreItem = await _db.StoreItem.FirstOrDefaultAsync(m => m.Id == item.StoreItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    StoreItemId = item.StoreItemId,
                    OrderId = DetailCart.OrderHeader.Id,
                    Description = item.StoreItem.Description,
                    Name = item.StoreItem.Name,
                    Price = item.StoreItem.Price,
                    Count = item.Count
                };
                DetailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                _db.OrderDetails.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Discount.Where(c => c.Name.ToLower() == DetailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, DetailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                DetailCart.OrderHeader.OrderTotal = DetailCart.OrderHeader.OrderTotalOriginal;
            }
            DetailCart.OrderHeader.CouponCodeDiscount = DetailCart.OrderHeader.OrderTotalOriginal - DetailCart.OrderHeader.OrderTotal;

            _db.ShoppingCart.RemoveRange(DetailCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            await _db.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(DetailCart.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID : " + DetailCart.OrderHeader.Id,
                Source = stripeToken
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                DetailCart.OrderHeader.Status = SD.PaymentStatusRejected;
            }
            else
            {
                DetailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
                DetailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                DetailCart.OrderHeader.Status = SD.StatusSubmitted;
                 //Send confirmation mail to user
                var user = await _userManager.GetUserAsync(User);
                string details = $"<h2>Order number {DetailCart.OrderHeader.Id} details</h2>" +
                    ("_________________________________________________________________________________________") +
                    "<h3>Order detils:</h3>" +
                    ("_________________________________________________________________________________________") +
                    ("<table border='0' cellpadding='5' cellspacing='5'>") +
                    ("<tr><th>No.</th><th>Name</th><th>Pcs.</th><th>Price</th></tr>");
                int i = 1;
                foreach (var item in DetailCart.listCart)
                {
                    details = details + ($"<tr><td>{i}.</td><td>{item.StoreItem.Name}</td><td>{item.Count}</td><td>{item.StoreItem.Price}$</td></tr>");
                    i++;
                }
                details = details + ("</table>") +
                    ("_________________________________________________________________________________________") +
                    ($"<p><strong>Order total: {DetailCart.OrderHeader.OrderTotal}$</strong></p>") +
                    ($"<p>Payment status: {DetailCart.OrderHeader.PaymentStatus}</p><br />") +
                    ($"<p>You can review order details your account, order history section.</p>");

                await _emailSender.SendEmailAsync(user.Email, $"Order details from SportStore", details);
            }
            else
            {
                DetailCart.OrderHeader.Status = SD.PaymentStatusRejected;
            }

            await _db.SaveChangesAsync();

            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Confirm", "Order", new { id = DetailCart.OrderHeader.Id });

        }

        public IActionResult AddCoupon()
        {
            if (DetailCart.OrderHeader.CouponCode == null)
            {
                DetailCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            cart.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _db.ShoppingCart.Remove(cart);
                await _db.SaveChangesAsync();

                var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _db.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId);

            _db.ShoppingCart.Remove(cart);
            await _db.SaveChangesAsync();

            var cnt = _db.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);


            return RedirectToAction(nameof(Index));
        }
    }
}