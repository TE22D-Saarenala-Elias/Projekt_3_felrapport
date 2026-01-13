using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketStatus> TicketStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Role).HasDefaultValue("User");
        });

        // Configure TicketStatus
        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.StatusID);
            entity.HasIndex(e => e.StatusName).IsUnique();
        });

        // Configure Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketID);
            
            entity.HasOne(e => e.Creator)
                .WithMany(u => u.Tickets)
                .HasForeignKey(e => e.CreatorUserID)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(e => e.StatusID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed default ticket statuses
        modelBuilder.Entity<TicketStatus>().HasData(
            new TicketStatus { StatusID = 1, StatusName = "Ny" },
            new TicketStatus { StatusID = 2, StatusName = "Pågående" },
            new TicketStatus { StatusID = 3, StatusName = "Löst" }
        );
    }
}
