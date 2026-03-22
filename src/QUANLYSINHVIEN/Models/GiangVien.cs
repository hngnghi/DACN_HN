using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class GiangVien
{
    public string MaGv { get; set; } = null!;

    public string HoTenGv { get; set; } = null!;

    public string? Email { get; set; }

    public string? MaKhoa { get; set; }

    public virtual ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();

    public virtual Khoa? MaKhoaNavigation { get; set; }

    public virtual ICollection<MonHoc> MonHocs { get; set; } = new List<MonHoc>();

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
