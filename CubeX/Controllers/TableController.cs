using CubeX.Models;
using CubeX.Utilities;
using CubeX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
    [Authorize(Roles = SD.AdminUserRole)]
    public class TableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Table
        public ActionResult Index()
        {
            var tables = db.Tables.ToList();
            return View(tables);
        }


        // GET: Table/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }

            var model = new TableViewModel
            {
                Table = table
            };
            return View(model);
        }

        // GET: Table/Create
        public ActionResult Create()
        {
            var model = new TableViewModel
            {
            };
            return View(model);
        }

        // POST: Table/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TableViewModel tableVM)
        {
            var table = new Table
            {
                Avaibility = tableVM.Table.Avaibility,
                Description = tableVM.Table.Description,
                Image = tableVM.Table.Image,
                Name = tableVM.Table.Name
            };
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tableVM);
        }

        // GET: Table/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }

            var model = new TableViewModel
            {
                Table = table
            };
            return View(model);
        }

        // POST: Table/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(TableViewModel tableVM)
        {
            var table = new Table
            {
                Id = tableVM.Table.Id,
                Avaibility = tableVM.Table.Avaibility,
                Description = tableVM.Table.Description,
                Image = tableVM.Table.Image,
                Name = tableVM.Table.Name
            };
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tableVM);
        }

        // GET: Table/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            var model = new TableViewModel
            {
                Table = table
            };
            return View(model);
        }

        // POST: Table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Table table = db.Tables.Find(id);
            db.Tables.Remove(table);
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