using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Data.Repos;
using BackEnd.Models.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Controllers
{
    [Route("Backend/Player")]
    [ApiController]
    public class PlayerController(PlayersRepo repo, IConfiguration configuration) : ControllerBase
    {
        private readonly PlayersRepo _repo = repo;
        private readonly IConfiguration _configuration = configuration;


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await repo.GetAllPlayers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var player = await repo.GetPlayerById(id);
            if (player == null)
                return NotFound();
            return Ok(player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Player newPlayer)
        {
            bool result = await repo.UpdatePlayer(id, newPlayer);
            return result ? NoContent() : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SavePlayer([FromBody] Player newPlayer)
        {
            var playerExists = await repo.PlayerExistsInDb(newPlayer.Id);
            if (playerExists) return Conflict();

            var result = await repo.SavePlayerToDb(newPlayer); 
            return CreatedAtAction(nameof(SavePlayer), new { newPlayer.Id }, result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer([FromRoute] int id)
        {
            bool isDeleted = await repo.DeletePlayerById(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            var player = await repo.Authenticate(request.Username, request.Password);
            if (player == null)
                return Unauthorized("Invalid username or password.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, player.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("playerId", player.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                id = player.Id,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });

        }
    }

    public record LoginRequest
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
    }

}