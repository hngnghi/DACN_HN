using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class TaiKhoan
{
    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? VaiTro { get; set; }

    public string? MaSv { get; set; }

    public string? MaGv { get; set; }

    public virtual GiangVien? MaGvNavigation { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
