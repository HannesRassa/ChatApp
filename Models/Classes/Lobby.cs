using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models.Classes;

public record Lobby {

    public int Id { get; init; }
    public int RoomCode { get; init; }
    public required Player Admin { get; init; }
    public List<Player> Players { get; set; } = new();

  
    public Lobby() { }

    // Constructor that takes an admin
    public Lobby(Player admin)
    {
        Admin = admin;
        Players.Add(admin);
        RoomCode = new Random().Next(1000, 9999); // Assign a random 4-digit room code
    }
    public class JoinLobbyRequest
    {
        public int PlayerId { get; set; }
        public int RoomCode { get; set; }
    }
}

