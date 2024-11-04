using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BackEnd.Data.Repos;

public class GameRoomsRepo
{
    private readonly DataContext context;

    public GameRoomsRepo(DataContext context)
    {
        this.context = context;
    }

    // CREATE
    public async Task<GameRoom> SaveGameRoomToDb(GameRoom gameRoom)
    {
        context.Add(gameRoom);
        await context.SaveChangesAsync();
        return gameRoom;
    }

    // READ
    public async Task<List<GameRoom>> GetAllGameRooms(Player? admin = null)
    {
        IQueryable<GameRoom> query = context.GameRooms.Include(gr => gr.Players);
        if (admin != null)
        {
            query = query.Where(x => x.Admin == admin);
        }
        return await query.ToListAsync();
    }

    public async Task<GameRoom?> GetGameRoomById(int id)
    {
        return await context.GameRooms.Include(gr => gr.Players).FirstOrDefaultAsync(gr => gr.Id == id);
    }

    public async Task<GameRoom?> GetGameRoomByCode(int roomCode)
    {
        return await context.GameRooms.Include(gr => gr.Players).FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);
    }

    public async Task<Player?> GetPlayerById(int id)
    {
        return await context.Players.FindAsync(id);
    }

    // UPDATE - Add a player to a game room
    public async Task<bool> AddPlayerToGameRoom(int roomCode, Player player)
    {
        var gameRoom = await GetGameRoomByCode(roomCode);
        if (gameRoom == null)
        {
            return false;
        }

        // Check if the player is already in the game room to avoid duplicates
        if (gameRoom.Players.Any(p => p.Id == player.Id))
        {
            return false;
        }

        gameRoom.Players.Add(player);
        context.Update(gameRoom);
        await context.SaveChangesAsync();
        return true;
    }

    // UPDATE - Full game room update
    public async Task<bool> UpdateGameRoom(int id, GameRoom gameRoom)
    {
        bool isIdsMatch = id == gameRoom.Id;
        bool gameRoomExists = await GameRoomExistsInDb(id);
        if (!isIdsMatch || !gameRoomExists)
        {
            return false;
        }

        context.Update(gameRoom);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }

    // Check if game room exists by ID
    public async Task<bool> GameRoomExistsInDb(int id)
    {
        return await context.GameRooms.AnyAsync(x => x.Id == id);
    }

    // DELETE
    public async Task<bool> DeleteGameRoomById(int id)
    {
        GameRoom? gameRoomInDb = await GetGameRoomById(id);
        if (gameRoomInDb == null) return false;
        context.Remove(gameRoomInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}
