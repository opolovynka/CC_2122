using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model;
using System;

namespace QuestionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionController : ControllerBase
    {
        
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ILogger<QuestionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Question Question(int userId, Question q = null)
        {
            if (userId <= 0)
            {
                _logger.LogError($"{nameof(userId)} = {userId} which is <= 0");
                throw new ArgumentException($"Arugment {nameof(userId)} is wrong. It can be less or equal 0");
            }

            if (q == null)
            {
                return new Question()
                {
                    Id = 1,
                    QuestionText = "I can't find my keys. I don't know where ____ are.",
                    CorrectAnswerId = 3,
                    Answers = new Answer[]
                    {
                        new Answer
                        {
                            Id = 1,
                            AnswerText = "a. it"
                        },
                        new Answer
                        {
                            Id = 2,
                            AnswerText = "b. them"
                        },
                         new Answer
                        {
                            Id = 3,
                            AnswerText = "c. they correct"
                        }
                    }
                };
            }
            else
            {
                return new Question()
                {
                    Id = 1,
                    QuestionText = "I can't find my keys. I don't know where ____ are.",
                    CorrectAnswerId = 3,
                    Answers = new Answer[]
                    {
                        new Answer
                        {
                            Id = 1,
                            AnswerText = "a. it"
                        },
                        new Answer
                        {
                            Id = 2,
                            AnswerText = "b. them"
                        },
                         new Answer
                        {
                            Id = 3,
                            AnswerText = "c. they correct"
                        }
                    }
                };
            }
        }
    }
}
