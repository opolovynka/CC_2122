using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GoSpeak.Model;
using System.Text.Json;
using System.Threading.Tasks;
using GoSpeak.QuestionService.Services;

namespace GoSpeak.QuestionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        
        private readonly ILogger<QuestionsController> _logger;
        private readonly IQuestionService _questionService;

        public QuestionsController(ILogger<QuestionsController> logger, IQuestionService questionService)
        {
            _logger = logger;
            _questionService = questionService;
        }

        [HttpGet]
        [Route("{userId}/{questionId?}")]
        public async Task<IActionResult> Get(int userId, int questionId = 0)
        {
            if (userId <= 0)
            {
                _logger.LogError($"{nameof(userId)} = {userId} which is <= 0");
                
                return BadRequest($"Parameter '{nameof(userId)}' is wrong. It should be more then 0");
            }

            Question result = await _questionService.GetQuestion(userId, questionId);

            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(JsonSerializer.Serialize(await _questionService.GetAllQuestions()));
        }
    }
}
