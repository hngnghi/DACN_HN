using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class HocKy
{
    public string MaHk { get; set; } = null!;

    public string? TenHk { get; set; }

    public string? NamHoc { get; set; }

    public virtual ICollection<BangDiem> BangDiems { get; set; } = new List<BangDiem>();

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
}
