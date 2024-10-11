using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class PlayerRepo(DataContext context)
{
    private readonly DataContext context = context;
    public async Task<List<Player>> GetAllPlayers() => await context.Players.ToListAsync();

    public async Task<Player?> GetPlayerById(int id)
    {
        return await context.Players.FirstOrDefaultAsync(p => p.PlayerID == id);
    }
    public async Task AddPlayer(Player player)
    {
        context.Players.Add(player);
        await context.SaveChangesAsync();
    }
    public async Task UpdatePlayer(Player player)
    {
        context.Players.Update(player);
        await context.SaveChangesAsync();
    }

}

