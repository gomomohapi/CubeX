using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class Order
    {
        public string ID { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Value must be a Currency ex. $20")]
        [Display(Name = "Sum")]
        public double TotalAmount { get; set; }

        [StringLength(50)]
        [Display(Name = "Order Destination")]

        public string Destination { get; set; } // Remember to HTML Encode this property when displaying it back to the user

        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<CartItem> Items { get; set; }
    }
}