using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Group {
    
    public int Id { get; init; }
    public List<Player>? Players { get; set; } = new();
    public Question? Question{ get; set; }
    public required List<Answer> Answers { get; set; }
}
