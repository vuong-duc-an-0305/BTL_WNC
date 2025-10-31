using Microsoft.EntityFrameworkCore;
using OnlineClassManagement.Models;
// Thêm namespace cho Enums
using OnlineClassManagement.Models.Enums;

namespace OnlineClassManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20); 
            });

            // Cấu hình Class
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasIndex(e => e.ClassCode).IsUnique();
                entity.Property(e => e.ClassName).IsRequired();
                entity.Property(e => e.ClassCode).IsRequired();
                entity.Property(e => e.Semester).IsRequired();
                entity.Property(e => e.AcademicYear).IsRequired();
                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TaughtClasses)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình ClassEnrollment
            modelBuilder.Entity<ClassEnrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.ClassId, e.StudentId }).IsUnique();
                
                // === CẬP NHẬT: Thêm chuyển đổi Enum (nếu bạn có Model cho nó) ===
                // Giả sử EnrollmentStatus.cs bạn gửi là thuộc tính của ClassEnrollment
                entity.Property(e => e.Status) // (Tôi đoán tên thuộc tính là 'Status')
                    .HasConversion<string>() 
                    .HasMaxLength(50);
            });

            // Cấu hình Assignment
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.DueDate).IsRequired();
                entity.Property(e => e.MaxScore).HasPrecision(5, 2);
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.AssignmentType)
                    .HasConversion<string>() 
                    .HasMaxLength(50);
            });

            // Cấu hình Submission
            modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(e => e.Score).HasPrecision(5, 2);
                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();

                // === THÊM CẤU HÌNH NÀY ĐỂ SỬA LỖI TIẾP THEO ===
                // Chỉ định EF lưu Enum 'SubmissionStatus' (tôi đoán tên là Status)
                entity.Property(e => e.Status) // (Tôi đoán tên thuộc tính là 'Status')
                    .HasConversion<string>() 
                    .HasMaxLength(50); // Khớp với NVARCHAR(50) trong CSDL
                // === KẾT THÚC PHẦN THÊM ===
            });

            // Cấu hình CourseMaterial
            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.FileUrl).IsRequired();
                entity.Property(e => e.OriginalFileName).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.FileSize).IsRequired();
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.UploadedByUser)
                    .WithMany(p => p.UploadedMaterials)
                    .HasForeignKey(d => d.UploadedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình Announcement
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Announcements)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.CreatedAnnouncements)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình decimal precision
            modelBuilder.Entity<ClassEnrollment>()
                .Property(e => e.Grade)
                .HasPrecision(3, 2);
        }
    }
}