using BackEnd.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Data.Repos;
public class QuestionsRepo(DataContext context) {
    private readonly DataContext context = context;
    public async Task<List<Question>> GetAllQuestions() => await context.Questions.ToListAsync();
}
    
