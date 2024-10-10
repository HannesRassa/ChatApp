namespace BackEnd.Models.Classes;
public record Player {
    public int PlayerID { get; init; }
    public required string Username { get; init; }
    public bool IsAdmin { get; set; }
    public int Score { get; set; } = 0;
}
