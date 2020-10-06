using CubeX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CubeX.ViewModels.Home
{
    public class CategoryViewModel
    {
        public ICollection<Category> Category { get; set; }
        public ICollection<FoodItem> foodList { get; set; }
    }
}