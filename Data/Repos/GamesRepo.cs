using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GamesRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Game> SaveGameToDb(Game game)
    {
        // Mark existing players as unchanged
        foreach (var player in game.Players)
        {
            context.Entry(player).State = EntityState.Unchanged;
        }

        // Mark existing questions as unchanged
        foreach (var round in game.GameRounds)
        {
            foreach (var group in round.Groups)
            {
                context.Entry(group.Question).State = EntityState.Unchanged;
            }
        }

        context.Add(game);
        await context.SaveChangesAsync();
        return game;
    }

    public async Task<Game?> AddRoundToGame(int gameId, Round newRound)
    {
        var game = await context.Games
            .Include(g => g.GameRounds)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        
        if (game == null) return null;

        // Mark related entities as added
        foreach (var group in newRound.Groups)
        {
            context.Entry(group.Question).State = EntityState.Unchanged;
            foreach (var answer in group.Answers)
            {
                context.Entry(answer).State = EntityState.Added;
            }
            context.Entry(group).State = EntityState.Added;
        }

        context.Entry(newRound).State = EntityState.Added;

        // Add the new round to the game
        game.GameRounds.Add(newRound);

        // Update the game
        await context.SaveChangesAsync();
        return game;
    }


    //READ
    public async Task<List<Game>> GetAllGames() => await context.Games.ToListAsync();

    public async Task<Game?> GetGameById(int id)
    {
        return await context.Games
            .Include(g => g.Players)
            .Include(g => g.GameRounds)
                .ThenInclude(r => r.Groups)
                    .ThenInclude(g => g.Players)
            .Include(g => g.GameRounds)
                .ThenInclude(r => r.Groups)
                    .ThenInclude(g => g.Question)
            .Include(g => g.GameRounds)
                .ThenInclude(r => r.Groups)
                    .ThenInclude(g => g.Answers)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

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

