using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BackEnd.Models.Classes
{
    public class Game
    {
        [Key]
        public int Id { get; init; }           

        [NotMapped]
        public Dictionary<string, int> PlayersPoints { get; set; } = new Dictionary<string, int>(); // Use player IDs or usernames as keys  PlayersPoints[player.PlayerID] = playerPoints;        
        public List<Round> Rounds { get; set; } = []; 
        
    }
}
