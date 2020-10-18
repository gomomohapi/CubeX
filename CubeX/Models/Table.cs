using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class Table
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public byte[] Image { get; set; }

        [Required]
        [Range(0, 10)]
        public int Avaibility { get; set; }
    }
}