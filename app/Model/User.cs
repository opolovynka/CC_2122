using GoSpeak.Model.Enums;


namespace GoSpeak.Model
{    
    public class User
    {
        public int UserId {get;set;}
        public int NumCorrectAnswers { get; set; }
        public int NumWrongAnswers { get; set; }
        public int[] AskedQuestion { get; set; }
        public double Level { get; set; }
        
        public UserRoleEnum Role { get; set; }
    }
}
