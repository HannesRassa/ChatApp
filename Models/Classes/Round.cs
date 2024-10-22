using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Round {
    
    [Key]
    public int Id { get; init; }
    public required List<Group> Groups { get; set; }
    public required List<Question> Questions { get; init; }
    
}
