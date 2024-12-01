using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class GroupsRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Group> SaveGroupToDb(Group group)
    {
        // Check if players already exist and attach if necessary
        foreach (var player in group.Players)
        {
            var existingPlayer = await context.Players.FindAsync(player.Id);
            if (existingPlayer == null)
            {
                context.Players.Attach(player);
            }
        }

        // Check if the question exists and attach it
        if (group.Question != null)
        {
            var existingQuestion = await context.Questions.FindAsync(group.Question.Id);
            if (existingQuestion == null)
            {
                context.Questions.Attach(group.Question);
            }
            else
            {
                group.Question = existingQuestion; // Use the existing question
            }
        }

        // Check if answers and their questions exist
        foreach (var answer in group.Answers)
        {
            var existingAnswer = await context.Answers.FindAsync(answer.Id);
            if (existingAnswer == null)
            {
                // Attach the question associated with the answer
                if (answer.Question != null)
                {
                    var existingAnswerQuestion = await context.Questions.FindAsync(answer.Question.Id);
                    if (existingAnswerQuestion == null)
                    {
                        context.Questions.Attach(answer.Question);
                    }
                    else
                    {
                        answer.Question = existingAnswerQuestion; // Use the existing question
                    }
                }
                context.Answers.Attach(answer);
            }
            else
            {
                answer.Question = existingAnswer.Question; // Ensure the question is consistent
            }
        }

        // Add the new group
        context.Groups.Add(group);
        await context.SaveChangesAsync();
        return group;
    }

    //READ
    public async Task<List<Group>> GetAllGroups() => await context.Groups.ToListAsync(); 

    public async Task<Group?> GetGroupById(int id)
    {
        return await context.Groups
            .Include(g => g.Players)          
            .Include(g => g.Question)        
            .Include(g => g.Answers)          
            .FirstOrDefaultAsync(g => g.Id == id); 
    }

    public async Task<bool> GroupExistsInDb(int id) => await context.Groups.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdateGroup(int id, Group group)
    {
        bool isIdsMatch = id == group.Id;
        bool groupExists = await GroupExistsInDb(id);
        if (!isIdsMatch || !groupExists) return false;
        context.Update(group);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteGroupById(int id)
    {
        Group? groupInDb = await GetGroupById(id);
        if (groupInDb == null) return false;
        context.Remove(groupInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}

