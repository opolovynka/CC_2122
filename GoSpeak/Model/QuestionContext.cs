using Microsoft.EntityFrameworkCore;

namespace GoSpeak.Model
{
    public class QuestionContext : DbContext
    {
        public QuestionContext(DbContextOptions<QuestionContext> options)
            : base(options)
        {
        }

        public DbSet<Question> Questions { get; set; } = null!;
    }
}
