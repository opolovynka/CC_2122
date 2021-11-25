using Microsoft.Extensions.Logging;
using Moq;
using GoSpeak.QuestionService.Controllers;
using System;
using Xunit;
using GoSpeak.QuestionService.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GoSpeak.Tests
{
    public class QuestionsControllerUnitTest
    {
        ILogger<QuestionsController> _logger = new Mock<ILogger<QuestionsController>>().Object;
        IQuestionService _questionService = new Mock<IQuestionService>().Object;
        private readonly QuestionsController _controller;

        public QuestionsControllerUnitTest()
        {
            _controller = new QuestionsController(_logger, _questionService);
        }

        [Fact]
        [Trait("Test", "Unit")]
        public async Task GetQuestion_BadRequest()
        {
            //Arange
            int userId = 0;
            int questionId = 1;

            //Act
            var result1 = await _controller.Get(userId);
            var result2 = await _controller.Get(userId, questionId);
            var badResult1 = result1 as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;
            var badResult2 = result2 as Microsoft.AspNetCore.Mvc.BadRequestObjectResult;

            //Assert
            //without input parameter QuestionId
            Assert.NotNull(badResult1);
            Assert.Equal(400, badResult1.StatusCode);
            Assert.Equal("Parameter 'userId' is wrong. It should be more then 0", badResult1.Value);

            //with input parameter QuestionId
            Assert.NotNull(badResult2);
            Assert.Equal(400, badResult2.StatusCode);
            Assert.Equal("Parameter 'userId' is wrong. It should be more then 0", badResult2.Value);
        }

        [Fact]
        [Trait("Test", "Unit")]
        public async Task GetQuestion_Success()
        {
            //Arange
            int userId = 1;
            int questionId = 1;

            //Act
            var response1 = await _controller.Get(userId);
            var response2 = await _controller.Get(userId, questionId);
            var okResponse1 = response1 as OkObjectResult;
            var okResponse2 = response2 as OkObjectResult;

            //Assert
            //without input parameter QuestionId
            Assert.NotNull(okResponse1);
            Assert.Equal(200, okResponse1.StatusCode);

            //with input parameter QuestionId
            Assert.NotNull(okResponse2);
            Assert.Equal(200, okResponse2.StatusCode);
        }
    }
}