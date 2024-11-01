using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Group {
    
    [Key]
    public int Id { get; init; }
    public required List<Player> Players { get; init; }
    public required Question Question { get; init; }
    public required List<Answer> Answers { get; set; }
}
