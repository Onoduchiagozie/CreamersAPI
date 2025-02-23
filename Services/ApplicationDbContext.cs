using AdonisAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdonisAPI.Services;

public class ApplicationDbContext:IdentityDbContext<GymBro>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Required for Identity tables
        modelBuilder.Entity<GymBro>().ToTable("GymBro"); // Renaming AspNetUsers to Users
        modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

       modelBuilder.Entity<Favourite>()
            .HasOne(f => f.GymBro)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Delete favorites if user is deleted

        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.Exercise)
            .WithMany()
            .HasForeignKey(f => f.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
   
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Favourite> Favorites { get; set; }
}