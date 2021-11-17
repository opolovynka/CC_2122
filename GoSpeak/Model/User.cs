using GoSpeak.Model.Enums;
using System.Text.Json.Serialization;

namespace GoSpeak.Model
{
    //class for Authorized/Not Authorized user
    public class User
    {
        public int UserId {get;set;}
        public int NumCorrectAnswers { get; set; }
        public int NumWrongAnswers { get; set; }
        public int[] AskedQuestion { get; set; }
        public double Level { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRoleEnum Role { get; set; }
    }
}
