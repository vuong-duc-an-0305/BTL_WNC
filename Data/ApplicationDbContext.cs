using Microsoft.EntityFrameworkCore;
using OnlineClassManagement.Models;

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

                // Cấu hình ánh xạ Enum 'Role' sang kiểu string trong CSDL
                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .HasMaxLength(20); 
            });

            // Cấu hình Class
            modelBuilder.Entity<Class>(entity =>
            {
                // ... (Cấu hình Class giữ nguyên)
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
                // ... (Cấu hình ClassEnrollment giữ nguyên)
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
                
                // === THÊM CẤU HÌNH NÀY ĐỂ SỬA LỖI ===
                // Chỉ định EF lưu Enum 'AssignmentType' (nếu có) dưới dạng 'String'
                // (Tôi giả định rằng bạn CÓ dùng Enum dựa trên lỗi này)
                entity.Property(e => e.AssignmentType)
                    .HasConversion<string>() 
                    .HasMaxLength(50); // Khớp với NVARCHAR(50) trong CSDL
                // === KẾT THÚC PHẦN THÊM ===
            });

            // Cấu hình Submission
            modelBuilder.Entity<Submission>(entity =>
            {
                // ... (Cấu hình Submission giữ nguyên)
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
            });

            // Cấu hình CourseMaterial
            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                // ... (Cấu hình CourseMaterial giữ nguyên)
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
                // ... (Cấu hình Announcement giữ nguyên)
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