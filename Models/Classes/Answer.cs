using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer
{
    public int Id { get; init; }
    public int GroupId { get; set; }
    public int PlayerId { get; set; }
    public  Question Question { get; set; }
    public int RoundId { get; set; }
    public required string AnswerText { get; set; }


}

