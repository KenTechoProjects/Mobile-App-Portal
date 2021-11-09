using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YellowStone.Models;

namespace YellowStone.Repository
{
    public class FBNMDashboardContext : IdentityDbContext<User>
    {

        public FBNMDashboardContext(DbContextOptions<FBNMDashboardContext> options) : base(options)
        {

        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<OnboardingRequest> OnboardingRequests { get; set; }
        public virtual DbSet<CustomerRequest> CustomerRequests { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<LimitRequest> Limits { get; set; }
        public virtual DbSet<DocumentReviewRequest> DocumentReviewRequests { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RolePermission>()
            .HasKey(bc => new { bc.RoleId, bc.PermissionId });
            modelBuilder.Entity<RolePermission>()
                .HasOne(bc => bc.Role)
                .WithMany(b => b.RolePermissions)
                .HasForeignKey(bc => bc.RoleId);
            modelBuilder.Entity<RolePermission>()
                .HasOne(bc => bc.Permission)
                .WithMany(c => c.RolePermissions)
                .HasForeignKey(bc => bc.PermissionId);

            modelBuilder.Entity<DocumentReviewRequest>().HasKey(x => new { x.PhoneNumber, x.DocumentType });
        }



    }
}
