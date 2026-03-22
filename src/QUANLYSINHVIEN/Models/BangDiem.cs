using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class BangDiem
{
    public int MaBd { get; set; }

    public string MaSv { get; set; } = null!;

    public string MaMh { get; set; } = null!;

    public string MaHk { get; set; } = null!;

    public double? DiemQt { get; set; }

    public double? DiemThi { get; set; }

    public double? DiemTb { get; set; }

    public string? XepLoai { get; set; }

    public virtual HocKy MaHkNavigation { get; set; } = null!;

    public virtual MonHoc MaMhNavigation { get; set; } = null!;

    public virtual SinhVien MaSvNavigation { get; set; } = null!;
}
