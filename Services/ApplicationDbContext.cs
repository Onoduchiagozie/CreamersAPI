using AdonisAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdonisAPI.Services;

public class ApplicationDbContext:IdentityDbContext<CreamUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Required for Identity tables
        modelBuilder.Entity<CreamUser>().ToTable("CreamUser"); // Renaming AspNetUsers to Users
        modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.Product)
            .WithMany(u => u.FavouritedBy)
            .HasForeignKey(f => f.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.CreamUser)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.CreamUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Seller)
            .WithMany(u => u.Inventory)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
   
    public DbSet<Product> Products { get; set; }
    public DbSet<Favourite> Favorites { get; set; }
}