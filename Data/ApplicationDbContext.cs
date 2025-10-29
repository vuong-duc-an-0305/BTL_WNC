using Microsoft.EntityFrameworkCore;
using OnlineClassManagement.Models;

namespace OnlineClassManagement.Data
{
    /// <summary>
    /// DbContext cho ứng dụng quản lý lớp học
    /// </summary>
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
                entity.HasIndex(e => e.ClassCode).IsUnique();
                entity.Property(e => e.ClassName).IsRequired();
                entity.Property(e => e.ClassCode).IsRequired();
                entity.Property(e => e.Semester).IsRequired();
                entity.Property(e => e.AcademicYear).IsRequired();
                
                // Foreign key relationship
                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TaughtClasses)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình ClassEnrollment
            modelBuilder.Entity<ClassEnrollment>(entity =>
            {
                entity.HasKey(e => e.EnrollmentId);
                
                // Foreign key relationships
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: một học sinh chỉ có thể đăng ký một lớp một lần
                entity.HasIndex(e => new { e.ClassId, e.StudentId }).IsUnique();
            });

            // Cấu hình Assignment
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.DueDate).IsRequired();
                entity.Property(e => e.MaxScore).HasPrecision(5, 2);
                
                // Foreign key relationship
                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình Submission
            modelBuilder.Entity<Submission>(entity =>
            {
                entity.Property(e => e.Score).HasPrecision(5, 2);
                
                // Foreign key relationships
                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint: một học sinh chỉ có thể nộp một bài tập một lần
                entity.HasIndex(e => new { e.AssignmentId, e.StudentId }).IsUnique();
            });

            // Cấu hình CourseMaterial
            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.FileUrl).IsRequired();
                entity.Property(e => e.OriginalFileName).IsRequired();
                entity.Property(e => e.FileType).IsRequired();
                entity.Property(e => e.FileSize).IsRequired();
                
                // Foreign key relationships
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
                
                // Foreign key relationships
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