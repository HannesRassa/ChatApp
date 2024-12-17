
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models.Classes;

public class Lobby
{
    public int Id { get; init; }
    public int RoomCode { get; init; }

    public int AdminId { get; init; } // Foreign key reference
    [ForeignKey("AdminId")]
    public Player Admin { get; init; } = null!;

    public List<Player> Players { get; set; } = new();

    public Lobby() { }

    public Lobby(Player admin)
    {
        Admin = admin;
        AdminId = admin.Id;
        Players.Add(admin);
        RoomCode = new Random().Next(1000, 9999); // Random 4-digit room code
    }
}

    public class JoinLobbyRequest
    {
        public int PlayerId { get; set; }
        public int RoomCode { get; set; }
    }


