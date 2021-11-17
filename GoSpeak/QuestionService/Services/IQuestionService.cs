using GoSpeak.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoSpeak.QuestionService.Services
{
    public interface IQuestionService
    {
        public Task<List<Question>> GetAllQuestions();
        public Task<Question> GetQuestion(int userId, int questionId);
    }
}
