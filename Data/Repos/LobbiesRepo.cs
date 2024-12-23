using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;

public class LobbiesRepo
{
    private readonly DataContext context;

    public LobbiesRepo(DataContext context)
    {
        this.context = context;
    }

    // CREATE
    public async Task<Lobby> SaveLobbyToDb(Lobby lobby)
    {
        context.Add(lobby);
        await context.SaveChangesAsync();
        return lobby;
    }

    // READ
    public async Task<List<Lobby>> GetAllLobbies() => await context.Lobbies.ToListAsync();
  
    public async Task<Lobby?> GetLobbyById(int id) => await context.Lobbies.FindAsync(id);

    public async Task<bool> LobbyExistsInDb(int id) => await context.Lobbies.AnyAsync(x => x.Id == id);
  
    // UPDATE    
    public async Task<bool> UpdateLobby(int id, Lobby lobby)
    {
        bool isIdsMatch = id == lobby.Id;
        if (!isIdsMatch)
            return false;
        
                // avoiding Players duplication
        lobby.Players = lobby.Players.GroupBy(x => x).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
        context.Update(lobby);
        int updatedRecordsCount = await context.SaveChangesAsync();
            return updatedRecordsCount == 1;
}
  
    // DELETE
    public async Task<bool> DeleteLobbyById(int id)
    {
        Lobby? lobbyInDb = await GetLobbyById(id);
        if (lobbyInDb == null) return false;
        context.Remove(lobbyInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}