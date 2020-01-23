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
                    // ����������: ��� ����� ���������� ������ ��� ���������� �������� �� �������� URL. SubstitutionFormat
                    // ����� ����� �������������� ��� ���������� �������� ������ API � �������� ���������


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
//������ �������� ������ - ��� ������, ������� ������������ ��� ������������ ������ � �������� �� �� ����� ���������� ���������� � ������.
//DTO ���� ����� ������������ ������� ����� � ���������� N-������ ��� �������� ������ ����� ����� � ������� ����������������� ����������.
//    �������� ������������� ����� �������� ��, ��� �� ��������� ����� ������, ������� ���������� ���������� �� �������� � �������������� 
//    �����������. ��� ����� ������ �������� ������ � ������� MVC.
//> ������ ������������� DTO ����� ����������� � ������������ ���������� ��� ������� �������.��� ����� ���� �������,
// ���� ����� ��������� ����� 4 ��� 5 ����������. <
//��� ������������� ������� DTO �� ����� ������ ������������ ���������� DTO. ���������� ������������ ��� �������� DTO �� �������� ��������, 
//� ��������.
//�������������� �� ��������� ������� � DTO � ������� ����� ���� ������������� ���������. ���� �� �� �������� �������������� ����������,
//��, ��������, �� ������� �����-���� ������������ 
//    ����������� �� �������, ��� ��������� ����� ������ ������
//DTO - ��� ����� ������ - �� ������ �������� �������� � ����� ������� � �������, �� ������� 
//������ ������ ������-���� ��������(�����, ����� ����, ���������� ��������� () ��� ����� ()).
//������ ��������� ������ � MVC(��� ������� .net MVC �����) �������� DTO ��� ����������� / ���������� DTO

//������ �������� ������(DTO), ����� ��������� ��� ������� �������� ��� VO, �������� �������� ��������������,
//������������ ��� �������� ������ ����� ������������ ����������� ����������.DTO ����� ������������ � 
//��������� � ��������� ������� � ������ ��� ���������� ������ �� ���� ������.
//��� ���� ������ ����� �� ������, ��� ����� DTO, ������� � ���, ��� DTO - ��� ������� �������, ������� �� ������ 
//��������� ������� ������-������ ��� ���������� �������, ������� ����������� �� ������������.
//������ ���� ������(������������ ������ MVC) ������������ ����� ���������������� ������, � ��� ����� 
//��������� ��������� / ��������� �������, ������� ��������� ��������� ��������� �������� ���������� ��� 
//���� ������(�� ������-������, ��� ������ ���� �� ������������). ������ ��� �������� ������(��������, ��� ������ �������� 
//����� REST (GET / POST / ��� ������) ������-����, ��� ��� ������������� ���-������ � �������������� SOA � �.�.) 
//�� �� ������ ���������� ������ �������� ������� � �����, ������� �� �������� ��������� ��� �������� �����, ����� ����������  
//������ � ��������� ��������.
#endregion