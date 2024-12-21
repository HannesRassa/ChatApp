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
        int GlobaGroupInex = 1;
        for (int roundIndex = 0; roundIndex < newGame.Rounds; roundIndex++)
        {
            var round = new Round
            {
                RoundNumber = roundIndex + 1,
                Groups = new List<Group>()
            };

            // Shuffle players and divide them into groups
            var shuffledPlayers = newGame.Players.OrderBy(_ => rnd.Next()).ToList();

            for (int groupIndex = 0; groupIndex < shuffledPlayers.Count; groupIndex += newGame.PlayersPerGroup)
            {
                var groupPlayers = shuffledPlayers.Skip(groupIndex).Take(newGame.PlayersPerGroup).ToList();
                var randomQuestion = allQuestions[rnd.Next(allQuestions.Count)];

                var group = new Group
                {
                    GroupNumber = GlobaGroupInex++,
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
        var playersPoints = newGame.PlayersPoints;
        foreach (var player in newGame.Players){
            playersPoints[player.Username] = 0;
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
    // GET ALL PLAYERS
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
     //FIND GROUP BY ROUND NUMBER AND PLAYER ID
    public async Task<int?> FindGroupNumberByGameIdAndRoundNumberAndPlayerId(int gameId, int roundNumber, int playerId)
        {
            if (gameId <= 0 || roundNumber <= 0 || playerId <= 0)
            {
                throw new ArgumentException("Invalid game ID, round number, or player ID.");
            }

            // Retrieve the game by GameId
            var game = await context.Games
                .Include(g => g.GameRounds)
                    .ThenInclude(r => r.Groups)
                        .ThenInclude(gr => gr.Players)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                return null; // No matching game found
            }

            // Find the round by RoundNumber
            var round = game.GameRounds.FirstOrDefault(r => r.RoundNumber == roundNumber);

            if (round == null)
            {
                return null; // No matching round found
            }

            // Find the group in the round containing the player
            var group = round.Groups.FirstOrDefault(g => g.Players.Any(p => p.Id == playerId));

            // Return the group number or null if no group found
            return group?.GroupNumber;
        }


    //FIND GROUP BY ROUND NUMBER AND PLAYER ID
    public async Task<(int? RoundId, int? GroupId)> FindRoundAndGroupByGameIdAndPlayerId(int gameId, int playerId)
    {
        if (gameId <= 0 || playerId <= 0)
        {
            throw new ArgumentException("Invalid game ID or player ID.");
        }

        // Fetch the game and include its rounds, groups, and players
        var game = await context.Games
            .Include(g => g.GameRounds) // Include rounds in the game
                .ThenInclude(r => r.Groups) // Include groups in each round
                    .ThenInclude(gr => gr.Players) // Include players in each group
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            return (null, null);
        }

        // Search for the round and group that contains the player
        foreach (var round in game.GameRounds)
        {
            var group = round.Groups.FirstOrDefault(g => g.Players.Any(p => p.Id == playerId));
            if (group != null)
            {
                // Return the round ID and group ID where the player was found
                return (round.Id, group.Id);
            }
        }

        // If no match is found, return null
        return (null, null);
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
    public async Task<Game> GetGameByPlayerId(int playerId)
    {
        return await context.Games
                        .Include(g => g.Players) 
                        .Where(g => g.Players.Any(p => p.Id == playerId)) 
                        .OrderByDescending(g => g.Id) 
                        .FirstOrDefaultAsync();
    }
}

