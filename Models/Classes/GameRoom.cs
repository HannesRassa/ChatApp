using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Classes;

public record GameRoom {
    [Key]
    public int Id { get; init; }
    public int RoomCode { get; init; }
    public required Player Admin { get; init; }
    public List<Player> Players { get; set; } = new();

    // Parameterless constructor for flexibility
    public GameRoom() { }

    // Constructor that takes an admin
    public GameRoom(Player admin)
    {
        Admin = admin;
        Players.Add(admin);
        RoomCode = new Random().Next(1000, 9999); // Assign a random 4-digit room code
    }
    public class JoinRoomRequest
{
    public int PlayerId { get; set; }
    public int RoomCode { get; set; }
}
}

