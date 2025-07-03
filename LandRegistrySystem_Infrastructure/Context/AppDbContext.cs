using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<FarmBoundaries> FarmBoundaries { get; set; }
        public DbSet<City> Citites { get; set; }
        public DbSet<FarmDocument> FarmDocuments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<OrganizationInfo> OrganizationInfo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.City)
                .WithMany(c => c.Projects)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Farm>()
                .HasOne(f => f.Project)
                .WithMany(p => p.Farms)
                .HasForeignKey(f => f.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Farm>()
                .HasOne(f => f.Boundaries)
                .WithOne(b => b.Farm)
                .HasForeignKey<FarmBoundaries>(b => b.FarmId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Role>().ToTable("Roles"); // Explicitly set the table name

            modelBuilder.Entity<City>()
        .HasIndex(c => c.CityNumber)
        .IsUnique();

            modelBuilder.Entity<Project>()
        .HasIndex(c => c.ProjectNumber)
        .IsUnique();

            modelBuilder.Entity<Farm>()
        .HasIndex(c => c.FarmNumber)
        .IsUnique();

        }

        public static async Task SeedRoles(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
            {
                new Role { Name = "Super Admin" },
                 new Role { Name = "Admin" },
                new Role { Name = "User" }
            };

                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedDefaultOwner(AppDbContext context)
        {
            const int defaultOwnerId = 1;
            if (!context.Owners.Any(o => o.Id == defaultOwnerId))
            {
                var owner = new Owner
                {
                    Id = defaultOwnerId, 
                    FullName = "المالك الافتراضي",
                    IsProtected = true,
                };
                context.Owners.Add(owner);
                await context.SaveChangesAsync();
            }
        }

        public static async Task EnsureSuperAdminCreated(AppDbContext context, IAuthRepository authRepository)
        {
            if (!context.Users.Any())
            {
                // Ensure the roles are seeded
                await SeedRoles(context);

                // Find the Super Admin role
                var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Super Admin");
                if (superAdminRole == null)
                {
                    throw new Exception("Super Admin role not found.");
                }

                // Create the super admin user
                var superAdminUser = new User
                {
                    Username = "superadmin",
                    FullName = "Super Admin",
                    Email = "superadmin@example.com",
                    RoleId = superAdminRole.Id,
                    Adress = "Admin Address",
                    Phone = "1234567890",
                    IsActive = true
                };

                // Register the super admin user with a password
                await authRepository.Register(superAdminUser, "superadmin");

                await context.SaveChangesAsync();
            }


        }


    }
}
