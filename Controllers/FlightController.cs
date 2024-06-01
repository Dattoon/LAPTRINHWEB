using LAPTRINHWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using LAPTRINHWEB.Controllers;
using LAPTRINHWEB.Models;

namespace FourAirLineFinal.Controllers
{
    public class FlightController : BaseController
    {
   

        // Hiển thị danh sách chuyến bay
        public ActionResult Index()
        {
            var flights = data.Flights.ToList();
            return View(flights);
        }

        // Hiển thị chi tiết chuyến bay
        public ActionResult Details(int id)
        {
            var flight = data.Flights.SingleOrDefault(f => f.FlightID == id);
            return View(flight);
        }

        // Hiển thị danh sách ghế cho một chuyến bay
        public ActionResult SelectSeats(int id)
        {
            var seats = data.Seats.Where(s => s.FlightID == id && s.IsAvailable).ToList();
            return View(seats);
        }

        // Hiển thị danh sách chuyến bay để chọn chuyến bay về
        public ActionResult SelectReturnFlight(int id)
        {
            var flights = data.Flights.Where(f => f.FlightID != id).ToList();
            return View(flights);
        }

        // Tìm kiếm chuyến bay
        public ActionResult SearchFlights(string departureCity, string arrivalCity, DateTime? departureDate, DateTime? returnDate)
        {
            var cities = data.Airports.Select(a => a.City).Distinct().ToList();
            ViewBag.Cities = new SelectList(cities);

            if (departureDate.HasValue)
            {
                var outboundFlights = from f in data.Flights
                                      where data.Airports.Single(a => a.AirportID == f.DepartureAirportID).City == departureCity
                                      && data.Airports.Single(a => a.AirportID == f.ArrivalAirportID).City == arrivalCity
                                      && f.DepartureTime.Date == departureDate.Value
                                      select f;
                ViewBag.OutboundFlights = outboundFlights.ToList();
            }

            if (returnDate.HasValue)
            {
                var returnFlights = from f in data.Flights
                                    where data.Airports.Single(a => a.AirportID == f.DepartureAirportID).City == arrivalCity
                                    && data.Airports.Single(a => a.AirportID == f.ArrivalAirportID).City == departureCity
                                    && f.DepartureTime.Date == returnDate.Value
                                    select f;
                ViewBag.ReturnFlights = returnFlights.ToList();
            }

            return View();
        }

        [HttpPost]
        public ActionResult BookSeats(int[] selectedSeats)
        {
            foreach (var seatId in selectedSeats)
            {
                var seat = data.Seats.SingleOrDefault(s => s.SeatID == seatId);
                if (seat == null || !seat.IsAvailable)
                {
                    // Handle the error, e.g., return an error message to the user
                    return View("Error", new { message = "One or more of the selected seats are not available." });
                }
                seat.IsAvailable = false;
                seat.IsBooked = true;
            }
            data.SubmitChanges();

            // Store the selected seats in a session
            Session["SelectedSeats"] = selectedSeats;

            return RedirectToAction("EnterCustomerInfo");
        }

        public ActionResult EnterCustomerInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EnterCustomerInfo(Guest guest)
        {
            // Lưu thông tin khách hàng vào Session
            Session["Guest"] = guest;

            // Chuyển hướng đến trang tiếp theo
            return RedirectToAction("SelectSeats");
        }

