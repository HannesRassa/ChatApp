using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        // Attach Admin if it's not tracked
        if (!context.Players.Local.Any(p => p.Id == lobby.AdminId))
        {
            context.Players.Attach(lobby.Admin);
        }

        // Ensure Players are tracked
        foreach (var player in lobby.Players)
        {
            if (!context.Players.Local.Any(p => p.Id == player.Id))
            {
                context.Players.Attach(player);
            }
        }

        // Add and save
        context.Lobbies.Add(lobby);
        await context.SaveChangesAsync();
        return lobby;
    }

    // READ
    public async Task<List<Lobby>> GetAllLobbies(Player? admin = null)
    {
        IQueryable<Lobby> query = context.Lobbies.Include(gr => gr.Players);
        if (admin != null)
        {
            query = query.Where(x => x.Admin == admin);
        }
        return await query.ToListAsync();
    }

    public async Task<Lobby?> GetLobbyById(int id)
    {
        return await context.Lobbies.Include(gr => gr.Players).FirstOrDefaultAsync(gr => gr.Id == id);
    }

    public async Task<Lobby?> GetLobbyByCode(int roomCode)
    {
        return await context.Lobbies.Include(gr => gr.Players).FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);
    }

    public async Task<Player?> GetPlayerById(int id)
    {
        return await context.Players.FindAsync(id);
    }

    // UPDATE - Add a player to a game room
    public async Task<bool> AddPlayerToLobby(int roomCode, Player player)
    {
        var lobby = await GetLobbyByCode(roomCode);
        if (lobby == null)
        {
            return false;
        }

        // Check if the player is already in the game room to avoid duplicates
        if (lobby.Players.Any(p => p.Id == player.Id))
        {
            return false;
        }

        lobby.Players.Add(player);
        context.Update(lobby);
        await context.SaveChangesAsync();
        return true;
    }

    // UPDATE - Full game room update
    public async Task<bool> UpdateLobby(int id, Lobby lobby)
    {
        bool isIdsMatch = id == lobby.Id;
        bool lobbyExists = await LobbyExistsInDb(id);
        if (!isIdsMatch || !lobbyExists)
        {
            return false;
        }

        context.Update(lobby);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    public async Task<bool> UpdateGameStatus(int id, int status)
    {
        // Find the game by its ID
        var lobby = await context.Lobbies.FindAsync(id);
        if (lobby == null)
        {
            // Return false if the game is not found
            return false;
        }

        // Update the status
        lobby.GameStatus = status;

        // Save changes to the database
        try
        {
            await context.SaveChangesAsync();
            return true; // Return true if the update is successful
        }
        catch
        {
            return false; // Return false if there is an error
        }
    }


    // Check if game room exists by ID
    public async Task<bool> LobbyExistsInDb(int id)
    {
        return await context.Lobbies.AnyAsync(x => x.Id == id);
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
    public async Task<Lobby> GetLobbyByPlayerId(int playerId)
    {
        return await context.Lobbies
                            .Include(gr => gr.Players) // Include players for the game room
                            .FirstOrDefaultAsync(gr => gr.Players.Any(p => p.Id == playerId))
            ?? new Lobby(); // Return a default instance of Lobby if no match is found
    }

}

