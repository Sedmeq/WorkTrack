using EmployeeAdminPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role - Employee relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Role - Boss relationship (Self-referencing)
            //modelBuilder.Entity<Role>()
            //    .HasOne(r => r.Boss)
            //    .WithMany(e => e.ManagedRoles)
            //    .HasForeignKey(r => r.BossId)
            //    .OnDelete(DeleteBehavior.SetNull);

            // WorkSchedule - Employee relationship
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.WorkSchedule)
                .WithMany(w => w.Employees)
                .HasForeignKey(e => e.WorkScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            // EmployeeTimeLog - Employee relationship
            modelBuilder.Entity<EmployeeTimeLog>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.TimeLogs)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var workSchedules = new[]
            {
                new WorkSchedule
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999991"),
                    Name = "8-17",
                    Description = "Standard 8:00-17:00 work schedule",
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    RequiredWorkHours = 8,
                    MinimumWorkMinutes = 480,
                    MaxLatenessMinutes = 15,
                    //CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new WorkSchedule
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999992"),
                    Name = "9-18",
                    Description = "Standard 9:00-18:00 work schedule",
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    RequiredWorkHours = 8,
                    MinimumWorkMinutes = 480,
                    MaxLatenessMinutes = 15,
                    //CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new WorkSchedule
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999993"),
                    Name = "9-14",
                    Description = "Morning shift 9:00-14:00",
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    RequiredWorkHours = 5,
                    MinimumWorkMinutes = 300,
                    MaxLatenessMinutes = 10,
                    //CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new WorkSchedule
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999994"),
                    Name = "14-18",
                    Description = "Afternoon shift 14:00-18:00",
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    RequiredWorkHours = 4,
                    MinimumWorkMinutes = 240,
                    MaxLatenessMinutes = 10,
                    //CreatedAt = DateTime.Now,
                    IsActive = true
                }
            };

            // Sadələşdirilmiş role sistemi
            var roles = new[]
            {
                // Əsas Rəhbər Rolu
                new Role
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Boss",
                    Description = "Company Boss - Full Access",
                    //BossId = null // Main boss has no boss
                  //  CreatedAt = DateTime.Now
                },

                // IT Departament Rolları
                new Role
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Boss-IT",
                    Description = "IT Department Boss",
                    //BossId = null //Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                   // CreatedAt = DateTime.Now
                },

                // Marketing Departament Rolları
                new Role
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Name = "Boss-Marketing",
                    Description = "Marketing Department Boss",
                    //BossId = null //Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                   // CreatedAt = DateTime.Now
                },

                // Finance Departament Rolları
                new Role
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Name = "Boss-Finance",
                    Description = "Finance Department Boss",
                    //BossId = null //Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                   // CreatedAt = DateTime.Now
                },

                // HR Departament Rolları
                new Role
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    Name = "Boss-HR",
                    Description = "HR Department Boss",
                    //BossId =null // Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                   // CreatedAt = DateTime.Now
                },
                
                // Sales Departament Rolları
                new Role
                {
                    Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    Name = "Boss-Sales",
                    Description = "Sales Department Boss",
                    //BossId = null //Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                    //CreatedAt = DateTime.Now
                },

                // Operations Departament Rolları
                new Role
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Name = "Boss-Operations",
                    Description = "Operations Department Boss",
                    //BossId =null // Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                   // CreatedAt = DateTime.Now
                },
                
                // Tək Employee rolu - artıq fəqr departament employee-ləri yoxdur
                new Role
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Name = "Employee",
                    Description = "Regular Employee",
                    //BossId =null // Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") // Reports to main boss
                    //CreatedAt = DateTime.Now
                }
            };

            modelBuilder.Entity<WorkSchedule>().HasData(workSchedules);
            modelBuilder.Entity<Role>().HasData(roles);
        }
    }
}
