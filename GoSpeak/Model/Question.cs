using GoSpeak.Model.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoSpeak.Model
{    
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LanguageLevelEnum QuestionLevel { get; set; }
        public int CorrectAnswerId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
