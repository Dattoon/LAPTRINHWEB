using LAPTRINHWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LAPTRINHWEB.Controllers
{
    public class AccountsController : BaseController

    {
        private List<Customer> customers;
        public AccountsController()
        {
            // Dùng danh sách giả lập dữ liệu thay vì database
            customers = new List<Customer>
        {
            new Customer { UserName = "testuser", PasswordHash = "123456", Email = "test@example.com", PhoneNumber = "123456789" }
        };
        }




        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangky(FormCollection collection, Customer kh)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var email = collection["Email"];
            var dienthoai = collection["DienThoai"];

            if (String.IsNullOrEmpty(tendn))
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(matkhau))
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            else if (String.IsNullOrEmpty(matkhaunhaplai))
                ViewData["Loi4"] = "Phải nhập lại mật khẩu";
            else if (matkhau != matkhaunhaplai) // ✅ Kiểm tra mật khẩu nhập lại
                ViewData["Loi4"] = "Mật khẩu nhập lại không khớp";
            else if (String.IsNullOrEmpty(email))
                ViewData["Loi5"] = "Email không được bỏ trống";
            else if (String.IsNullOrEmpty(dienthoai))
                ViewData["Loi7"] = "Phải nhập điện thoại";
            else
            {
                kh.UserName = tendn;
                kh.PasswordHash = matkhau; // Cân nhắc mã hóa mật khẩu
                kh.Email = email;
                kh.PhoneNumber = dienthoai;
                 
                try
                {
                    if (data == null) // ✅ Kiểm tra data có bị null không
                    {
                        ViewData["Loi7"] = "Lỗi hệ thống, không thể kết nối cơ sở dữ liệu.";
                        return View();
                    }

                    data.Customers.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    return RedirectToAction("Dangnhap");
                }
                catch (Exception ex)
                {
                    ViewData["Loi7"] = "Có lỗi xảy ra khi tạo tài khoản. Vui lòng thử lại.";
                    Console.WriteLine(ex.Message); // Log lỗi ra console (hoặc dùng logging)
                }
            }

            return View(); // ✅ Trả về View để hiển thị lỗi nếu có
        }

        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        /*
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];

            if (String.IsNullOrEmpty(tendn))
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            else if (String.IsNullOrEmpty(matkhau))
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            else
            {
                Customer kh = data.Customers.SingleOrDefault(n => n.UserName == tendn && n.PasswordHash == matkhau);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        */
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];

            if (string.IsNullOrEmpty(tendn))
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            else if (string.IsNullOrEmpty(matkhau))
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            else
            {
                // ✅ Kiểm tra danh sách giả lập thay vì database
                Customer kh = customers.SingleOrDefault(n => n.UserName == tendn && n.PasswordHash == matkhau);
                if (kh != null)
                {
                    ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";

                    // ✅ Tránh NullReferenceException với Session
                    if (Session != null)
                    {
                        Session["Taikhoan"] = kh;
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult Dangxuat()
        {
            Session["Taikhoan"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
