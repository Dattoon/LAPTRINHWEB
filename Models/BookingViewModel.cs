using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LAPTRINHWEB.Models
{
    public class BookingViewModel
    {
        public Booking Booking { get; set; }
        public List<BookingDetail> BookingDetails { get; set; }
        public Guest Guest { get; set; }
        public List<Seat> Seats { get; set; } 
        public List<Flight> Flights { get; set; } 
        public List<Airline> Airlines { get; set; } 
    }
}
