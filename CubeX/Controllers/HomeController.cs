using CubeX.Keys;
using CubeX.Models;
using CubeX.ViewModels.Home;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public HomeController() : base()
        {

        }

        [AllowAnonymous]
        public ActionResult Image(string fileName)
        {

            if (fileName == null)
            {
                fileName = "food-one.jpg";
            }

            string filePath = HttpContext.Server.MapPath("~/Images/" + fileName);

            byte[] file = System.IO.File.ReadAllBytes(filePath);
            string fileType = MimeMapping.GetMimeMapping(filePath);

            return File(file, fileType);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminIndex()
        {
            return View("Index", "~/Views/Shared/_AdminLayout.cshtml");
        }



        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Buy(string id, string returnUrl)
        {
            // TODO Add FoodItem to ApplicationUser's Shopping Cart Entity in the DB, if it doesn't exist? create a new one

            FoodItem foodItem = db.FoodItems.Find(id);

            if (foodItem == null)
            {
                return HttpNotFound();
            }
            if (returnUrl != null)
            {
                ViewBag.ReturnUrl = returnUrl;
            }



            return View(foodItem);
        }

        [HttpPost]
        public ActionResult ConfirmBuy(string ID, string returnUrl, int Quantity)
        {
            FoodItem foodItem = db.FoodItems.Find(ID);
            if (foodItem.isEnabled == false)
            {
                ViewBag.ErrorMessage = "This item is currently unavailable.";
                return View("All", GetFoodItems());
            }

            if (foodItem != null)
            {
                // TODO Add foodItem to User's Cart

                var user = db.Users.Find(User.Identity.GetUserId<string>());

                // Will change this when i enable Authorization Filters
                // user != null

                // If the user does not currently have a shoppingCart instance, create and save a new one for the user
                if (user != null && user.ShoppingCart == null)
                {
                    // Save a new cart in the DB
                    if (CreateAndSaveNewShoppingCart(user) == false)
                    {
                        // Then there was a problem with saving the new shopping cart in the DB and linking it with the User
                        throw new Exception("There was a problem saving a new Shopping cart in the database.");
                    }
                }

                ShoppingCart cart = db.Users.Find(User.Identity.GetUserId<string>()).ShoppingCart;

                return CreateSaveCartItemAndRedirect(foodItem, cart, returnUrl, Quantity);

            }
            else
            {
                // Then The Id Supplied is not valid
                ViewBag.ErrorMeesage = "The item was not specified, please click a 'Add to Cart' Button to proceed";
                return View("All", GetFoodItems());
            }


        }

        private string GetNewUid()
        {
            return Guid.NewGuid().ToString();
        }

        private ActionResult CreateSaveCartItemAndRedirect(FoodItem foodItem, ShoppingCart cart, string returnUrl, int Quantity)
        {
            if (IsItemInCart(cart, foodItem) == true)
            {
                // TODO Implement : Disable 'Buy' for fooditems that are already in your cart
                ViewBag.ErrorMessage = $"Item '{ foodItem.Name}' is already in your Cart";
                return View("All", GetFoodItems());
            }

            // TODO Create a new CartItem, link it to the Shopping Cart and save it to the database
            CartItem cartItem = new CartItem
            {
                ID = GetNewUid(),
                Name = foodItem.Name,
                Price = foodItem.Price,
                Quantity = Quantity,
                TotalAmount = (foodItem.Price * Quantity),
                ShoppingCartID = cart.ID,
                Discriminator = Discriminators.CartItem
            };

            // Add the price to ShoppingCart
            cart.Sum += cartItem.TotalAmount;

            // Save changes
            db.Entry(cart).State = EntityState.Modified;
            db.CartItems.Add(cartItem);
            db.SaveChanges();

            // TODO Change Actions and Views to Handle Strings now instead of an Integer
            ViewBag.ReturnUrl = returnUrl;
            //return View("ItemAdded", cartItem);
            return RedirectToLocalAction(returnUrl);
        }

        private bool CreateAndSaveNewShoppingCart(ApplicationUser user)
        {
            // If User's Cart is already empty, create a new one in DbSet<ShoppingCart> and connect it to the user Through the UserId Property
            ShoppingCart shoppingCart = new ShoppingCart
            {
                ApplicationUserID = user.Id,
                ID = GetNewUid(),
                Sum = 0,
                Items = new List<CartItem>()
            };
            user.ShoppingCartID = shoppingCart.ID;

            db.Entry(user).State = EntityState.Modified;
            db.ShoppingCarts.Add(shoppingCart);

            int affectedRows = db.SaveChanges();

            return affectedRows > 0;
        }

        private bool IsItemInCart(ShoppingCart cart, FoodItem foodItem)
        {
            // TODO check if item is already in cart, if it is, add
            bool isInCart = cart.Items.Where(item => item.Name == foodItem.Name).SingleOrDefault() != null;

            return isInCart;
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


        // GET: Category
        [AllowAnonymous]
        public ActionResult Category(int? categoryId)
        {
            var categories = db.Categories.Include(c => c.foodList);
            CategoryViewModel viewModel = new CategoryViewModel();
            viewModel.foodList = new List<FoodItem>();
            if (categoryId != null)
            {
                ViewBag.CategoryID = categoryId;

                var foodList = categories
                    .Where(c => c.ID == categoryId)
                    .Single()
                    .foodList;

                viewModel.foodList = FilterFoodItems(foodList);
            }

            viewModel.Category = categories.ToList();

            ViewBag.ReturnUrl = Url.Action("Category", "Home");

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult All(string message)
        {
            var foods = GetFoodItems();

            if (message != null)
            {
                ViewBag.Message = "Item Removed.";
            }
            return View(foods);


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
            // Transform it to a Dictionary before filtering is applied
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