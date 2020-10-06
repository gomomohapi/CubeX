using CubeX.Models;
using CubeX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {

        private ApplicationDbContext db;

        public UserController()
        {
            db = ApplicationDbContext.Create();
        }

        // GET: User
        public ActionResult Index()
        {
            var user = from u in db.Users
                       from r in u.Roles
                       join role in db.Roles on r.RoleId equals role.Id
                       select new UserVM
                       {
                           Id = u.Id,
                           FirstName = u.FirstName,
                           Surname = u.Surname,
                           Email = u.Email,
                           Role = role.Name
                       };

            var usersList = user.ToList();

            return View(usersList);
        }

        //GET Edit
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }
            UserVM model = new UserVM()
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Email = user.Email,
                Id = user.Id
            };

            return View(model);
        }

        //POST Method for EDIT Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserVM user)
        {
            if (!ModelState.IsValid)
            {
                UserVM model = new UserVM()
                {
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    Email = user.Email,
                    Id = user.Id
                };
                return View(model);
            }
            else
            {
                var userInDb = db.Users.Single(u => u.Id == user.Id);
                userInDb.FirstName = user.FirstName;
                userInDb.Surname = user.Surname;
                userInDb.Email = user.Email;
            }

            db.SaveChanges();

            return RedirectToAction("Index", "User");
        }


        public ActionResult Details(string id)
        {
            if (id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            UserVM model = new UserVM()
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Email = user.Email,
                Id = user.Id
            };
            return View(model);
        }

        //DELETE Get Method
        public ActionResult Delete(string id)
        {
            if (id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id);
            UserVM model = new UserVM()
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Email = user.Email,
                Id = user.Id
            };
            return View(model);
        }

        //DELETE Post method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var userInDb = db.Users.Find(id);
            if (id == null || id.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //userInDb.Disable = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}