using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Lobby> Lobbies { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Round> Rounds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
         modelBuilder.Entity<Question>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Question>().HasData(
            new Question
            {
                Id = 1,
                QuestionText = "Text1"                
            },
            new Question
            {
                Id = 2,
                QuestionText = "Text2"
            }
            );

        modelBuilder.Entity<Player>().Property(x => x.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Player>().HasData(
            new Player
            {
                Id = 1,
                Username = "Player1",
                Password = "1234"
            },
            new Player
            {
                Id = 2,
                Username = "Player2",
                Password = "0000"
            }
            );
    }

}
