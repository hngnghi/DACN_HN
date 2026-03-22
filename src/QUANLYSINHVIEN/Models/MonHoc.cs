using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class MonHoc
{
    public string MaMh { get; set; } = null!;

    public string TenMh { get; set; } = null!;

    public int SoTinChi { get; set; }

    public string? MaGv { get; set; }

    public virtual ICollection<BangDiem> BangDiems { get; set; } = new List<BangDiem>();

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

    public virtual GiangVien? MaGvNavigation { get; set; }
}
