using EmployeeAdminPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Model.Entities;
using Models.Models.Entities;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeTimeLog> EmployeeTimeLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee-nin Boss ilə əlaqəsi (self-referencing)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Boss)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.BossId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Cascade delete qarşısını alır

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.WorkSchedule)
                .WithMany(w => w.Employees)
                .HasForeignKey(e => e.WorkScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeTimeLog>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.TimeLogs)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Yeni Permission və Employee arasındakı əlaqə
            modelBuilder.Entity<Permission>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permission>()
                .HasOne(l => l.Boss)
                .WithMany()
                .HasForeignKey(l => l.BossId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Work Schedules
            modelBuilder.Entity<WorkSchedule>().HasData(
                new WorkSchedule { Id = Guid.Parse("99999999-9999-9999-9999-999999999991"), Name = "8-17", Description = "Standard 8:00-17:00 work schedule", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0), RequiredWorkHours = 8, MinimumWorkMinutes = 480, MaxLatenessMinutes = 15, IsActive = true },
                new WorkSchedule { Id = Guid.Parse("99999999-9999-9999-9999-999999999992"), Name = "9-18", Description = "Standard 9:00-18:00 work schedule", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0), RequiredWorkHours = 8, MinimumWorkMinutes = 480, MaxLatenessMinutes = 15, IsActive = true },
                new WorkSchedule { Id = Guid.Parse("99999999-9999-9999-9999-999999999993"), Name = "9-14", Description = "Morning shift 9:00-14:00", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(14, 0, 0), RequiredWorkHours = 5, MinimumWorkMinutes = 300, MaxLatenessMinutes = 10, IsActive = true },
                new WorkSchedule { Id = Guid.Parse("99999999-9999-9999-9999-999999999994"), Name = "14-18", Description = "Afternoon shift 14:00-18:00", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(18, 0, 0), RequiredWorkHours = 4, MinimumWorkMinutes = 240, MaxLatenessMinutes = 10, IsActive = true }
            );

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Boss", Description = "Company Boss - Full Access" },
                new Role { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Boss-IT", Description = "IT Department Boss" },
                new Role { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Boss-Marketing", Description = "Marketing Department Boss" },
                new Role { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Boss-Finance", Description = "Finance Department Boss" },
                new Role { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "Boss-HR", Description = "HR Department Boss" },
                new Role { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Boss-Sales", Description = "Sales Department Boss" },
                new Role { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Boss-Operations", Description = "Operations Department Boss" },
                new Role { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Employee", Description = "Regular Employee" }
            );

            // Seed Admin User
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminRoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = adminId,
                Username = "Admin",
                Email = "admin@company.com",
                RoleId = adminRoleId,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Phone = "000000000",
                Salary = 0,
                WorkScheduleId = null,
                BossId = null,
                // "Admin123!" ŞİFRƏSİNİN STATİK VƏ ƏVVƏLCƏDƏN YARADILMIŞ HASH-İ
                PasswordHash = "AQAAAAIAAYagAAAAEI/1Yw/p4Jlv//3+A9y5St7jH8Dkdi3QhBYJ13d5u94aTeB3pY/dnmx3M9pGLi+D8Q=="
            });
        }
    }
}