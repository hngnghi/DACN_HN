using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class DuBaoKetQua
{
    public int MaDuBao { get; set; }

    public string? MaSv { get; set; }

    public double? XacSuatTotNghiep { get; set; }

    public string? DuBaoXepLoai { get; set; }

    public DateTime? NgayDuBao { get; set; }

    public string? MoTaPhanTich { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
