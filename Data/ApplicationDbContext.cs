using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using S_DES_project4.Models;

namespace S_DES_project4.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Friend>(e =>
            {
                e.HasKey(f => f.Id);

                // First user
                e.HasOne(f => f.UserA)
                 .WithMany() // IdentityUser has no navigation property
                 .HasForeignKey(f => f.UserAId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Second user
                e.HasOne(f => f.UserB)
                 .WithMany()
                 .HasForeignKey(f => f.UserBId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Optional: avoid duplicates between two users
                e.HasIndex(f => new { f.UserAId, f.UserBId }).IsUnique();

                // Default values
                e.Property(f => f.IsBlocked).HasDefaultValue(false);
                e.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Message table configuration
            builder.Entity<Message>(e =>
            {
                e.HasKey(m => m.Id);

                // Sender
                e.HasOne(m => m.Sender)
                 .WithMany()
                 .HasForeignKey(m => m.SenderId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Receiver
                e.HasOne(m => m.Receiver)
                 .WithMany()
                 .HasForeignKey(m => m.ReceiverId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Friend relationship for key
                e.HasOne(m => m.Friend)
                 .WithMany()
                 .HasForeignKey(m => m.FriendId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.Property(m => m.CipherText)
                 .IsRequired();
            });
        }

        public DbSet<Friend> Friends { get; set; }
        public DbSet<Message> Messages { get; set; }

    }
}