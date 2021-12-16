using GoSpeak.Model.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections.Generic;


namespace GoSpeak.Model
{    
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        
        
        public LanguageLevelEnum QuestionLevel { get; set; }
        public int CorrectAnswerId { get; set; }
        public List<Answer> Answers { get; set; }
    }
}
