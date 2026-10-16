using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
namespace RaceDay.API.Data;

public class RaceDayDbContext : DbContext
{
    public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Weather> Weather => Set<Weather>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Organiser', 'Participant')"));

        modelBuilder.Entity<Event>().Property(e => e.Status).HasDefaultValue("Upcoming");
        modelBuilder.Entity<Event>().Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Event>().ToTable(t => t.HasCheckConstraint("CK_Events_Status", "[Status] IN ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled')"));
        modelBuilder.Entity<Event>().HasOne(e => e.Organiser).WithMany(u => u.OrganisedEvents).HasForeignKey(e => e.OrganiserID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Events_Organiser");

        modelBuilder.Entity<Category>().HasIndex(c => new { c.EventID, c.CategoryName }).IsUnique().HasDatabaseName("UQ_Category_Event_Name");
        modelBuilder.Entity<Category>().Property(c => c.DistanceKm).HasPrecision(6, 2);
        modelBuilder.Entity<Category>().Property(c => c.EntryFee).HasPrecision(10, 2);
        modelBuilder.Entity<Category>().ToTable(t => { t.HasCheckConstraint("CK_Categories_Distance", "[DistanceKm] > 0"); t.HasCheckConstraint("CK_Categories_EntryFee", "[EntryFee] >= 0"); t.HasCheckConstraint("CK_Categories_MaxParticipants", "[MaximumParticipants] > 0"); });
        modelBuilder.Entity<Category>().HasOne(c => c.Event).WithMany(e => e.Categories).HasForeignKey(c => c.EventID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Categories_Event");

        modelBuilder.Entity<Enrolment>().Property(e => e.EnrolmentDate).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Enrolment>().Property(e => e.Status).HasDefaultValue("Active");
        modelBuilder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantID, e.CategoryID }).IsUnique().HasDatabaseName("UQ_Participant_Category");
        modelBuilder.Entity<Enrolment>().ToTable(t => t.HasCheckConstraint("CK_Enrolments_Status", "[Status] IN ('Active', 'Cancelled', 'Completed')"));
        modelBuilder.Entity<Enrolment>().HasOne(e => e.Participant).WithMany(u => u.Enrolments).HasForeignKey(e => e.ParticipantID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Enrolments_Participant");
        modelBuilder.Entity<Enrolment>().HasOne(e => e.Category).WithMany(c => c.Enrolments).HasForeignKey(e => e.CategoryID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Enrolments_Category");

        modelBuilder.Entity<Result>().HasIndex(r => r.EnrolmentID).IsUnique();
        modelBuilder.Entity<Result>().Property(r => r.ResultStatus).HasDefaultValue("Finished");
        modelBuilder.Entity<Result>().Property(r => r.RecordedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Result>().ToTable(t => { t.HasCheckConstraint("CK_Results_Position", "[Position] IS NULL OR [Position] > 0"); t.HasCheckConstraint("CK_Results_Status", "[ResultStatus] IN ('Finished', 'DNF', 'DNS', 'Disqualified')"); });
        modelBuilder.Entity<Result>().HasOne(r => r.Enrolment).WithOne(e => e.Result).HasForeignKey<Result>(r => r.EnrolmentID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Results_Enrolment");

        modelBuilder.Entity<Weather>().Property(w => w.Temperature).HasPrecision(5, 2);
        modelBuilder.Entity<Weather>().Property(w => w.WindSpeed).HasPrecision(5, 2);
        modelBuilder.Entity<Weather>().Property(w => w.RainProbability).HasPrecision(5, 2);
        modelBuilder.Entity<Weather>().Property(w => w.RecordedAt).HasDefaultValueSql("GETDATE()");
        modelBuilder.Entity<Weather>().ToTable(t => { t.HasCheckConstraint("CK_Weather_RainProbability", "[RainProbability] IS NULL OR [RainProbability] BETWEEN 0 AND 100"); t.HasCheckConstraint("CK_Weather_WindSpeed", "[WindSpeed] IS NULL OR [WindSpeed] >= 0"); });
        modelBuilder.Entity<Weather>().HasOne(w => w.Event).WithMany(e => e.WeatherRecords).HasForeignKey(w => w.EventID).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Weather_Event");
    }
}
