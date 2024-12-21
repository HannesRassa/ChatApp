using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Question {
    
    public int Id { get; init; }
    public required string QuestionText { get; init; }
    public string PackName { get; set; } = "Default";
    
}