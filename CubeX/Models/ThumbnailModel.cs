using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class ThumbnailModel
    {
        public int TableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public string Link { get; set; }
    }
}