        [HttpPost]
        public ActionResult BookFlights(Guest guest)
        {
            // Get the current user and their selected seats
            var user = User.Identity.IsAuthenticated ? data.Customers.SingleOrDefault(c => c.UserName == User.Identity.Name) : null;

            // Retrieve the selected seats from the session
            var selectedSeats = Session["SelectedSeats"] as int[];
            var seats = data.Seats.Where(s => selectedSeats.Contains(s.SeatID)).ToList();

            // Create a new booking
            var booking = new Booking
            {
                CustomerID = user != null ? user.CustomerID : (int?)null, // Use null for guests
                BookingDate = DateTime.Now,
                IsPaid = false,
                IsRoundTrip = seats.Count > 1
            };
            data.Bookings.InsertOnSubmit(booking);
            data.SubmitChanges();

            // Create booking details for each selected seat
            var bookingDetails = new List<BookingDetail>();
            foreach (var seat in seats)
            {
                var flight = data.Flights.Single(f => f.FlightID == seat.FlightID);
                var bookingDetail = new BookingDetail
                {
                    BookingID = booking.BookingID,
                    SeatID = seat.SeatID,
                    OutboundFlightID = flight.FlightID,
                    // Set ReturnFlightID if this is a round trip
                };
                data.BookingDetails.InsertOnSubmit(bookingDetail);
                bookingDetails.Add(bookingDetail);
            }
            data.SubmitChanges();

            // Create the ViewModel
            var viewModel = new BookingViewModel
            {
                Booking = booking,
                BookingDetails = bookingDetails,
                Guest = guest // Add the guest to the ViewModel
            };

            return View(viewModel);
        }

        public ActionResult ConfirmBooking(int bookingId)
        {
            // Get the booking and its details
            var booking = data.Bookings.Single(b => b.BookingID == bookingId);
            var bookingDetails = data.BookingDetails.Where(bd => bd.BookingID == bookingId).ToList();

            // Create the ViewModel
            var viewModel = new BookingViewModel
            {
                Booking = booking,
                BookingDetails = bookingDetails,
                // Add other necessary information...
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Pay(int bookingId, string cardName, string cardNumber, DateTime expiryDate, string securityCode)
        {
            // Lưu thông tin thanh toán vào Session
            Session["CardName"] = cardName;
            Session["CardNumber"] = cardNumber;

            // Chuyển hướng đến trang ReviewPayment
            return RedirectToAction("ReviewPayment", new { id = bookingId });
        }

        public ActionResult ReviewPayment(int id)
        {
            // Lấy thông tin đặt chỗ
            var booking = data.Bookings.Single(b => b.BookingID == id);

            // Lấy thông tin thanh toán từ Session
            ViewBag.CardName = Session["CardName"];
            ViewBag.CardNumber = Session["CardNumber"];

            // Lấy thông tin khách hàng từ Session
            var guest = Session["Guest"] as Guest;

            // Tạo ViewModel
            var viewModel = new BookingViewModel
            {
                Booking = booking,
                BookingDetails = data.BookingDetails.Where(bd => bd.BookingID == id).ToList(),
                Guest = guest // Thêm thông tin khách hàng vào ViewModel
            };

            // Trả về View với ViewModel
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ConfirmPayment(int bookingId)
        {
            // Get the booking
            var booking = data.Bookings.Single(b => b.BookingID == bookingId);

            // Mark the booking as paid
            booking.IsPaid = true;
            data.SubmitChanges();

            // Prepare the booking details for the email
            var bookingDetails = data.BookingDetails.Where(bd => bd.BookingID == bookingId).ToList();
            string body = "Thank you for booking your flight with us. Your seats have been successfully booked. Here are your booking details:\n\n";
            foreach (var detail in bookingDetails)
            {
                body += $"Flight ID: {detail.OutboundFlightID}, Seat Number: {detail.Seat.SeatNumber}, Seat Class: {detail.Seat.SeatClass}, Price: {detail.Seat.Price}\n";
            }

            // Send a confirmation email
            string customerEmail = User.Identity.Name; // Replace this with the actual customer's email
            string subject = "Booking Confirmation";
            SendEmail(customerEmail, subject, body);

            // Redirect the user to a confirmation page
            return RedirectToAction("Confirmation", new { id = bookingId });
        }

        private void SendEmail(string recipient, string subject, string body)
        {
            // Kiểm tra xem tham số recipient có hợp lệ hay không
            if (string.IsNullOrWhiteSpace(recipient))
            {
                throw new ArgumentException("Địa chỉ email người nhận không được để trống.", nameof(recipient));
            }

            // Địa chỉ email và tên người gửi
            var fromAddress = new MailAddress("fouairline@gmail.com", "FourAirLine Bay Cùng Bạn");
            var toAddress = new MailAddress(recipient);
            const string fromPassword = "fouairline533";

            // Cấu hình SMTP client
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Tạo và gửi email
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


        // Các hành động khác...
    }
}
