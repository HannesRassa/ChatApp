using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class QuestionsRepo(DataContext context) {
   private readonly DataContext context = context;
    public async Task<List<Question>> GetAllQuestions() => await context.Questions.ToListAsync();

    public async Task<Question?> GetQuestionById(int id)
    {
        return await context.Questions.FirstOrDefaultAsync(p => p.QuestionID == id);
    }
    public async Task AddQuestion(Question question)
    {
        context.Questions.Add(question);
        await context.SaveChangesAsync();
    }
    public async Task UpdateQuestion(Question question)
    {
        context.Questions.Update(question);
        await context.SaveChangesAsync();
    }
}
    
