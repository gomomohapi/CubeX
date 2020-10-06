using CubeX.Models;
using CubeX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}