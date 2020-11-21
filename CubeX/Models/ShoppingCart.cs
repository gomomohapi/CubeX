using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class ShoppingCart
    {
        [Required]
        public string ID { get; set; }

        //[DataType(DataType.Currency, ErrorMessage = "Value must be a Currency ex. R20")]
        [Required]
        public double Sum { get; set; }

        public string ApplicationUserID { get; set; }


        public virtual ICollection<CartItem> Items { get; set; }
    }
}