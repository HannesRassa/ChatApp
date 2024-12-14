using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Answer
{
    public int Id { get; init; }

    // Foreign key to Group
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    // Player who provided the answer
    public int PlayerId { get; set; }

    // Question associated with the answer
    public required Question Question { get; set; }

    // Answer text
    public required string AnswerText { get; set; }

    // Points awarded for the answer
    public required int AnswerPoints { get; set; }
}

