using LAPTRINHWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using LAPTRINHWEB.Controllers;
using LAPTRINHWEB.Models;

using PagedList;

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
            if (flight != null)
            {
                ViewBag.AirlineName = data.Airlines.Single(a => a.AirlineID == flight.AirlineID).AirlineName;
                ViewBag.DepartureAirportName = data.Airports.Single(a => a.AirportID == flight.DepartureAirportID).AirportName;
                ViewBag.ArrivalAirportName = data.Airports.Single(a => a.AirportID == flight.ArrivalAirportID).AirportName;
            }
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

        public ActionResult SearchFlights(string departureCity, string arrivalCity, DateTime? departureDate, DateTime? returnDate, int? page)
        {
            var cities = data.Airports.Select(a => a.City).Distinct().ToList();
            ViewBag.Cities = new SelectList(cities);

            if (departureDate.HasValue)
            {
                var outboundFlights = from f in data.Flights
                                      join a in data.Airlines on f.AirlineID equals a.AirlineID
                                      join d in data.Airports on f.DepartureAirportID equals d.AirportID
                                      join r in data.Airports on f.ArrivalAirportID equals r.AirportID
                                      where d.City == departureCity && r.City == arrivalCity && f.DepartureTime.Date == departureDate.Value
                                      select new FlightDetailsViewModel
                                      {
                                          Flight = f,
                                          AirlineName = a.AirlineName,
                                          AirlineLogo = a.Logo, // Thêm logo hãng hàng không
                                          DepartureAirportName = d.AirportName,
                                          ArrivalAirportName = r.AirportName,
                                          AvailableSeats = data.Seats.Count(s => s.FlightID == f.FlightID && s.IsAvailable),
                                          SeatPrice = data.Seats.Where(s => s.FlightID == f.FlightID && s.IsAvailable).Select(s => s.Price).FirstOrDefault(), // Thêm giá ghế
                                          SeatClass = data.Seats.Where(s => s.FlightID == f.FlightID && s.IsAvailable).Select(s => s.SeatClass).FirstOrDefault() // Thêm hạng ghế
                                      };

                int pageSize = 10;
                int pageNumber = (page ?? 1);
                ViewBag.OutboundFlights = outboundFlights.ToPagedList(pageNumber, pageSize);
            }

            // Tương tự cho ViewBag.ReturnFlights

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
            var user = Session["Taikhoan"] as Customer;
            if (user != null)
            {
                // Nếu người dùng đã đăng nhập, điền tự động thông tin của họ
                var guest = new Guest
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                    // Thêm các trường khác nếu cần
                };
                return View(guest);
            }
            else
            {
                // Nếu người dùng chưa đăng nhập, hiển thị form trống
                return View();
            }
        }

        [HttpPost]
        public ActionResult EnterCustomerInfo(Guest guest)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (User.Identity.IsAuthenticated)
            {
                // Nếu người dùng đã đăng nhập, lấy thông tin người dùng
                var user = Session["Taikhoan"] as Customer;

                // Lưu CustomerID vào Session
                Session["CustomerID"] = user.CustomerID;
            }
            else
            {
                // Nếu người dùng chưa đăng nhập, tạo một bản ghi Guest mới
                var newGuest = new Guest
                {
                    UserName = guest.UserName,
                    Email = guest.Email,
                    PhoneNumber = guest.PhoneNumber
                    // Thêm các trường khác nếu cần
                };
                data.Guests.InsertOnSubmit(newGuest);
                data.SubmitChanges();

                // Lưu GuestID vào Session
                Session["GuestID"] = newGuest.GuestID;
            }

            // Chuyển hướng đến trang tiếp theo
            return RedirectToAction("SelectSeats");
        }




        [HttpPost]
        public ActionResult BookFlights()
        {
            // Get the current user and their selected seats var user = Session["Taikhoan"] as Customer;
            var user = Session["Taikhoan"] as Customer;
            var guestId = Session["GuestID"] as int?;

            // Retrieve the selected seats from the session
            var selectedSeats = Session["SelectedSeats"] as int[];
            var seats = data.Seats.Where(s => selectedSeats.Contains(s.SeatID)).ToList();

            // Create a new booking
            var booking = new Booking
            {
                CustomerID = user != null ? user.CustomerID : (int?)null, // Use null for guests
                GuestID = guestId, // Set GuestID for guests
                BookingDate = DateTime.Now,
                IsPaid = false,
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
                Guest = Session["Guest"] as Guest // Add the guest to the ViewModel
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

            // Display a success message
            TempData["SuccessMessage"] = "Đặt vé thành công! " + body;

            // Redirect the user to the home page
            return RedirectToAction("Index", "Home");
        }


        private void SendEmail(string subject, string body)
        {
            string recipient = "ttmdvhd@gmail.com"; // Địa chỉ email cố định để kiểm tra

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

                public ActionResult MyBookings()
                {
    
                    var user = Session["Taikhoan"] as Customer;
                    if (user == null)
                 {
                return RedirectToAction("Dangnhap", "Accounts");
                }

            var bookings = data.Bookings.Where(b => b.CustomerID == user.CustomerID).ToList();
            var bookingViewModels = new List<BookingViewModel>();
            var bookingId = Session["BookingID"] as int?;

            foreach (var booking in bookings)
            {
                var bookingDetails = data.BookingDetails.Where(bd => bd.BookingID == booking.BookingID).ToList();
                var guest = new Guest
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                var viewModel = new BookingViewModel
                {
                    Booking = booking,
                    BookingDetails = bookingDetails,
                    Guest = guest
                };
                bookingViewModels.Add(viewModel);
            }

            return View(bookingViewModels);
        }


    


        // Các hành động khác...
    }
}
