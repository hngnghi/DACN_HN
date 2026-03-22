using System;
using System.Collections.Generic;

namespace QUANLYSINHVIEN.Models;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string? MaLop { get; set; }

    public double DiemTrungBinh { get; set; }

    public string? TrangThaiHocTap { get; set; }

    public virtual ICollection<BangDiem> BangDiems { get; set; } = new List<BangDiem>();

    public virtual ICollection<DuBaoKetQua> DuBaoKetQuas { get; set; } = new List<DuBaoKetQua>();

    public virtual Lop? MaLopNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();

    public virtual ICollection<ThongKeBi> ThongKeBis { get; set; } = new List<ThongKeBi>();
}
