using LAPTRINHWEB.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LAPTRINHWEB.Controllers
{
    public class AdminsController : BaseController
    {
        // GET: Admins
        public ActionResult Index()
        {
            return View();
        }
        #region ManagerAirport



        public ActionResult ManagerAirport()
        {
            return View(data.Airports.ToList());
        }
        private List<Airport> CreateAirport(int count)
        {
            return data.Airports.OrderByDescending(a => a.AirportID).Take(count).ToList();
        }
        public ActionResult Airport(int? page)
        {
            int pageSize = 10;
            int pageNum = (page ?? 1);

            var sachmoi = CreateAirport(15);
            return View(sachmoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult AddNewAirport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewAirport(Airport airport)
        {
            data.Airports.InsertOnSubmit(airport);
            data.SubmitChanges();
            return RedirectToAction("ManagerAirport");
        }
        public ActionResult EditAirport(int id)
        {
            Airport airport = data.Airports.SingleOrDefault(n => n.AirportID == id);
            if (airport == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(airport);
        }
        
        [HttpPost]
        public ActionResult EditAirport(Airport airport)
        {
            if (ModelState.IsValid)
            {
                Airport airportToUpdate = data.Airports.SingleOrDefault(n => n.AirportID == airport.AirportID);
                if (airportToUpdate != null)
                {
                    TryUpdateModel(airportToUpdate);
                    if (ModelState.IsValid)
                    {
                        data.SubmitChanges();
                        return RedirectToAction("ManagerAirport");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật không thành công");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy sân bay");
                }
            }
            else
            {
                ModelState.AddModelError("", "Dữ liệu không hợp lệ");
            }
            return View(airport);
        }
        public ActionResult DetailAirport(int id)
        {
            Airport airport = data.Airports.SingleOrDefault(n => n.AirportID == id);
            if (airport == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(airport);
        }
        public ActionResult DeleteAirport(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Airport airport = data.Airports.SingleOrDefault(n => n.AirportID== id);
            if (airport == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }
            return View(airport);
        }


        [HttpPost, ActionName("DeleteAirport")]
        public ActionResult ConfirmDeleteAirport(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Airport airport = data.Airports.SingleOrDefault(n => n.AirportID== id);
            if (airport == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }

            try
            {
                data.Airports.DeleteOnSubmit(airport);
                data.SubmitChanges();
                TempData["Message"] = "Xóa sân bay thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi  xóa sân bay: " + ex.Message;
            }

            return RedirectToAction("ManagerAirport");
        }
        #endregion

        #region ManagerAirline
        public ActionResult ManagerAirline()
        {
            return View(data.Airlines.ToList());
        }
        private List<Airline> CreateAirline(int count)
        {
            return data.Airlines.OrderByDescending(a => a.AirlineID).Take(count).ToList();
        }
        public ActionResult Airline(int? page)
        {
            int pageSize = 10;
            int pageNum = (page ?? 1);

            var sachmoi = CreateAirport(15);
            return View(sachmoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult AddNewAirline()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewAirline(Airline airline)
        {
            data.Airlines.InsertOnSubmit(airline);
            data.SubmitChanges();
            return RedirectToAction("ManagerAirline");
        }
        public ActionResult EditAirlines(int id)
        {
            Airline airline = data.Airlines.SingleOrDefault(n => n.AirlineID == id);
            if (airline == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(airline);
        }

        [HttpPost]
        public ActionResult EditAirline(Airline airline)
        {
            if (ModelState.IsValid)
            {
                Airline airlineToUpdate = data.Airlines.SingleOrDefault(n => n.AirlineID == airline.AirlineID);
                if (airlineToUpdate != null)
                {
                    TryUpdateModel(airlineToUpdate);
                    if (ModelState.IsValid)
                    {
                        data.SubmitChanges();
                        return RedirectToAction("ManagerAirline");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật không thành công");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy sân bay");
                }
            }
            else
            {
                ModelState.AddModelError("", "Dữ liệu không hợp lệ");
            }
            return View(airline);
        }
        public ActionResult DetailAirline(int id)
        {
            Airline airline = data.Airlines.SingleOrDefault(n => n.AirlineID == id);
            if (airline == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(airline);
        }
        public ActionResult DeleteAirline(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Airline airline = data.Airlines.SingleOrDefault(n => n.AirlineID == id);
            if (airline == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }
            return View(airline);
        }


        [HttpPost, ActionName("DeleteAirline")]
        public ActionResult ConfirmDeleteAirline(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Airline airline = data.Airlines.SingleOrDefault(n => n.AirlineID == id);
            if (airline == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }

            try
            {
                data.Airlines.DeleteOnSubmit(airline);
                data.SubmitChanges();
                TempData["Message"] = "Xóa sân bay thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi  xóa sân bay: " + ex.Message;
            }

            return RedirectToAction("ManagerAirline");
        }

        #endregion

        #region ManagerFlight
        public ActionResult ManagerFlight()
        {
            return View(data.Flights.ToList());
        }
        private List<Airport> CreateFlight(int count)
        {
            return data.Airports.OrderByDescending(a => a.AirportID).Take(count).ToList();
        }
        public ActionResult Flight(int? page)
        {
            int pageSize = 10;
            int pageNum = (page ?? 1);

            var sachmoi = CreateFlight(15);
            return View(sachmoi.ToPagedList(pageNum, pageSize));
        }
        public ActionResult AddNewFlight()
        {
            ViewBag.AirlineID = new SelectList(data.Airlines, "AirlineID", "AirlineName");
            ViewBag.DepartureAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            ViewBag.ArrivalAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            return View();
        }
        [HttpPost]
        public ActionResult AddNewFlight(Flight flight)
        {
            data.Flights.InsertOnSubmit(flight);
            data.SubmitChanges();
            return RedirectToAction("ManagerFlight");
        }

        public ActionResult EditFlight(int id)
        {
            Flight flight = data.Flights.SingleOrDefault(n => n.AirlineID == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.AirlineID = new SelectList(data.Airlines, "AirlineID", "AirlineName");
            ViewBag.DepartureAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            ViewBag.ArrivalAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            return View(flight);
        }

        [HttpPost]
        public ActionResult EditFlight(Flight flight)
        {
            ViewBag.AirlineID = new SelectList(data.Airlines, "AirlineID", "AirlineName");
            ViewBag.DepartureAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            ViewBag.ArrivalAirportID = new SelectList(data.Airports, "AirportID", "AirportName");
            if (ModelState.IsValid)
            {
                Flight airlineToUpdate = data.Flights.SingleOrDefault(n => n.AirlineID == flight.AirlineID);
                if (airlineToUpdate != null)
                {
                    TryUpdateModel(airlineToUpdate);
                    if (ModelState.IsValid)
                    {
                        data.SubmitChanges();
                        return RedirectToAction("ManagerFlight");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật không thành công");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Không tìm thấy sân bay");
                }
            }
            else
            {
                ModelState.AddModelError("", "Dữ liệu không hợp lệ");
            }
            return View(flight);
        }

        public ActionResult DetailFlight(int id)
        {
            // Lấy ra đối tượng sách theo mã
            Flight flight = data.Flights.SingleOrDefault(n => n.FlightID == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(flight);
        }
        public ActionResult DeleteFlight (int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Flight flight = data.Flights.SingleOrDefault(n => n.FlightID == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }
            return View(flight);
        }


        [HttpPost, ActionName("DeleteFlight")]
        public ActionResult ConfirmDeleteFlight(int id)
        {
            // Lấy ra đối tượng sách cần xóa theo mã
            Flight flight = data.Flights.SingleOrDefault(n => n.FlightID == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Sân bay không tồn tại.");
            }

            try
            {
                data.Flights.DeleteOnSubmit(flight);
                data.SubmitChanges();
                TempData["Message"] = "Xóa sân bay thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi  xóa sân bay: " + ex.Message;
            }

            return RedirectToAction("ManagerFlight");
        }

            #endregion
            #region LoginAdmin
            [HttpGet]
        public ActionResult Login()
        { return View(); }
        public ActionResult Login(FormCollection collection)
        {
            string tendn = collection["username"];
            string matkhau = collection["password"];
            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = data.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admins");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        #endregion


    }
}
