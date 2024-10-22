using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer {
    
    [Key]
    public int Id { get; init; }
    public required Question Question { get; init; }
    public required Player Player { get; init; }
    public required string AnswerText { get; init; }
    public required int AnswerPoints { get; init; }
    
}
