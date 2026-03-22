
**WEBAPP QUẢN LÝ SINH VIÊN TÍCH HỢP DASHBOARD BI & DỰ ĐOÁN KẾT QUẢ HỌC TẬP**

Một hệ thống web ứng dụng **Business Intelligence (BI)** và **Machine Learning (ML)** nhằm chuyển đổi quy trình quản lý điểm số từ "thụ động" sang "chủ động cảnh báo sớm" rủi ro học vụ cho Cố vấn học tập và Sinh viên.

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white) ![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)

---

## 🎯 Mục tiêu dự án
Giải quyết bài toán trong việc quản lý hàng ngàn sinh viên của các Cố vấn học tập & Nhà trường. Hệ thống đóng vai trò như một **Trợ lý ảo**, phân tích dữ liệu lịch sử để đưa ra các dự báo và Insight, giúp nhà trường can thiệp và hỗ trợ sinh viên kịp thời.

## ✨ Tính năng nổi bật

* **BI Dashboard (Hỗ trợ ra quyết định):** Trực quan hóa dữ liệu toàn trường bằng `Chart.js`. Cung cấp các Insight Cards nhận diện ngay "điểm nóng" (môn có tỷ lệ rớt cao, lớp có thành tích xuất sắc).
* **AI Prediction (Cảnh báo sớm):** Tích hợp thuật toán học máy `ML.NET Regression` kết hợp logic nghiệp vụ (*Hybrid Rule-based*) để dự báo điểm số cuối kỳ và cảnh báo nguy cơ rớt môn cá nhân hóa cho từng sinh viên.
* **Hiệu năng cao (High Performance):** Tối ưu hóa truy vấn Database bằng kỹ thuật `Bulk Operations` và `In-memory Dictionary Caching`. Giải quyết triệt để tình trạng treo máy, ép thời gian xử lý dự báo hàng loạt cho 200+ sinh viên xuống **dưới 1 giây**.
* **Quản lý Dữ liệu Lớn:** Tự động hóa Import/Export bảng điểm từ file Excel sử dụng thư viện `EPPlus`.
* **Bảo mật:** Triển khai cơ chế phân quyền chặt chẽ và chống truy cập ngang quyền (*Horizontal Access Control*) thông qua Session/Claims.

## 💻 Công nghệ sử dụng (Tech Stack)

* **Backend:** C#, ASP.NET Core MVC
* **Database:** SQL Server, Entity Framework Core (Code-First / Database-First)
* **Machine Learning:** ML.NET (SDCA / FastTree Regression)
* **Frontend:** HTML5, CSS3, Bootstrap 5, Chart.js
* **Libraries:** EPPlus (Xử lý Excel), Newtonsoft.Json

## 📸 Hình ảnh giao diện (Screenshots)

| Dashboard Quản trị viên (Admin) | Dashboard Sinh viên (Personalized) |
| :---: | :---: |
| <img src="[docs/Dashboard AD.jpg]" width="400"/> | <img src="[docs/Dashboard Sinh viên.jpg]" width="400"/> |
| *Insight Cards & Phân tích phổ điểm* | *Xu hướng GPA & Trợ lý ảo AI dự báo* |

## 🚀 Hướng dẫn cài đặt (Getting Started)

### Yêu cầu hệ thống (Prerequisites)
* [.NET SDK 6.0 / 7.0 / 8.0](https://dotnet.microsoft.com/download)
* SQL Server (hoặc SQL Server Express)
* Visual Studio 2022

### Các bước chạy dự án
1. **Clone repository này về máy:**
   git clone [https://github.com/](https://github.com/)[Tên-Github-Của-Bạn]/[Tên-Repo].git
   
2. **Cấu hình chuỗi kết.**
  **Mở file appsettings.json và thay đổi DefaultConnection cho phù hợp với SQL Server của bạn.**
  "ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=QUANLYSINHVIEN;Trusted_Connection=True;MultipleActiveResultSets=true"
}

3. Cập nhật Database
  **Mở Package Manager Console trong Visual Studio và chạy lệnh:**
  Update-Database
