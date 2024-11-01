using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GameRoomsRepo(DataContext context) {
    private readonly DataContext context = context;

    //CREATE
    public async Task<GameRoom> SaveGameRoomToDb(GameRoom gameRoom)
    {
        context.Add(gameRoom);
        await context.SaveChangesAsync();
        return gameRoom;
    }

    //READ
    public async Task<List<GameRoom>> GetAllGameRooms(Player? admin = null)
    {
        IQueryable<GameRoom> query = context.GameRooms.AsQueryable();
        if (admin is not null) query = query.Where(x => x.Admin == admin);
        return await query.ToListAsync();
    }

    public async Task<GameRoom?> GetGameRoomById(int id) => await context.GameRooms.FindAsync(id);
    public async Task<bool> GameRoomExistsInDb(int id) => await context.GameRooms.AnyAsync(x => x.Id == id);

    //UPDATE
    public async Task<bool> UpdateGameRoom(int id, GameRoom gameRoom)
    {
        bool isIdsMatch = id == gameRoom.Id;
        bool gameRoomExists = await GameRoomExistsInDb(id);
        if (!isIdsMatch || !gameRoomExists) return false;
        context.Update(gameRoom);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }

    //DELETE
    public async Task<bool> DeleteGameRoomById(int id)
    {
        GameRoom? gameRoomInDb = await GetGameRoomById(id);
        if (gameRoomInDb == null) return false;
        context.Remove(gameRoomInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
   
}
