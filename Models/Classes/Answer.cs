using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer {
    
    public int Id { get; init; }
    public int PlayerId { get; set; }
    public required Question Question { get; set; }
    public required string AnswerText { get; set; }
    public required int AnswerPoints { get; set; }
    
}
