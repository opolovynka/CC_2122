using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using GoSpeak.Model;
using Newtonsoft.Json;

public static class Seeder
{

    public static void Seedit(string jsonData, IServiceProvider serviceProvider)
    {        
        List<Question> questions =
         JsonConvert.DeserializeObject<List<Question>>(
           jsonData);
        using (
         var serviceScope = serviceProvider
           .GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope
                   .ServiceProvider.GetService<QuestionContext>();
            if (!context.Questions.Any())
            {
                context.AddRange(questions);
                context.SaveChanges();
            }
        }
    }
}