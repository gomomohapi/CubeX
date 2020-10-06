using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class CartItem : FoodItem
    {
        [Range(1, 20)]
        public int? Quantity { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Value must be a Currency ex. R20")]
        public decimal TotalAmount { get; set; }

        public string OrderID { get; set; }

        public string ShoppingCartID { get; set; }
    }
}