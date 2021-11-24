using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GoSpeak.Model;
using Xunit;
using System.Collections.Generic;

namespace GoSpeak.Tests
{
    public class QuestionControllerIntegrationTest
    {

        private ComponentConfig _testConfig { get; set; } = new ComponentConfig();
        private string _endpoint = "api/question";
        private string _url;

        public QuestionControllerIntegrationTest()
        {
            _testConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Component")
                .Get<ComponentConfig>();

            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("API_URL")))
            {
                _testConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Component")
                .Get<ComponentConfig>();

                _url = _testConfig.ServiceUri + _endpoint;
            }
            else
            {
                _url = Environment.GetEnvironmentVariable("API_URL") + _endpoint;
            }
        }
        
        [Fact]
        [Trait("Test", "Integration")]
        public async Task Question_GetAllRequest_Success(){
            //Arrange
            var httpClient = new HttpClient();            

            //Act
            var response = await httpClient.GetAsync(_url);
            var responseJson = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var questions = JsonSerializer.Deserialize<List<Question>>(responseJson);

            Assert.Equal(10, questions.Count);
        }

        [Fact]
        [Trait("Test", "Integration")]
        public async Task Question_GetRequest_Success(){
            //Arrange            
            var httpClient = new HttpClient();
            var userId = 1;
            var questionId = 1;
            string url = _url + "/" + userId + "/" + questionId;

            //Act
            var response = await httpClient.GetAsync(url);
            var responseJson = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            var question = JsonSerializer.Deserialize<Question>(responseJson);

            Assert.NotEqual(questionId, question.Id);
        }

        [Fact]
        [Trait("Test", "Integration")]
        public async Task Question_GetRequest_BadRequest(){
            //Arrange
            int userId = 0;
            var httpClient = new HttpClient();
            
            string url = _url + "/" + userId;

            //Act and A
            var response = await httpClient.GetAsync(url);
            var result = response.Content.ReadAsStringAsync().Result;

            //Asserts
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal($"Parameter 'userId' is wrong. It should be more then 0", result);
        }
    }
}