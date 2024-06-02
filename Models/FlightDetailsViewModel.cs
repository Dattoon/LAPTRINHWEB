using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LAPTRINHWEB.Models
{
    public class FlightDetailsViewModel
    {
        public Flight Flight { get; set; }
        public string AirlineName { get; set; }
        public string AirlineLogo { get; set; } 
        public string DepartureAirportName { get; set; }
        public string ArrivalAirportName { get; set; }
        public decimal SeatPrice { get; set; } 
        public string SeatClass { get; set; } 
        public int AvailableSeats { get; set; }
    }
}
