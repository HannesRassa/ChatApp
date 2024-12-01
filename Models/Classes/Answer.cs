using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer {
    
    public int Id { get; init; }
    public required Question Question { get; set; }
    public required string AnswerText { get; init; }
    public required int AnswerPoints { get; init; }
    
}
