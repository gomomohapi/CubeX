using CubeX.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CubeX.ViewModels
{
    public class TableRentalViewModel
    {
        public int Id { get; set; }

        //Table Model Properties
        public int TableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        [Range(0, 10)]
        public int Avaibility { get; set; }

        //Table book Model Properties
        public DateTime? BookingMade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BookingTime { get; set; }

        public int Seats { get; set; }
        public String Status { get; set; }

        //Users Model Properties
        public string UserId { get; set; }
        public string Email { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; } 
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }

        public string actionName
        {
            get
            {
                if (Status.ToLower().Contains(SD.RequestedLower))
                {
                    return "Approve";
                }
                if (Status.ToLower().Contains(SD.ApprovedLower))
                {
                    return "CheckIn";
                }
                if (Status.ToLower().Contains(SD.CheckedInLower))
                {
                    return "Close";
                }
                return null;
            }
        }
    }
}