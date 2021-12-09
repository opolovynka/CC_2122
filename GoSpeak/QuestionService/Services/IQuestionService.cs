using GoSpeak.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoSpeak.QuestionService.Services
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAllQuestions();
        Task<Question> GetQuestion(int userId, int questionId);
    }
}
