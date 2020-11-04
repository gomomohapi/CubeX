using CubeX.Models;
using CubeX.Utilities;
using CubeX.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CubeX.Controllers
{
    [Authorize]
    public class TableBookController : Controller
    {
        private ApplicationDbContext db;

        public TableBookController()
        {
            db = ApplicationDbContext.Create();
        }

        // GET: BookRent
        public ActionResult Index(int? pageNumber, string option = null, string search = null)
        {
            string userid = User.Identity.GetUserId();

            var model = from br in db.Booking
                        join b in db.Tables on br.TableId equals b.Id
                        join u in db.Users on br.UserId equals u.Id
                        select new TableRentalViewModel
                        {
                            Id = br.Id,
                            TableId = b.Id,
                            FirstName = u.FirstName,
                            LastName = u.Surname,
                            BookingMade = br.BookingMade,
                            Avaibility = b.Avaibility,
                            Description = b.Description,
                            Email = u.Email,
                            BookingDate = br.BookingDate,
                            BookingTime = br.BookingTime,
                            Image = b.Image,
                            Seats = br.Seats,
                            Status = br.Status.ToString(),
                            Name = b.Name,
                            UserId = u.Id

                        };

            if (option == "email" && search.Length > 0)
            {
                model = model.Where(u => u.Email.Contains(search));
            }
            if (option == "name" && search.Length > 0)
            {
                model = model.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search));
            }
            if (option == "status" && search.Length > 0)
            {
                model = model.Where(u => u.Status.Contains(search));
            }

            if (!User.IsInRole(SD.AdminUserRole))
            {
                model = model.Where(u => u.UserId.Equals(userid));
            }

            return View(model.ToList().ToPagedList(pageNumber ?? 1, 5));
        }



        [HttpPost]
        public ActionResult Reserve(TableRentalViewModel book)
        {
            var userid = User.Identity.GetUserId();
            Table tableToRent = db.Tables.Find(book.TableId);

            if (userid != null)
            {
                var userInDb = db.Users.SingleOrDefault(c => c.Id == userid);
                
                TableBooking tableBook = new TableBooking
                {
                    TableId = tableToRent.Id,
                    UserId = userid,
                    BookingMade = DateTime.Now,
                    BookingDate = book.BookingDate,
                    BookingTime = book.BookingTime,
                    Seats = book.Seats,
                    Status = TableBooking.StatusEnum.Requested,

                };

                db.Booking.Add(tableBook);
                var tableInDb = db.Tables.SingleOrDefault(c => c.Id == book.TableId);

                tableInDb.Avaibility -= 1;

                db.SaveChanges();
                return RedirectToAction("Index", "TableBook");
            }
            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TableBooking bookRent = db.Booking.Find(id);

            var model = getVMFromTableBook(bookRent);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        private TableRentalViewModel getVMFromTableBook(TableBooking bookRent)
        {
            Table tableSelected = db.Tables.Where(x => x.Id == bookRent.TableId).FirstOrDefault();

            var userDetails = from u in db.Users
                              where u.Id.Equals(bookRent.UserId)
                              select new { u.Id, u.FirstName, u.Surname, u.Email };

            TableRentalViewModel model = new TableRentalViewModel
            {
                Id = bookRent.Id,
                TableId = tableSelected.Id,
                Name = tableSelected.Name,
                Description = tableSelected.Description,
                Image = tableSelected.Image,
                Avaibility = tableSelected.Avaibility,
                BookingMade = bookRent.BookingMade,
                BookingDate = bookRent.BookingDate,
                BookingTime = bookRent.BookingTime,
                Seats = bookRent.Seats,
                TableNumber = bookRent.TableNumber,
                Status = bookRent.Status.ToString(),
                Email = userDetails.ToList()[0].Email,
                FirstName = userDetails.ToList()[0].FirstName,
                LastName = userDetails.ToList()[0].Surname,
                UserId = userDetails.ToList()[0].Id
            };

            return model;

        }


        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TableBooking tableBook = db.Booking.Find(id);

        //    var model = getVMFromBookRent(tableBook);
        //    if (model == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(model);
        //}

        //Decline GET Method
        public ActionResult Decline(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TableBooking tableBook = db.Booking.Find(id);

            tableBook.Status = TableBooking.StatusEnum.Rejected;
            Table tableInDb = db.Tables.Find(tableBook.TableId);
            tableInDb.Avaibility += 1;
            db.SaveChanges();

            return RedirectToAction("Index", "TableBook");
        }

        //Approve GET Method
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TableBooking tableBook = db.Booking.Find(id);

            var model = getVMFromTableBook(tableBook);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("Approve", model);

            //tableBook.Status = TableBooking.StatusEnum.Approved;

            //db.SaveChanges();


            //return RedirectToAction("Index", "TableBook");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(TableRentalViewModel model)
        {
            if (model.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                TableBooking tableBook = db.Booking.Find(model.Id);
                tableBook.Status = TableBooking.StatusEnum.Approved;

                db.SaveChanges();
            }

            return RedirectToAction("Index", "TableBook");
        }

        //PickUp Get Method
        public ActionResult CheckIn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TableBooking tableBook = db.Booking.Find(id);
            tableBook.Status = TableBooking.StatusEnum.CheckedIn;

            db.SaveChanges();


            return RedirectToAction("Index", "TableBook");
        }

        //Return Get Method
        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TableBooking tableBook = db.Booking.Find(id);
            tableBook.Status = TableBooking.StatusEnum.Closed;
            Table tableInDb = db.Tables.Find(tableBook.TableId);
            tableInDb.Avaibility += 1;

            db.SaveChanges();


            return RedirectToAction("Index", "TableBook");
        }

        //Delete GET Method
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TableBooking tableBook = db.Booking.Find(id);
            Table tableInDb = db.Tables.Find(tableBook.TableId);
            tableInDb.Avaibility += 1;
            db.Booking.Remove(tableBook);
            db.SaveChanges();

            return RedirectToAction("Index", "TableBook");
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