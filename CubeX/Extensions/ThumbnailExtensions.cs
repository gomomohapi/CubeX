using CubeX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CubeX.Extensions
{
    public static class ThumbnailExtensions
    {
        public static IEnumerable<ThumbnailModel> GetBookThumbnail(this List<ThumbnailModel> thumbnails, ApplicationDbContext db = null, string search = null)
        {
            try
            {
                if (db == null) db = ApplicationDbContext.Create();

                thumbnails = (from b in db.Tables
                              select new ThumbnailModel
                              {
                                  TableId = b.Id,
                                  Name = b.Name,
                                  Description = b.Description,
                                  Image = b.Image,
                                  Link = "/TableDetail/Index/" + b.Id,
                              }).ToList();

                if (search != null)
                {
                    return thumbnails.Where(t => t.Name.ToLower().Contains(search.ToLower())).OrderBy(t => t.Name);
                }
            }
            catch (Exception ex)
            {

            }
            return thumbnails.OrderBy(t => t.Name);

        }
    }
}