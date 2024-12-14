using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer
{
    public int Id { get; init; }
    public int GroupId { get; set; }
    // public Group Group { get; set; } = null!;
    // public int RoundId { get; set;}
    public int PlayerId { get; set; }
    // public required Question Question { get; set; }
    public int QuestionId { get; set; }
    public int RoundId { get; set; }
    public required string AnswerText { get; set; }


}

