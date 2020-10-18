using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CubeX.Models
{
    public class TableBooking
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int TableId { get; set; }

        public DateTime? BookingMade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BookingDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime? BookingTime { get; set; }

        public int Seats { get; set; }

        [Required]
        public StatusEnum Status { get; set; }

        public enum StatusEnum
        {
            Requested,
            Approved,
            Rejected,
            CheckedIn,
            Closed
        }
    }
}