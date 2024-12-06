using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GamesRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Game> SaveGameToDb(Game newGame)
    {
        // Ensure enough players and questions are available
        if (!newGame.Players.Any())
            throw new InvalidOperationException("Players list cannot be empty.");

        var allQuestions = await context.Questions.ToListAsync();
        if (!allQuestions.Any())
            throw new InvalidOperationException("No questions available to assign.");

        var rnd = new Random();

        // Create rounds
        for (int roundIndex = 0; roundIndex < newGame.Rounds; roundIndex++)
        {
            var round = new Round { Groups = new List<Group>() };

            // Shuffle players and divide them into groups
            var shuffledPlayers = newGame.Players.OrderBy(_ => rnd.Next()).ToList();

            for (int i = 0; i < shuffledPlayers.Count; i += newGame.PlayersPerGroup)
            {
                var groupPlayers = shuffledPlayers.Skip(i).Take(newGame.PlayersPerGroup).ToList();
                var randomQuestion = allQuestions[rnd.Next(allQuestions.Count)];

                var group = new Group
                {
                    Players = groupPlayers,
                    Question = randomQuestion,
                    Answers = new List<Answer>() // Answers will be filled during gameplay
                };

                round.Groups.Add(group);
            }

            newGame.GameRounds.Add(round);
        }

        // Mark existing players as unchanged
        foreach (var player in newGame.Players)
        {
            context.Entry(player).State = EntityState.Unchanged;
        }

        // Attach questions to avoid duplication
        foreach (var round in newGame.GameRounds)
        {
            foreach (var group in round.Groups)
            {
                context.Entry(group.Question).State = EntityState.Unchanged;
            }
        }

        // Save the game to the database
        context.Add(newGame);
        await context.SaveChangesAsync();

        return newGame;
    }

    public async Task<Game?> AddRoundToGame(int gameId, Round newRound)
    {
        // Retrieve the game with its rounds
        var game = await context.Games
            .Include(g => g.GameRounds)
                .ThenInclude(r => r.Groups)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return null;

        // Attach related entities to the context to prevent duplication
        foreach (var group in newRound.Groups)
        {
            // Attach the question
            if (group.Question != null)
            {
                context.Entry(group.Question).State = EntityState.Unchanged;
            }

            // Attach players in the group
            foreach (var player in group.Players)
            {
                context.Entry(player).State = EntityState.Unchanged;
            }

            // Attach answers
            foreach (var answer in group.Answers)
            {
                context.Entry(answer).State = EntityState.Added;
            }

            // Mark group as added
            context.Entry(group).State = EntityState.Added;
        }

        // Add the new round to the context
        context.Entry(newRound).State = EntityState.Added;

        // Add the round to the game
        game.GameRounds.Add(newRound);

        // Save changes to the database
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
    public async Task<List<Player>> GetAllPlayersInGame(int gameId)
    {
        var game = await context.Games
            .Include(g => g.GameRounds)
                .ThenInclude(r => r.Groups)
                    .ThenInclude(g => g.Players)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return new List<Player>();

        // Extract all players from the groups
        var players = game.GameRounds
            .SelectMany(round => round.Groups)
            .SelectMany(group => group.Players)
            .Distinct()
            .ToList();

        return players;
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

