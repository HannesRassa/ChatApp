using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Group {
    
    public int Id { get; init; }
    public required List<Player> Players { get; init; }
    public Question? Question{ get; set; }
    public required List<Answer> Answers { get; set; }
}
