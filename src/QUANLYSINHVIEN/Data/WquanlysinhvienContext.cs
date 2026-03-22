using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QUANLYSINHVIEN.Models;

namespace QUANLYSINHVIEN.Data;

public partial class WquanlysinhvienContext : DbContext
{
    public WquanlysinhvienContext()
    {
    }

    public WquanlysinhvienContext(DbContextOptions<WquanlysinhvienContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BangDiem> BangDiems { get; set; }

    public virtual DbSet<DuBaoKetQua> DuBaoKetQuas { get; set; }

    public virtual DbSet<GiangVien> GiangViens { get; set; }

    public virtual DbSet<HocKy> HocKies { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<Lop> Lops { get; set; }

    public virtual DbSet<LopHocPhan> LopHocPhans { get; set; }

    public virtual DbSet<MonHoc> MonHocs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThongKeBi> ThongKeBis { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BangDiem>(entity =>
        {
            entity.HasKey(e => e.MaBd).HasName("PK__BangDiem__272475A7EB2037BC");

            entity.ToTable("BangDiem");

            entity.HasIndex(e => new { e.MaSv, e.MaMh, e.MaHk }, "UC_DiemDuyNhat").IsUnique();

            entity.Property(e => e.MaBd).HasColumnName("MaBD");
            entity.Property(e => e.DiemQt).HasColumnName("DiemQT");
            entity.Property(e => e.DiemTb).HasColumnName("DiemTB");
            entity.Property(e => e.MaHk)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaHK");
            entity.Property(e => e.MaMh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.XepLoai).HasMaxLength(20);

            entity.HasOne(d => d.MaHkNavigation).WithMany(p => p.BangDiems)
                .HasForeignKey(d => d.MaHk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BangDiem__MaHK__68487DD7");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.BangDiems)
                .HasForeignKey(d => d.MaMh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BangDiem__MaMH__6754599E");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.BangDiems)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BangDiem__MaSV__66603565");
        });

        modelBuilder.Entity<DuBaoKetQua>(entity =>
        {
            entity.HasKey(e => e.MaDuBao).HasName("PK__DuBao_Ke__03C13C226B3058CD");

            entity.ToTable("DuBao_KetQua");

            entity.Property(e => e.DuBaoXepLoai).HasMaxLength(20);
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.NgayDuBao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.DuBaoKetQuas)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__DuBao_KetQ__MaSV__73BA3083");
        });

        modelBuilder.Entity<GiangVien>(entity =>
        {
            entity.HasKey(e => e.MaGv).HasName("PK__GiangVie__2725AEF3D0A6DAF4");

            entity.ToTable("GiangVien");

            entity.HasIndex(e => e.Email, "UQ__GiangVie__A9D10534231BD4C2").IsUnique();

            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HoTenGv)
                .HasMaxLength(100)
                .HasColumnName("HoTenGV");
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.GiangViens)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK__GiangVien__MaKho__5441852A");
        });

        modelBuilder.Entity<HocKy>(entity =>
        {
            entity.HasKey(e => e.MaHk).HasName("PK__HocKy__2725A6E7D9289234");

            entity.ToTable("HocKy");

            entity.Property(e => e.MaHk)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaHK");
            entity.Property(e => e.NamHoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TenHk)
                .HasMaxLength(50)
                .HasColumnName("TenHK");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.MaKhoa).HasName("PK__Khoa__653904056E5D8551");

            entity.ToTable("Khoa");

            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenKhoa).HasMaxLength(100);
            entity.Property(e => e.TruongKhoa).HasMaxLength(100);
        });

        modelBuilder.Entity<Lop>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__Lop__3B98D273BA6EF291");

            entity.ToTable("Lop");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.KhoaHoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenLop).HasMaxLength(100);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.Lops)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK__Lop__MaKhoa__4BAC3F29");
        });

        modelBuilder.Entity<LopHocPhan>(entity =>
        {
            entity.HasKey(e => e.MaLhp).HasName("PK__LopHocPh__3B9B96909B57498A");

            entity.ToTable("LopHocPhan");

            entity.Property(e => e.MaLhp)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaLHP");
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaHk)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaHK");
            entity.Property(e => e.MaMh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.SiSo).HasDefaultValue(0);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.LopHocPhans)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK__LopHocPhan__MaGV__5EBF139D");

            entity.HasOne(d => d.MaHkNavigation).WithMany(p => p.LopHocPhans)
                .HasForeignKey(d => d.MaHk)
                .HasConstraintName("FK__LopHocPhan__MaHK__5FB337D6");

            entity.HasOne(d => d.MaMhNavigation).WithMany(p => p.LopHocPhans)
                .HasForeignKey(d => d.MaMh)
                .HasConstraintName("FK__LopHocPhan__MaMH__5DCAEF64");
        });

        modelBuilder.Entity<MonHoc>(entity =>
        {
            entity.HasKey(e => e.MaMh).HasName("PK__MonHoc__2725DFD99FBF8D08");

            entity.ToTable("MonHoc");

            entity.Property(e => e.MaMh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaMH");
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.TenMh)
                .HasMaxLength(100)
                .HasColumnName("TenMH");

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.MonHocs)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK__MonHoc__MaGV__5812160E");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__2725081A068F9F8D");

            entity.ToTable("SinhVien");

            entity.HasIndex(e => e.Email, "UQ__SinhVien__A9D105349599FB49").IsUnique();

            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.DiemTrungBinh).HasDefaultValue(0.0);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TrangThaiHocTap).HasMaxLength(50);

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .HasConstraintName("FK__SinhVien__MaLop__5070F446");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TenDangNhap).HasName("PK__TaiKhoan__55F68FC1E2D90F1C");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaGv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaGV");
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.MaGvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaGv)
                .HasConstraintName("FK__TaiKhoan__MaGV__6D0D32F4");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__TaiKhoan__MaSV__6C190EBB");
        });

        modelBuilder.Entity<ThongKeBi>(entity =>
        {
            entity.HasKey(e => e.MaThongKe).HasName("PK__ThongKe___60E521F46D229A6D");

            entity.ToTable("ThongKe_BI");

            entity.Property(e => e.DiemTb).HasColumnName("DiemTB");
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.XepLoai).HasMaxLength(20);

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.ThongKeBis)
                .HasForeignKey(d => d.MaSv)
                .HasConstraintName("FK__ThongKe_BI__MaSV__6FE99F9F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
