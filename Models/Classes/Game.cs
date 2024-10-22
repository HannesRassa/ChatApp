using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes
{
    public class Game
    {
        [Key]
        public int Id { get; init; }           
        public int AdminID { get; set; } 

        public List<Player> Players { get; set; } = []; 
        public Dictionary<Player, int> PlayersPoints { get; set; } = []; // <Id, Points>
        
        public List<Question> Questions { get; set; } = []; 
        
        
        [Range(1, 100, ErrorMessage = "The numbers of rounds can be from 1 to 100")]
        public int Rounds { get; set; } = 5;           

        [Range(5, 300, ErrorMessage = "The timer can last from 5 sec to 5 min")]
        public int TimerForAnsweringInSec { get; set; } = 30;                           
        
        [Range(2, 10, ErrorMessage = "2 - 10 people per gourp")]
        public int PlayersPerGroup { get; set; } = 2;                          

    }
}
