using Microsoft.EntityFrameworkCore;

namespace CapstoneApi.Models
{
    public class CapstoneContext : DbContext
    {
        public CapstoneContext(DbContextOptions<CapstoneContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<PrimaryContact> PrimaryContacts { get; set; }
        public DbSet<Registrant> Registrants { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Registrations)
                .WithOne(r => r.Event)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PrimaryContact>()
                .HasMany(pc => pc.Registrations)
                .WithOne(r => r.PrimaryContact)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Registrant>()
                .HasMany(r => r.Registrations)
                .WithOne(reg => reg.Registrant)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Events)
                .WithOne(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}