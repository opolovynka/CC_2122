using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GoSpeak.Model;
using Xunit;
using System.Collections.Generic;

namespace Tests
{
    public class QuestionControllerIntegrationTest
    {

        private ComponentConfig TestConfig { get; set; } = new ComponentConfig();

        public QuestionControllerIntegrationTest()
        {
            TestConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Component")
                .Get<ComponentConfig>();
        }
        [Fact]
        public async Task Question_GetAllRequest_Success(){
            //Arrange            
            var httpClient = new HttpClient { BaseAddress = new Uri(TestConfig.ServiceUri) };
            
            string url = "api/question";

            //Act
            var response = await httpClient.GetAsync(url);
            var responseJson = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var questions = JsonSerializer.Deserialize<List<Question>>(responseJson);

            Assert.Equal(10, questions.Count);
        }

        [Fact]
        public async Task Question_GetRequest_Success(){
            //Arrange            
            var httpClient = new HttpClient { BaseAddress = new Uri(TestConfig.ServiceUri) };
            var userId = 1;
            var questionId = 1;
            string url = "api/question" + "/" + userId + "/" + questionId;

            //Act
            var response = await httpClient.GetAsync(url);
            var responseJson = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var question = JsonSerializer.Deserialize<Question>(responseJson);

            Assert.NotEqual(questionId, question.Id);
        }

        [Fact]
        public async Task Question_GetRequest_BadRequest(){
            //Arrange
            int userId = 0;
            var httpClient = new HttpClient { BaseAddress = new Uri(TestConfig.ServiceUri) };
            
            string url = "api/question/" + userId;

            //Act and A
            var response = await httpClient.GetAsync(url);
            var result = response.Content.ReadAsStringAsync().Result;

            //Asserts
            Assert.Equal(false, response.IsSuccessStatusCode);
            Assert.Equal($"Parameter 'userId' is wrong. It should be more then 0", result);
        }
    }
}