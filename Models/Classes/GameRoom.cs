using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record GameRoom {
    [Key]
    
    public int Id { get; init; }
    public int RoomCode { get; init; }
    public required Player Admin { get; init; }
    public List<Player> Players { get; set; } = new();   
}
