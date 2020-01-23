using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProjectModul.AppDataAccessLayer;
using ApiProjectModul.Models;
using ApiProjectModul.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiProjectModul
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var _db = services.GetRequiredService<AppDataBase>();
                    List<Composition> check = _db.Compositions.ToList();


                    if (check.Count() == 0)
                    {
                        var dbInitializer = services.GetRequiredService<ISeedDataBaseErrorService>();
                        dbInitializer.Initialize(_db).GetAwaiter().GetResult();//Buna Bax

                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("The Program Database is null");
                    }
                }
                catch (Exception problem)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(problem, "An error occurred while seeding the database");
                }
                //finally
                //{
                //    var logger = services.GetRequiredService<ILogger<Program>>();
                //    logger.LogError("An error occurred while seeding the database");
                //}

            }
            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
    }
}
#region SECURITY
//Здравствуйте Друзья.
//Наверняка думаете зачем на все это?
//У мня тоже эст вопрос.
//Эсли однажды ваша база данных покажеть вам null или что то присходить в ветом роде.ЧТО НАМ ДЕЛАТ?
//Эсли поймете етот код вы можете пдключать свою зпасную базу данных.
//Блогодаря Логирование Ну если вы правилно устанавили систем Логирование то вы можете оследоать  зачепку.
#endregion