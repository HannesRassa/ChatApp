using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;


namespace BackEnd.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
public DbSet<Game> Games { get; set; }
public DbSet<Player> Players { get; set; }
public DbSet<Round> Rounds { get; set; }
public DbSet<Group> Groups { get; set; }
public DbSet<Lobby> Lobbies { get; set; }
public DbSet<Question> Questions { get; set; }
public DbSet<Answer> Answers{ get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Game>()
        .HasMany(g => g.Players)
        .WithMany(); // Configure as needed

    modelBuilder.Entity<Game>()
        .HasMany(g => g.GameRounds)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Round>()
        .HasMany(r => r.Groups)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Group>()
        .HasMany(g => g.Players)
        .WithMany(); // Configure as needed

    modelBuilder.Entity<Group>()
        .HasOne(g => g.Question)
        .WithMany()
        .OnDelete(DeleteBehavior.SetNull);
}

}
