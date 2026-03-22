using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QUANLYSINHVIEN.Filters
{
    // Kế thừa từ ActionFilterAttribute để làm màng lọc
    public class Authentication : ActionFilterAttribute
    {
        // 1. Thuộc tính để quy định Role nào được vào (nếu để trống thì ai đăng nhập rồi cũng được)
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // --- KIỂM TRA 1: ĐÃ ĐĂNG NHẬP CHƯA? ---
            var sessionUser = context.HttpContext.Session.GetString("UserName");
            var sessionRole = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(sessionUser))
            {
                // Chưa đăng nhập -> Đá về trang Login
                context.Result = new RedirectToActionResult("Login", "TaiKhoans", null);
                return;
            }

            // --- KIỂM TRA 2: CÓ ĐỦ QUYỀN KHÔNG? ---
            if (!string.IsNullOrEmpty(Roles))
            {
                // Nếu Controller yêu cầu quyền (VD: "Admin"), mà user là "SinhVien" -> Chặn
                // Lưu ý: Roles truyền vào có thể là chuỗi "Admin,GiangVien"
                if (!Roles.Contains(sessionRole))
                {
                    // Đăng nhập rồi nhưng không đủ quyền -> Chuyển sang trang "Cấm truy cập"
                    context.Result = new RedirectToActionResult("AccessDenied", "TaiKhoans", null);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}