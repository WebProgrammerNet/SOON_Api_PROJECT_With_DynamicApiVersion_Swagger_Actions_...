using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiProjectModul.AppDataAccessLayer;
using ApiProjectModul.DataBaseGenerates;
using ApiProjectModul.MappingProfiles;
using ApiProjectModul.Services;
using ApiProjectModul.Swagger;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiProjectModul
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions(); //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.1
            //  services.AddDbContext<FoodDbContext>(opt => opt.UseInMemoryDatabase("FoodDatabase"));
            //https://stackoverflow.com/questions/58396865/disable-system-text-json-on-web-api-on-net-core-3-0

            services.AddDbContext<AppDataBase>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    configurepolicy =>
                    {
                        configurepolicy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddSingleton<ISeedDataBaseErrorService, RepositorySeedDataBaseErrorService>();
            services.AddScoped<IDataBaseGenerate, DataBaseGenerate>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();//using Microsoft.AspNetCore.Mvc.Infrastructure;
            services.AddScoped<IUrlHelper>(x => //using Microsoft.AspNetCore.Mvc.Routing;
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddControllers()
                 .AddNewtonsoftJson(options =>
                     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                          //serialization/deserialization ucun kohne kitabxanadan istifadeetmekucundur.
                          //https://stackoverflow.com/questions/58396865/disable-system-text-json-on-web-api-on-net-core-3-0
                          .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddApiVersioning(
               config =>
               {
                   config.ReportApiVersions = true;
                   config.AssumeDefaultVersionWhenUnspecified = true;
                   config.DefaultApiVersion = new ApiVersion(1, 0);
                   config.ApiVersionReader = new HeaderApiVersionReader("api-version");//using Microsoft.AspNetCore.Mvc.Versioning;
               });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    // примечание: эта опция необходима только при управлении версиями по сегменту URL. SubstitutionFormat
                    // также может использоваться для управления форматом версии API в шаблонах маршрутов


                    options.SubstituteApiVersionInUrl = true; //Substitute - ?v?zedici
                                                              //https://github.com/microsoft/aspnet-api-versioning/wiki/API-Explorer-Options
                                                              //https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings
                                                              //https://github.com/microsoft/aspnet-api-versioning/blob/master/samples/webapi/SwaggerWebApiSample/Startup.cs
                });
            //https://translate.google.com/translate?hl=en&sl=en&tl=ru&u=https%3A%2F%2Fdotnetcoretutorials.com%2F2017%2F01%2F17%2Fapi-versioning-asp-net-core%2F
            //https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/
            //Bizim Programimizin bir nece versiyada olan(v1,v2) Api-leri 

            //basa dusmesi ucun Nudget Packetden > Microsoft.AspNetCore.Mvc.Versioning < YUKLEMELIYIK.
            //https://github.com/domaindrivendev/Swashbuckle
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();

            services.AddAutoMapper(typeof(CompositionMappings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();//using Microsoft.AspNetCore.Diagnostics;
                        if (errorFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                        }

                        await context.Response.WriteAsync("There was an error");
                    });
                });
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAllOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });


        }
    }
}
///Install-Package Swashbuckle.AspNetCore -Version 5.0.0-rc3
#region
//https://translate.google.com/translate?hl=en&sl=en&tl=ru&u=https%3A%2F%2Fstackoverflow.com%2Fquestions%2F1051182%2Fwhat-is-data-transfer-object
//Объект передачи данных - это объект, который используется для инкапсуляции данных и отправки их из одной подсистемы приложения в другую.
//DTO чаще всего используются уровнем служб в приложении N-уровня для передачи данных между собой и уровнем пользовательского интерфейса.
//    Основным преимуществом здесь является то, что он уменьшает объем данных, которые необходимо передавать по проводам в распределенных 
//    приложениях. Они также делают отличные модели в шаблоне MVC.
//> Другое использование DTO может заключаться в инкапсуляции параметров для вызовов методов.Это может быть полезно,
// если метод принимает более 4 или 5 параметров. <
//При использовании шаблона DTO вы также должны использовать ассемблеры DTO. Ассемблеры используются для создания DTO из доменных объектов, 
//и наоборот.
//Преобразование из Доменного объекта в DTO и обратно может быть дорогостоящим процессом. Если вы не создаете распределенное приложение,
//вы, вероятно, не увидите каких-либо значительных 
//    преимуществ от шаблона, как объясняет здесь Мартин Фаулер
//DTO - это тупой объект - он просто содержит свойства и имеет геттеры и сеттеры, но никакой 
//другой логики какого-либо значения(кроме, может быть, реализации сравнения () или равно ()).
//Обычно модельные классы в MVC(при условии .net MVC здесь) являются DTO или коллекциями / агрегатами DTO

//Объект передачи данных(DTO), ранее известный как объекты значений или VO, является шаблоном проектирования,
//используемым для передачи данных между подсистемами программных приложений.DTO часто используются в 
//сочетании с объектами доступа к данным для извлечения данных из базы данных.
//Для меня лучший ответ на вопрос, что такое DTO, состоит в том, что DTO - это простые объекты, которые не должны 
//содержать никакой бизнес-логики или реализации методов, которые потребовали бы тестирования.
//Обычно ваша модель(использующая шаблон MVC) представляет собой интеллектуальные модели, и они могут 
//содержать множество / несколько методов, которые выполняют некоторые различные операции специально для 
//этой модели(не бизнес-логика, это должно быть на контроллерах). Однако при передаче данных(например, при вызове конечной 
//точки REST (GET / POST / что угодно) откуда-либо, или при использовании веб-службы с использованием SOA и т.Д.) 
//Вы не хотите передавать объект большого размера с кодом, который не является необходим для конечной точки, будет потреблять  
//данные и замедлять передачу.
#endregion