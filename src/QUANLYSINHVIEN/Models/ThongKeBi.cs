using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class ThongKeBi
{
    public int MaThongKe { get; set; }

    public string? MaSv { get; set; }

    public double? DiemTb { get; set; }

    public string? XepLoai { get; set; }

    public int? SoTinChiTichLuy { get; set; }

    public double? TyLeMonRot { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
