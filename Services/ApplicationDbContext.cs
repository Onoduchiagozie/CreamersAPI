using AdonisAPI.Models;
using AdonisAPI.Models.Order;
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
        
        
        modelBuilder.Entity<TreatCustomizationGroup>()
            .HasOne(g => g.Product)
            .WithMany(p => p.CustomizationGroups)
            .HasForeignKey(g => g.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TreatCustomizationOptions>()
            .HasOne(o => o.TreatCustomizationGroup)
            .WithMany(g => g.Options)
            .HasForeignKey(o => o.CustomizationGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
   
    public DbSet<TreatCustomizationGroup> CustomizationGroup { get; set; } = null!;
    public DbSet<TreatCustomizationOptions> CustomizationOptions { get; set; } = null!; 
    
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Favourite> Favorites { get; set; }
    
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
     
    
    public DbSet<Transaction> Transactions { get; set; }
    
 
    
    
}