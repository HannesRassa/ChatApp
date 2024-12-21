using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public class Round
{
    public int Id { get; set; }
    public int RoundNumber{get;set;}
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public List<Group> Groups { get; set; } = new();
}