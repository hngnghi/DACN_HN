using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYSINHVIEN.Data;
using QUANLYSINHVIEN.Filters;
using QUANLYSINHVIEN.Models;

namespace QUANLYSINHVIEN.Controllers
{
    public class TaiKhoansController : Controller
    {
        private readonly WquanlysinhvienContext _context;

        public TaiKhoansController(WquanlysinhvienContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoans/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /TaiKhoans/Login
        [HttpPost]
        public IActionResult Login(string tenDangNhap, string matKhau)
        {
            // 1. Tìm tài khoản trong CSDL (So sánh chính xác tên và pass)
            // Lưu ý: Trong thực tế nên mã hóa mật khẩu (MD5/SHA), ở đây ta làm đơn giản trước
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(x => x.TenDangNhap == tenDangNhap && x.MatKhau == matKhau);

            if (taiKhoan != null)
            {
                // 2. Đăng nhập thành công -> Lưu thông tin vào Session
                HttpContext.Session.SetString("UserName", taiKhoan.TenDangNhap);
                HttpContext.Session.SetString("Role", taiKhoan.VaiTro);

                if (taiKhoan.MaSv != null) HttpContext.Session.SetString("MaSv", taiKhoan.MaSv);
                if (taiKhoan.MaGv != null) HttpContext.Session.SetString("MaGv", taiKhoan.MaGv);

                return RedirectToAction("Index", "Home");
            }

            // 3. Đăng nhập thất bại
            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        // GET: /TaiKhoans/Logout
        public IActionResult Logout()
        {
            // Xóa Session -> Đăng xuất
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /TaiKhoans/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ---------------------------------------------------
        // 1. TRANG THÔNG TIN CÁ NHÂN (PROFILE)
        // ---------------------------------------------------
        [Authentication] // Bắt buộc đăng nhập
        public async Task<IActionResult> Profile()
        {
            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("UserName");

            if (role == "SinhVien")
            {
                var maSv = HttpContext.Session.GetString("MaSv");
                var sinhVien = await _context.SinhViens
                    .Include(s => s.MaLopNavigation)
                    .FirstOrDefaultAsync(s => s.MaSv == maSv);

                return View("ProfileSinhVien", sinhVien);
            }
            else if (role == "GiangVien")
            {
                var maGv = HttpContext.Session.GetString("MaGv");
                var giangVien = await _context.GiangViens
                    .Include(g => g.MaKhoaNavigation)
                    .FirstOrDefaultAsync(g => g.MaGv == maGv);

                return View("ProfileGiangVien", giangVien);
            }
            else // Admin
            {
                return View("ProfileAdmin");
            }
        }

        // ---------------------------------------------------
        // 2. CHỨC NĂNG ĐỔI MẬT KHẨU
        // ---------------------------------------------------
        [Authentication]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authentication]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            // 1. Kiểm tra dữ liệu nhập
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // 2. Lấy user hiện tại từ DB
            var username = HttpContext.Session.GetString("UserName");
            var user = await _context.TaiKhoans.FindAsync(username);

            if (user != null)
            {
                // 3. Kiểm tra mật khẩu cũ
                if (user.MatKhau == CurrentPassword)
                {
                    // 4. Cập nhật mật khẩu mới
                    user.MatKhau = NewPassword;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    ViewBag.Success = "Đổi mật khẩu thành công!";
                }
                else
                {
                    ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                }
            }

            return View();
        }
    }
}