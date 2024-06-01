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
        public ActionResult AirportManager()
        {
            return View(data.Airports.ToList());
        }
        public ActionResult ManagerAirport()
        {
            return View(data.Airports.ToList());
        }
        private List<Airport> CreateAirport(int count)
        {
            return data.Airports.OrderByDescending(a => a.AirportID).Take(count).ToList();
        }
        public ActionResult Sach(int? page)
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
            return RedirectToAction("AirportManager");
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
                        return RedirectToAction("AirportManager");
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

            return RedirectToAction("AirportManager");
        }

    



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
    }
}
