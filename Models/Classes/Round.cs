using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Round {
    
    public int Id { get; init; }
    public required List<Group> Groups { get; set; }       
}
