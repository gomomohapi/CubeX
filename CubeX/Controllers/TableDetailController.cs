using CubeX.Extensions;
using CubeX.Models;
using CubeX.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
    public class TableDetailController : Controller
    {
        private ApplicationDbContext db;

        public TableDetailController()
        {
            db = ApplicationDbContext.Create();
        }

        public ActionResult Book(string search = null)
        {
            var thumbnails = new List<ThumbnailModel>().GetBookThumbnail(ApplicationDbContext.Create(), search);
            var count = thumbnails.Count() / 4;

            var model = new List<ThumbnailBoxViewModel>();

            for (int i = 0; i <= count; i++)
            {
                model.Add(new ThumbnailBoxViewModel
                {
                    Thumbnails = thumbnails.Skip(i * 2).Take(2)
                });
            }


            return View(model);
        }


        // GET: BookDetail
        public ActionResult Index(int id)
        {
            var userid = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(u => u.Id == userid);

            var bookModel = db.Tables.SingleOrDefault(b => b.Id == id);

            //var rentalPrice = 0.0;
            //var oneMonthRental = 0.0;
            //var sixMonthRental = 0.0;
            //var rentalCount = 0;

            //if (userid != null && !User.IsInRole(SD.AdminUserRole))
            //{
            //    var chargeRate = from u in db.Users
            //                     join m in db.MembershipTypes on u.MembershipTypeId equals m.Id
            //                     where u.Id.Equals(userid)
            //                     select new { m.ChargeRateOneMonth, m.ChargeRateSixMonth, u.RentalCount };

            //    oneMonthRental = Convert.ToDouble(bookModel.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateOneMonth) / 100;
            //    sixMonthRental = Convert.ToDouble(bookModel.Price) * Convert.ToDouble(chargeRate.ToList()[0].ChargeRateSixMonth) / 100;
            //    rentalCount = Convert.ToInt32(chargeRate.ToList()[0].RentalCount);
            //}

            TableRentalViewModel model = new TableRentalViewModel
            {
                TableId = bookModel.Id,
                Avaibility = bookModel.Avaibility,
                Description = bookModel.Description,
                Image = bookModel.Image,
                Name = bookModel.Name,
                UserId = userid,

            };

            return View(model);
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