using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Villa> Villas { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<VillaServices> VillaServices { get; set; }
    public DbSet<BookingProcess> BookingProcesses { get; set; }
    public DbSet<VillaImage> VillaImages { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Services> Services { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define primary keys and relationships
        builder.Entity<VillaServices>()
            .HasKey(vs => new { vs.VillaId, vs.ServiceId });

        builder.Entity<VillaImage>()
            .HasKey(vi => vi.Id);

        builder.Entity<Location>()
        .HasMany(l => l.Villas)
        .WithOne(v => v.Location)
        .HasForeignKey(v => v.LocationId);

        builder.Entity<Villa>()
            .HasMany(v => v.VillaImages)
            .WithOne(vi => vi.Villa)
            .HasForeignKey(vi => vi.VillaId);

        builder.Entity<Booking>()
            .HasOne(b => b.Villa)
            .WithMany(v => v.Bookings)
            .HasForeignKey(b => b.VillaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .Property(b => b.VillaId)
            .HasColumnName("VillaId");

        builder.Entity<Wishlist>()
        .HasOne(w => w.User) 
        .WithMany(u => u.Wishlists) 
        .HasForeignKey(w => w.UserId)
        .OnDelete(DeleteBehavior.Cascade); 

        // Cấu hình quan hệ giữa Wishlist và Villa
        builder.Entity<Wishlist>()
            .HasOne(w => w.Villa)
            .WithMany(v => v.Wishlists) 
            .HasForeignKey(w => w.VillaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
        .HasOne(r => r.Villa)
        .WithMany(v => v.Reviews)
        .HasForeignKey(r => r.VillaId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany() // Assuming User has a collection of Bookings
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // Restrict delete on User

        builder.Entity<BookingProcess>()
            .HasOne(bp => bp.Booking)
            .WithMany(b => b.BookingProcesses)
            .HasForeignKey(bp => bp.BookingId)
            .OnDelete(DeleteBehavior.NoAction);  // No action on delete for Booking -> BookingProcess

        builder.Entity<BookingProcess>()
            .HasOne(bp => bp.User)
            .WithMany() // Assuming User has a collection of BookingProcesses
            .HasForeignKey(bp => bp.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // Restrict delete on User from BookingProcess
    }



    // Seed roles method
    private void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole { Name = Role.Role_SuperAdmin, NormalizedName = Role.Role_SuperAdmin.ToUpper() },
                new IdentityRole { Name = Role.Role_Admin, NormalizedName = Role.Role_Admin.ToUpper() },
                new IdentityRole { Name = Role.Role_Customer, NormalizedName = Role.Role_Customer.ToUpper() },
                new IdentityRole { Name = Role.Role_Manager, NormalizedName = Role.Role_Manager.ToUpper() }
            );
    }
}
