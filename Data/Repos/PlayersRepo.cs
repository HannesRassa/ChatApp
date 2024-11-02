using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class PlayersRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Player> SavePlayerToDb(Player player)
    {
        context.Add(player);
        await context.SaveChangesAsync();
        return player;
    }

    //READ
    public async Task<List<Player>> GetAllPlayers() => await context.Players.ToListAsync();

    public async Task<Player?> GetPlayerById(int id) => await context.Players.FindAsync(id);
    public async Task<bool> PlayerExistsInDb(int id) => await context.Players.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdatePlayer(int id, Player player)
    {
        bool isIdsMatch = id == player.Id;
        bool playerExists = await PlayerExistsInDb(id);
        if (!isIdsMatch || !playerExists) return false;
        context.Update(player);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeletePlayerById(int id)
    {
        Player? playerInDb = await GetPlayerById(id);
        if (playerInDb == null) return false;
        context.Remove(playerInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
    public async Task<Player?> Authenticate(string username, string password)
    {
        return await context.Players
            .FirstOrDefaultAsync(p => p.Username == username && p.Password == password);
    }
    
}

