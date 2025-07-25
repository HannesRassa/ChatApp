using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;

public class QuestionsRepo(DataContext context)
{
    private readonly DataContext context = context;

    // CREATE
    public async Task<Question> SaveQuestionToDb(Question question)
    {
        context.Add(question);
        await context.SaveChangesAsync();
        return question;
    }

    public async Task<List<Question>> SaveQuestionsToDb(IEnumerable<Question> questions)
    {
        context.AddRange(questions);
        await context.SaveChangesAsync();
        return questions.ToList();
    }

    // READ
    public async Task<List<Question>> GetAllQuestions() => await context.Questions.ToListAsync();

    public async Task<Question?> GetQuestionById(int id) => await context.Questions.FindAsync(id);

    public async Task<List<Question>> GetQuestionsByPackName(string packName) =>
        await context.Questions.Where(q => q.PackName == packName).ToListAsync();

    public async Task<bool> QuestionExistsInDb(int id) => await context.Questions.AnyAsync(x => x.Id == id);

    public async Task<List<string>> GetDistinctPackNames() =>
    await context.Questions.Select(q => q.PackName).Distinct().ToListAsync();

    // UPDATE
    public async Task<bool> UpdateQuestion(int id, Question question)
    {
        bool isIdsMatch = id == question.Id;
        bool questionExists = await QuestionExistsInDb(id);
        if (!isIdsMatch || !questionExists) return false;
        context.Update(question);
        int updatedRecordsCount = await context.SaveChangesAsync();
        return updatedRecordsCount == 1;
    }

    // DELETE
    public async Task<bool> DeleteQuestionById(int id)
    {
        Question? questionInDb = await GetQuestionById(id);
        if (questionInDb == null) return false;
        context.Remove(questionInDb);
        int changesCount = await context.SaveChangesAsync();
        return changesCount == 1;
    }
}
