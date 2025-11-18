using AnswerUA.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnswerUA.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Address>()
        .HasOne(a => a.User)
        .WithMany(u => u.Addresses)
        .HasForeignKey("UserId")
        .OnDelete(DeleteBehavior.Cascade);

         builder.Entity<PaymentMethod>()
        .HasOne(a => a.User)
        .WithMany(u => u.PaymentMethods)
        .HasForeignKey("UserId")
        .OnDelete(DeleteBehavior.Cascade);
        
    }
}
