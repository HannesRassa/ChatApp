using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Group
{
    public int Id { get; init; }

    // Foreign key to Round
    public int RoundId { get; set; }
    public Round Round { get; set; } = null!;

    // List of players in the group
    public List<Player>? Players { get; set; } = new();

    // Question assigned to the group
    public Question? Question { get; set; }

    // List of answers provided by players
    public List<Answer>? Answers { get; set; } = new();
}

