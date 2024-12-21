using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class PlayerPointsRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<PlayerPoint> SavePlayerPointToDb(PlayerPoint playerPoint)
    {
        context.Add(playerPoint);
        await context.SaveChangesAsync();
        return playerPoint;
    }

    //READ
    public async Task<List<PlayerPoint>> GetAllPlayerPoints() => await context.PlayerPoints.ToListAsync();

    public async Task<PlayerPoint?> GetPlayerPointById(int id) => await context.PlayerPoints.FindAsync(id);
    public async Task<bool> PlayerPointExistsInDb(int id) => await context.PlayerPoints.AnyAsync(x => x.Id == id);
    public async Task<PlayerPoint?> GetPlayerPointByPlayerIdAndGameId(int playerId, int gameId)
{
    return await context.PlayerPoints
        .FirstOrDefaultAsync(pp => pp.PlayerId == playerId && pp.GameId == gameId);
}

    //UPDATE
    public async Task<bool> UpdatePlayerPoint(int id, PlayerPoint playerPoint)
    {
        bool isIdsMatch = id == playerPoint.Id;
        bool playerPointExists = await PlayerPointExistsInDb(id);
        if (!isIdsMatch || !playerPointExists) return false;
        context.Update(playerPoint);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeletePlayerPointById(int id)
    {
        PlayerPoint? playerPointInDb = await GetPlayerPointById(id);
        if (playerPointInDb == null) return false;
        context.Remove(playerPointInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}
