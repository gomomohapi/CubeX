using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CubeX.Models;

namespace CubeX.Controllers
{
    public class FoodItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FoodItems
        public ActionResult Index()
        {
            var foodItems = db.FoodItems.Include(f => f.Category).Where(x => x.Discriminator == "FoodItem");
            return View(foodItems.ToList());
        }

        // GET: FoodItems/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = db.FoodItems.Find(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            return View(foodItem);
        }

        // GET: FoodItems/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            return View();
        }

        // POST: FoodItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FoodItem foodItem, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //you can put your existing save code here
                if (file != null && file.ContentLength > 0)
                {
                    foodItem.Image = ConvertToBytes(file);
                }

                foodItem.isEnabled = true;
                foodItem.ID = Guid.NewGuid().ToString().Substring(0, 13).Replace("-", String.Empty);
                db.FoodItems.Add(foodItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", foodItem.CategoryID);
            return View(foodItem);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        // GET: FoodItems/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = db.FoodItems.Find(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", foodItem.CategoryID);
            return View(foodItem);
        }

        // POST: FoodItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodItem foodItem, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //you can put your existing save code here
                if (file != null && file.ContentLength > 0)
                {
                    foodItem.Image = ConvertToBytes(file);
                }

                db.Entry(foodItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", foodItem.CategoryID);
            return View(foodItem);
        }

        // GET: FoodItems/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FoodItem foodItem = db.FoodItems.Find(id);
            if (foodItem == null)
            {
                return HttpNotFound();
            }
            return View(foodItem);
        }

        // POST: FoodItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            FoodItem foodItem = db.FoodItems.Find(id);
            db.FoodItems.Remove(foodItem);
            db.SaveChanges();
            return RedirectToAction("Index");
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
