using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models.Classes;

public class Round
{
    public int Id { get; set; }
    public List<Group> Groups { get; set; } = new();
}
