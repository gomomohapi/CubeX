using CubeX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CubeX.ViewModels
{
    public class ThumbnailBoxViewModel
    {
        public IEnumerable<ThumbnailModel> Thumbnails { get; set; }
    }
}