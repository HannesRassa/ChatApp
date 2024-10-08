using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data;

public class DataContext(DbContextOptions options) : DbContext(options) {
    public DbSet<Player> Players { get; set; }
    public DbSet<GameRoom> GameRooms{ get; set; }
    public DbSet<Question> Questions { get; set; }
}