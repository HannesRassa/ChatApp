using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GamesRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Game> SaveGameToDb(Game game)
    {
        context.Add(game);
        await context.SaveChangesAsync();
        return game;
    }

    //READ
    public async Task<List<Game>> GetAllGames() => await context.Games.ToListAsync();

    public async Task<Game?> GetGameById(int id) => await context.Games.FindAsync(id);
    public async Task<bool> GameExistsInDb(int id) => await context.Games.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdateGame(int id, Game game)
    {
        bool isIdsMatch = id == game.Id;
        bool gameExists = await GameExistsInDb(id);
        if (!isIdsMatch || !gameExists) return false;
        context.Update(game);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteGameById(int id)
    {
        Game? gameInDb = await GetGameById(id);
        if (gameInDb == null) return false;
        context.Remove(gameInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}

