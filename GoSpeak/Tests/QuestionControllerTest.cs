using Microsoft.Extensions.Logging;
using Moq;
using QuestionService.Controllers;
using System;
using Xunit;

namespace Tests
{
    public class QuestionControllerTest
    {
        ILogger<QuestionController> _logger = new Mock<ILogger<QuestionController>>().Object;
        private readonly QuestionController _controller;

        public QuestionControllerTest()
        {
            _controller = new QuestionController(_logger);
        }

        [Fact]
        public void GetQuestion_ThrowArgumentError()
        {
            //Arange
            int userId = 0;

            //Act
            Action act = () => _controller.Question(userId);
            Action act2 = () => _controller.Question(userId, new Model.Question());

            //Assert
            //without input parameter Question
            ArgumentException exception = Assert.Throws<ArgumentException>(act);
            Assert.Equal("Arugment userId is wrong. It can be less or equal 0", exception.Message);

            //with input parameter Question
            exception = Assert.Throws<ArgumentException>(act2);
            Assert.Equal("Arugment userId is wrong. It can be less or equal 0", exception.Message);
        }

        [Fact]
        public void GetQuestion_Success()
        {
            //Arange
            int userId = 1;

            //Act
            var q = _controller.Question(userId);

            //Assert
            Assert.NotNull(q);
        }
    }
}
