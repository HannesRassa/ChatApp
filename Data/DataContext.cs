using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Player> Players { get; set; }
    public DbSet<GameRoom> GameRooms { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Question>().Property(x => x.QuestionID).ValueGeneratedOnAdd();
        modelBuilder.Entity<Question>().HasData(
            new Question
            {
                QuestionID = 1,
                QuestionText = "Text1",
                PossibleAnswers = new List<string> { "Answer1" },
                CorrectAnswer = "CorrectAns1",
                TimeLimit = 100
            },
                new Question
                {
                    QuestionID = 2,
                    QuestionText = "Text2",
                    PossibleAnswers = new List<string> { "Answer2" },
                    CorrectAnswer = "CorrectAns2",
                    TimeLimit = 200
                }

            );
        modelBuilder.Entity<Player>().Property(x => x.PlayerID).ValueGeneratedOnAdd();
        modelBuilder.Entity<Player>().HasData(
            new Player
            {
                PlayerID = 1,
                Username = "Player1",
                IsAdmin = false,
                Score = 100
            },
                new Player
                {
                    PlayerID = 2,
                    Username = "Player2",
                    IsAdmin = true,
                    Score = 200
                }
                );
    }

}
