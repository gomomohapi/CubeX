
using CubeX.Keys;
using CubeX.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;

namespace CubeX.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: ShoppingCart/Details/5

        public ActionResult Details()
        {

            ApplicationUser user = GetUser();

            bool userHasOrder = db.Orders
                .Where(order => order.UserID == user.Id).Count() > 0;

            if (userHasOrder)
            {
                ViewBag.HasOrder = true;
            }

            ShoppingCart shoppingCart = user.ShoppingCart;

            // The shoppingCart will always be null when it is Empty because it's instance is deleted when it is empty
            if (shoppingCart == null)
            {
                // The view will tell the user that there is no item in the Cart
                return View();
            }

            return View(shoppingCart.Items);
        }



        // GET: ShoppingCart/Edit/5
        public async Task<ActionResult> Edit(string cartItemId, string returnUrl)
        {
            if (cartItemId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cartItem = await db.CartItems.Where(c => c.ID == cartItemId).SingleOrDefaultAsync();

            if (cartItem == null)
            {
                return HttpNotFound();
            }

            if (returnUrl != null)
            {
                ViewBag.ReturnUrl = returnUrl;
            }
            return View(cartItem);
        }

        // POST: ShoppingCart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(string ID, string returnUrl)
        {

            CartItem cartItem = db.CartItems.Find(ID);

            if (TryUpdateModel(cartItem, new string[] { "Quantity" }))
            {
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToLocalAction(returnUrl);
            }

            return View(cartItem);
        }

        // GET: ShoppingCart/Remove/id
        public async Task<ActionResult> Remove(string cartItemId, string returnUrl)
        {
            if (cartItemId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            string userId = User.Identity.GetUserId();

            var item = await db.CartItems.FindAsync(cartItemId);
            var cart = await db.ShoppingCarts
                .Where(shoppingCart => shoppingCart.ApplicationUserID == userId)
                .SingleOrDefaultAsync();


            if (item == null || cart == null)
            {
                return HttpNotFound();
            }

            try
            {
                db.Entry(item).State = EntityState.Deleted;

                await db.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.ToString());
                ViewBag.ErrorMessage = "There was an error item was not successfully removed, please try again...";
                return View("Details", cart.Items.ToList());
            }

            ViewBag.Message = "Item Removed.";

            // If there is no more item, then delete the ShoppingCart instance from the database
            if (cart.Items.Count == 0)
            {
                bool isDeleted = DeleteShoppingCart(cart);
                if (isDeleted)
                {
                    return RedirectToAction("All", "Home");
                }
            }


            return RedirectToLocalAction(returnUrl);
        }

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout([Bind(Include = "Destination")] Order order)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();

                ApplicationUser user = db.Users
                    .Include(usr => usr.ShoppingCart)
                    .Include(usr => usr.ShoppingCart.Items)
                    .Where(usr => usr.Id == userId)
                    .SingleOrDefault();

                ShoppingCart cart = user.ShoppingCart;

                if (cart == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                order.ID = GetNewUid();
                order.TotalAmount = cart.Sum;
                order.UserID = user.Id;
                order.OrderDate = DateTime.Now;

                SendEmail(order, cart, user);

                // TODO set all cartItems order id to the new order entity
                cart.Items.ToList().ForEach(cartItem =>
                {
                    cartItem.ShoppingCartID = null;
                    cartItem.OrderID = order.ID;
                    db.Entry(cartItem).State = EntityState.Modified;
                });

                // Remove user's ShoppingCart
                user.ShoppingCartID = null;
                db.Entry(user).State = EntityState.Modified;
                db.Entry(cart).State = EntityState.Deleted;

                try
                {
                    db.Orders.Add(order);
                    db.SaveChanges();

                    return View("CheckoutConfirmed");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.ToString());
                    return RedirectToAction("Checkout");
                }
            }
            return View();
        }

        public void SendEmail(Order order, ShoppingCart cart, ApplicationUser user)
        {
            string orderItems = "";
            foreach (var item in cart.Items.ToList())
            {
                orderItems += $"\t Item: {item.Name} | Price: {item.Price.ToString("C")} | Quantity: {item.Quantity} \n";
            }


            string fullName = user.FirstName + " " + user.Surname;
            string orderMessage =
                $"Hello, {fullName}! \n\n" +
                $"You have made an order with DUT Rendezvous Restaurant. \n\n" +
                $"Your order details are as follows: \n" +
                $"{orderItems} \n" +
                $"Date: {order.OrderDate.ToLongDateString()} \n" +
                $"Time: {order.OrderDate.ToShortTimeString()} \n" +
                $"Total: {order.TotalAmount.ToString("C")} \n\n" +
                $"Thank you for ordering with DUT Rendezvous Restaurant :)";

            //SendEmail
            var senderEmail = new MailAddress("dutrendezvous@gmail.com", "Rendezvous Restaurant");
            var recieverMail = new MailAddress(user.UserName, fullName);
            var password = "RendezvousDUT123";
            var sub = $"New Order #{order.ID.Substring(0, 5)}!";
            var body = orderMessage;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, recieverMail)
            {
                Subject = sub,
                Body = body
            })
            {
                smtp.Send(mess);
            }
        }

        // Helpers
        private bool DeleteShoppingCart(ShoppingCart cart)
        {
            ApplicationUser user = GetUser();
            user.ShoppingCartID = null;
            db.Entry(cart).State = EntityState.Deleted;
            db.Entry(user).State = EntityState.Modified;

            int rowsAffected = db.SaveChanges();

            return rowsAffected > 0;

        }

        private List<FoodItem> GetFoodItems()
        {
            List<FoodItem> foodItems = db.FoodItems.Include(f => f.Category)
                 .Where(f => f.Discriminator == Discriminators.FoodItem)
                 .ToList();

            return FilterFoodItems(foodItems);
        }

        private List<FoodItem> FilterFoodItems(ICollection<FoodItem> foodItemsList)
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return foodItemsList.ToList();
            }

            Dictionary<string, FoodItem> foodItems = foodItemsList.ToDictionary(fList => fList.Name);
            // Filter it with items in the Cart

            string userId = User.Identity.GetUserId();
            var cart = db.ShoppingCarts.Include(shoppingCart => shoppingCart.Items)
                .Where(s => s.ApplicationUserID == userId)
                .SingleOrDefault();

            if (cart != null)
            {
                List<CartItem> cartItems = cart.Items.ToList();

                cartItems.ForEach(cartItem =>
                {
                    if (foodItems.ContainsKey(cartItem.Name))
                    {
                        // Disable the item
                        var foodItem = foodItems[cartItem.Name];
                        foodItem.isEnabled = false;
                        foodItem.CartItemId = cartItem.ID;

                    }
                });
            }

            return foodItems.Select(f => f.Value).ToList();
        }

        private ApplicationUser GetUser()
        {
            string userId = User.Identity.GetUserId();

            ApplicationUser user = db.Users
                .Include(u => u.ShoppingCart)
                .Where(u => u.Id == userId)
                .SingleOrDefault();

            return user;
        }

        private string GetNewUid()
        {
            return Guid.NewGuid().ToString();
        }

        private ActionResult RedirectToLocalAction(string returnUrl)
        {
            if (returnUrl != null)
            {
                if (Url.IsLocalUrl(returnUrl))
                {       // If returnUrl is Local, then goto it
                    return Redirect(returnUrl);
                }
                else
                {   // If not, then return an Error View with a message in the ViewBag
                    ViewBag.ErrorMessage = "The return Url did not originate from our Server";
                    return View("Error");
                }
            }

            return RedirectToAction("All");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}