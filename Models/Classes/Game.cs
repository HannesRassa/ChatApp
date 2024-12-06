using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Classes
{
    public class Game
    {
        public int Id { get; init; }

        // List of players in the game
        public List<Player> Players { get; set; } = new();

        // Player points for the game
        [NotMapped]
        public Dictionary<string, int> PlayersPoints { get; set; } = new();

        // List of rounds in the game
        public List<Round> GameRounds { get; set; } = new();

        // List of questions specific to the game
        public List<Question> GameQuestions { get; set; } = new();

        // Number of rounds in the game
        [Range(1, 100, ErrorMessage = "The numbers of rounds can be from 1 to 100")]
        public int Rounds { get; set; } = 5;

        // Timer for answering questions
        [Range(5, 300, ErrorMessage = "The timer can last from 5 sec to 5 min")]
        public int TimerForAnsweringInSec { get; set; } = 30;

        // Maximum players per group
        [Range(2, 10, ErrorMessage = "2 - 10 people per group")]
        public int PlayersPerGroup { get; set; } = 2;
    }
}

