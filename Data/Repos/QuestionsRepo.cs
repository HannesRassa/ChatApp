using BackEnd.Models.Classes;
using ISA3Demos.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class QuestionsRepo(DataContext context)
{
    private readonly DataContext context = context;

    //READ
    public async Task<List<Question>> GetAllQuestions(QuestionHardness? hardness = null){
        IQueryable<Question> query = context.Questions.AsQueryable();
        if (hardness.HasValue){
            query = query.Where(x => x.Hardness == hardness);
        }
        return await query.ToListAsync();
    } 

    public async Task<Question?> GetQuestionById(int id) => await context.Questions.FindAsync(id);

    public async Task<bool> QuestionExistsInDb(int id) => await context.Questions.AnyAsync(x => x.QuestionID == id);

    //CREATE
    public async Task<Question> SaveQuestionToDb(Question question)
    {
        context.Add(question);
        await context.SaveChangesAsync();
        return question;
    }
    //UPDATE
    public async Task<bool> UpdateQuestion(int id, Question question)
    {
        bool isIdsMatch = id == question.QuestionID;
        bool questionExists = await QuestionExistsInDb(id);

        if (!isIdsMatch || !questionExists) return false;

        context.Update(question);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }
    //DELETE
    public async Task<bool> DeleteQuestionById(int id)
    {
        Question? questionInDb = await GetQuestionById(id);

        if (questionInDb == null) return false;

        context.Remove(questionInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;

    }
}

