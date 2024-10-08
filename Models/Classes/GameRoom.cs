using BackEnd.Models.Enums;

namespace BackEnd.Models.Classes;

public record GameRoom {
    public int RoomCode { get; init; }
    public int MaxCapacity { get; init; }
    public GameStatus Status { get; init; }
    public int AdminID { get; init; }          
    public List<Player> Players { get; init; } = new();   
    public List<Question> Questions { get; init; } = new();  
    public int CurrentQuestion { get; set; }        
    public Dictionary<string, int> ScoreBoard { get; init; } = new(); 
    public int GameTimer { get; set; }              
}
