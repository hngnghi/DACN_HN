using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class Khoa
{
    public string MaKhoa { get; set; } = null!;

    public string TenKhoa { get; set; } = null!;

    public string? TruongKhoa { get; set; }

    public virtual ICollection<GiangVien> GiangViens { get; set; } = new List<GiangVien>();

    public virtual ICollection<Lop> Lops { get; set; } = new List<Lop>();
}
