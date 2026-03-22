using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class LopHocPhan
{
    public string MaLhp { get; set; } = null!;

    public string? MaMh { get; set; }

    public string? MaGv { get; set; }

    public string? MaHk { get; set; }

    public int? SiSo { get; set; }

    public virtual GiangVien? MaGvNavigation { get; set; }

    public virtual HocKy? MaHkNavigation { get; set; }

    public virtual MonHoc? MaMhNavigation { get; set; }
}
