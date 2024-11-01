using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class RoundsRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Round> SaveRoundToDb(Round round)
    {
        context.Add(round);
        await context.SaveChangesAsync();
        return round;
    }

    //READ
    public async Task<List<Round>> GetAllRounds() => await context.Rounds.ToListAsync(); 

    public async Task<Round?> GetRoundById(int id) => await context.Rounds.FindAsync(id);
    public async Task<bool> RoundExistsInDb(int id) => await context.Rounds.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdateRound(int id, Round round)
    {
        bool isIdsMatch = id == round.Id;
        bool roundExists = await RoundExistsInDb(id);
        if (!isIdsMatch || !roundExists) return false;
        context.Update(round);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteRoundById(int id)
    {
        Round? roundInDb = await GetRoundById(id);
        if (roundInDb == null) return false;
        context.Remove(roundInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}

