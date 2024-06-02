    using LAPTRINHWEB.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    namespace LAPTRINHWEB.Controllers
    {
        public class AccountsController : BaseController
        {
            // GET: User
            [HttpGet]
            public ActionResult Dangky()
            {
                return View();
            }

            [HttpPost]
            public ActionResult Dangky(FormCollection collection, Customer kh)
            {
                // Gán các giá trị người dùng nhập liệu cho các biến 

                var tendn = collection["TenDN"];
                var matkhau = collection["MatKhau"];
                var matkhaunhaplai = collection["Matkhaunhaplai"];
                var email = collection["Email"];

                var dienthoai = collection["DienThoai"];


                // Kiểm tra hợp lệ của các trường dữ liệu


                if (String.IsNullOrEmpty(tendn))
                    ViewData["Loi2"] = "Phải nhập tên đăng nhập";

                else if (String.IsNullOrEmpty(matkhau))
                    ViewData["Loi3"] = "Phải nhập mật khẩu";

                else if (String.IsNullOrEmpty(matkhaunhaplai))
                    ViewData["Loi4"] = "Phải nhập lại mật khẩu";
                if (String.IsNullOrEmpty(email))
                {
                    ViewData["Loi5"] = "Email không được bỏ trống";
                }


                if (String.IsNullOrEmpty(dienthoai))
                {
                    ViewData["Loi7"] = "Phải nhập điện thoại";
                }
                else
                {
                    // Gán giá trị cho đối tượng được tạo mới (kh)

                    kh.UserName = tendn;
                    kh.PasswordHash = matkhau; // Consider encrypting this
                    kh.Email = email;

                    kh.PhoneNumber = dienthoai;

                    try
                    {
                        data.Customers.InsertOnSubmit(kh);
                        data.SubmitChanges();
                        return RedirectToAction("Dangnhap");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        ViewData["Loi7"] = "Có lỗi xảy ra khi tạo tài khoản. Vui lòng thử lại.";
                    }
                }

                return this.Dangky();
            }



            [HttpGet]
            public ActionResult Dangnhap()
            {
                return View();
            }
            [HttpPost]
            public ActionResult Dangnhap(FormCollection collection)
            {
                var tendn = collection["TenDN"];
                var matkhau = collection["MatKhau"];
                if (String.IsNullOrEmpty(tendn))
                {
                    ViewData["Loi1"] = "Phải nhập tên đăng nhập";
                }
                else if (String.IsNullOrEmpty(matkhau))
                {
                    ViewData["Loi2"] = "Phải nhập mật khẩu";
                }
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

            [HttpGet]
            public ActionResult Dangxuat()
            {
                Session["Taikhoan"] = null;
                return RedirectToAction("Index", "Home");
            }
        }
    }   