using System.Text;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;

public class PlayersRepo(DataContext context)
{
    private readonly DataContext _context = context;

    // CREATE
    public async Task<Player> SavePlayerToDb(Player player)
    {
        // Generate a random salt
        var salt = Guid.NewGuid().ToString(); // You can also use a secure random generator

        // Hash the password with the generated salt
        player.Password = HashPassword(player.Password, salt);

        // Save the player to the database
        _context.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    // READ
    public async Task<List<Player>> GetAllPlayers() => await _context.Players.ToListAsync();

    public async Task<Player?> GetPlayerById(int id) => await _context.Players.FindAsync(id);

    public async Task<bool> PlayerExistsInDb(int id) => await _context.Players.AnyAsync(x => x.Id == id);

    // UPDATE
    public async Task<bool> UpdatePlayer(int id, Player player)
    {
        // Check if player exists and IDs match
        bool isIdsMatch = id == player.Id;
        bool playerExists = await PlayerExistsInDb(id);

        if (!isIdsMatch || !playerExists) return false;

        // If updating the password, hash it again
        if (!string.IsNullOrEmpty(player.Password))
        {
            var salt = Guid.NewGuid().ToString();
            player.Password = HashPassword(player.Password, salt);
        }

        _context.Update(player);
        int updatedRecordsCount = await _context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }

    // DELETE
    public async Task<bool> DeletePlayerById(int id)
    {
        var playerInDb = await GetPlayerById(id);
        if (playerInDb == null) return false;

        _context.Remove(playerInDb);
        int changesCount = await _context.SaveChangesAsync();
        return changesCount == 1;
    }

    // AUTHENTICATE
    public async Task<Player?> Authenticate(string username, string password)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Username == username);
        if (player == null) return null;

        var passwordParts = player.Password.Split(':');
        if (passwordParts.Length != 2) return null; 

        var salt = passwordParts[0];
        var storedHashedPassword = passwordParts[1];

        var inputHashedPassword = HashPassword(password, salt).Split(':')[1];

        return storedHashedPassword == inputHashedPassword ? player : null;
    }

    // HELPER: Hash Password
    private string HashPassword(string password, string salt)
    {
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return $"{salt}:{hashed}";
    }
}
