using BackEnd.Models.Classes;
using BackEnd.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos
{
    public class GameRepo
    {
        private readonly DataContext _context;
        
        public GameRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<GameRoom> StartNewGame(int adminId, List<int> playerIds)
        {
            var players = await _context.Players.Where(p => playerIds.Contains(p.PlayerID)).ToListAsync();
            var questions = await _context.Questions.ToListAsync();

            var newGameRoom = new GameRoom
            {
                RoomCode = GenerateRoomCode(),
                MaxCapacity = players.Count,
                AdminID = adminId,
                Players = players,
                Questions = questions,
                CurrentQuestion = 0,
                GameTimer = 0, 
                Status = GameStatus.Waiting
            };

            _context.GameRooms.Add(newGameRoom);
            await _context.SaveChangesAsync();

            return newGameRoom;
        }

        public async Task<Question?> GetCurrentQuestion(int roomCode)
        {
            var gameRoom = await _context.GameRooms
                .Include(gr => gr.Questions)
                .FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);

            if (gameRoom == null || gameRoom.CurrentQuestion >= gameRoom.Questions.Count)
                return null;

            return gameRoom.Questions[gameRoom.CurrentQuestion];
        }

        public async Task<bool> GoToNextQuestion(int roomCode)
        {
            var gameRoom = await _context.GameRooms.FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);
            if (gameRoom == null || gameRoom.CurrentQuestion >= gameRoom.Questions.Count)
                return false;

            gameRoom.CurrentQuestion++;
            _context.GameRooms.Update(gameRoom);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatePlayerScore(int roomCode, int playerId, bool isCorrect)
        {
            var gameRoom = await _context.GameRooms
                .Include(gr => gr.Players)
                .FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);

            if (gameRoom == null)
                return false;

            var player = gameRoom.Players.FirstOrDefault(p => p.PlayerID == playerId);
            if (player == null)
                return false;

            // Обновляем очки игрока
            player.Score += isCorrect ? 10 : 0;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return true;
        }

        // Randomly generated RoomCode
        private int GenerateRoomCode()
        {
            Random rand = new Random();
            return rand.Next(1000, 9999); 
        }

        public async Task<GameRoom?> GetGameRoomByCode(int roomCode)
        {
            return await _context.GameRooms
                .FirstOrDefaultAsync(gr => gr.RoomCode == roomCode);
        }
    }
}
