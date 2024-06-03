    using LAPTRINHWEB.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;



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




        // GET: User/UpdateProfile
        [HttpGet]
        public ActionResult UpdateProfile()
        {
            var user = (Customer)Session["Taikhoan"];
            if (user == null)
            {
                return RedirectToAction("Dangnhap");
            }
            return View(user);
        }

        // POST: User/UpdateProfile
        // POST: User/UpdateProfile
        [HttpPost]
        public ActionResult UpdateProfile(Customer updatedUser)
        {
            try
            {
                // Get the user from the database
                var dbUser = data.Customers.SingleOrDefault(u => u.CustomerID == updatedUser.CustomerID);
                if (dbUser != null)
                {
                    dbUser.UserName = updatedUser.UserName;
                    dbUser.Email = updatedUser.Email;
                    dbUser.PhoneNumber = updatedUser.PhoneNumber;
                    data.SubmitChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewBag.Error = "Có lỗi xảy ra khi cố gắng cập nhật thông tin người dùng: " + e.Message;
                return View(updatedUser);
            }
        }

       

        private async Task SendResetPasswordEmailAsync(string email, string resetCode)
        {
            var fromAddress = new MailAddress("fourairline@gmail.com", "Four Airline");
            var toAddress = new MailAddress(email);
            const string subject = "Password Reset";
            string body = "Your password reset code is: " + resetCode;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, "ysjm dynx uawd arit")
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }



        // GET: User/ChangePassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var user = (Customer)Session["Taikhoan"];
            if (user == null)
            {
                return RedirectToAction("Dangnhap");
            }
            return View(user);
        }

        // POST: User/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var user = (Customer)Session["Taikhoan"];
            if (user == null)
            {
                return RedirectToAction("Dangnhap");
            }
            if (user.PasswordHash != oldPassword)
            {
                ViewBag.Error = "Mật khẩu cũ không đúng";
                return View(user);
            }
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và mật khẩu xác nhận không khớp";
                return View(user);
            }
            // Get the user from the database
            var dbUser = data.Customers.SingleOrDefault(u => u.CustomerID == user.CustomerID);
            if (dbUser != null)
            {
                dbUser.PasswordHash = newPassword; // Consider encrypting this
                try
                {
                    data.SubmitChanges();
                }
                catch (Exception e)
                {
                    ViewBag.Error = "Có lỗi xảy ra khi cố gắng cập nhật mật khẩu: " + e.Message;
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: User/ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: User/ForgotPassword
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = data.Customers.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                // Generate a reset password code
                var resetCode = Guid.NewGuid().ToString();
                user.ResetPasswordCode = resetCode;
                // Consider setting an expiry time for the code
                data.SubmitChanges();

                // Send the reset password code to the user's email
                // This will depend on your email sending setup
                await SendResetPasswordEmailAsync(user.Email, resetCode);
            }
            return RedirectToAction("ResetPassword");
        }


        // GET: User/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: User/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string resetCode, string newPassword)
        {
            var user = data.Customers.SingleOrDefault(u => u.ResetPasswordCode == resetCode);
            if (user != null)
            {
                user.PasswordHash = newPassword; // Consider encrypting this
                user.ResetPasswordCode = null; // Clear the reset password code
                data.SubmitChanges();
            }
            return RedirectToAction("Dangnhap");
        }



    }
}   