using GoSpeak.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoSpeak.QuestionService.Services
{
    public class QuestionService : IQuestionService
    {
        private QuestionContext _context;
        public QuestionService(QuestionContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetAllQuestions()
        {
            return await _context.Questions.Include(q => q.Answers).ToListAsync();
        }

        public async Task<Question> GetQuestion(int userId, int questionId = 0)
        {
            if (questionId == 0)
            {
                return await _context.Questions.Include(q => q.Answers).FirstAsync();
            }
            else
            {
                return await _context.Questions.Include(q => q.Answers).FirstAsync(q => q.Id != questionId);
            }
        }

    }
}
