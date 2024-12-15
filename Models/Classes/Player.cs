using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public record Player {

    
    public int Id { get; init; }
    public required string Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public List<Game> Games { get; set; } = new();

}
