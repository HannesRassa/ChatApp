using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Round> Rounds { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Lobby> Lobbies { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Answer> Answers { get; set; } = null!;
    public DbSet<PlayerPoint> PlayerPoints { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Game -> Rounds
        modelBuilder.Entity<Game>()
            .HasMany(g => g.GameRounds)
            .WithOne(r => r.Game)
            .HasForeignKey(r => r.GameId)
            .OnDelete(DeleteBehavior.Cascade);


        // Round -> Groups
        modelBuilder.Entity<Round>()
            .HasMany(r => r.Groups)
            .WithOne()
            .HasForeignKey(g => g.RoundId)
            .OnDelete(DeleteBehavior.Cascade);

        // Group -> Answers
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Answers)
            .WithOne()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Group -> Question (optional)
        modelBuilder.Entity<Group>()
            .HasOne(g => g.Question)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);


        // Group -> Players (Many-to-Many)
        modelBuilder.Entity<Group>()
            .HasMany(g => g.Players)
            .WithMany();


        // Lobby -> Players (Many-to-Many)
        modelBuilder.Entity<Lobby>()
            .HasOne(l => l.Admin)
            .WithMany()
            .HasForeignKey(l => l.AdminId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Lobby>()
            .HasMany(l => l.Players)
            .WithMany();

        // Game -> Players (Many-to-Many)
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Players)
            .WithMany()
            .UsingEntity(j => j.ToTable("GamePlayers"));

        // PlayerPoint -> Player
        modelBuilder.Entity<PlayerPoint>()
            .HasOne<Player>()
            .WithMany()
            .HasForeignKey(pp => pp.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // PlayerPoint -> Game
        modelBuilder.Entity<PlayerPoint>()
            .HasOne<Game>()
            .WithMany()
            .HasForeignKey(pp => pp.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}