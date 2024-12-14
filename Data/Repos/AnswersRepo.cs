using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class AnswersRepo(DataContext context)
{
    private readonly DataContext context = context;

    //CREATE
    public async Task<Answer> SaveAnswerToDb(Answer answer)
    {
        context.Questions.Attach(answer.Question);
        context.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    //READ
    public async Task<List<Answer>> GetAllAnswers(Question? question = null, Player? player = null)
    {
        IQueryable<Answer> query = context.Answers.AsQueryable();
        if (question is not null) query = query.Where(x => x.Question == question);
        return await query.ToListAsync();
    } 

    public async Task<Answer?> GetAnswerById(int id) => await context.Answers.FindAsync(id);
    public async Task<bool> AnswerExistsInDb(int id) => await context.Answers.AnyAsync(x => x.Id == id);
    
    //UPDATE
    public async Task<bool> UpdateAnswer(int id, Answer answer)
    {
        bool isIdsMatch = id == answer.Id;
        bool answerExists = await AnswerExistsInDb(id);
        if (!isIdsMatch || !answerExists) return false;
        context.Update(answer);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteAnswerById(int id)
    {
        Answer? answerInDb = await GetAnswerById(id);
        if (answerInDb == null) return false;
        context.Remove(answerInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}

