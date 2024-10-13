using BackEnd.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Classes
{
    public class Game
    {
        [Key]
        public int RoomCode { get; init; }           // Randomly generated
        public int AdminID { get; set; }             // ID of admin
        public List<Player> Players { get; set; } = new(); // List of players
        public List<Question> Questions { get; set; } = new(); // List of questions
        public int CurrentQuestionIndex { get; set; } = 0;     // Current question index
        public int GameTimer { get; set; }                   // Timer
        public GameStatus Status { get; set; } = GameStatus.Waiting; // Game Status

        public Dictionary<int, int> ScoreBoard { get; set; } = new(); // players score (PlayerID -> Score)

        public Game(int roomCode, int adminID, List<Player> players, List<Question> questions, int gameTimer)
        {
            RoomCode = roomCode;
            AdminID = adminID;
            Players = players;
            Questions = questions;
            GameTimer = gameTimer;

            // Инициализация счёта игроков
            foreach (var player in players)
            {
                ScoreBoard[player.PlayerID] = 0;
            }
        }

        public Question? GetCurrentQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count)
                return Questions[CurrentQuestionIndex];
            return null;
        }

        public bool GoToNextQuestion()
        {
            if (CurrentQuestionIndex + 1 < Questions.Count)
            {
                CurrentQuestionIndex++;
                return true;
            }
            return false; // No more questions
        }

        public bool UpdatePlayerScore(int playerId, bool isCorrect)
        {
            if (ScoreBoard.ContainsKey(playerId))
            {
                ScoreBoard[playerId] += isCorrect ? 1 : 0; // +1 scorepoint for right answer
                return true;
            }
            return false;
        }

        public bool IsGameOver()
        {
            return CurrentQuestionIndex >= Questions.Count;
        }
    }
}
