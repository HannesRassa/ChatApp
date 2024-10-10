using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GameRoomRepo(DataContext context) {
    private readonly DataContext context = context;

    public async Task<List<GameRoom>> GetAllGameRooms() => await context.GameRooms.ToListAsync();
}
