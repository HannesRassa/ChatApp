using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class PlayerRepo(DataContext context) {
    private readonly DataContext context = context;
    public async Task<List<Player>> GetAllPlayers() => await context.Players.ToListAsync();
}

