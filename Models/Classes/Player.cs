using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Player {

    
    public int Id { get; init; }
    public required string Username { get; set; }
    public string? Password { get; set; }
    public List<Game> Games { get; set; } = new();

}
