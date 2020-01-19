using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace CourseLibrary.API
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
            services.AddControllers(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                // setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction => {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "https://courselibrary.com/modelvalidationproblem",
                        Title = "One or more model validation errors occurred.",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "See the errors property for details.",
                        Instance = context.HttpContext.Request.Path
                    };

                    problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            
            var server = Configuration["DbConnectionString:DbServer"] ?? "localhost";
            var port = Configuration["DbConnectionString:DbPort"] ?? "1433";
            var database = Configuration["DbConnectionString:Database"] ?? "CourseLibraryDb";
            var user = Configuration["DbConnectionString:DbUser"] ?? "sa";
            var password = Configuration["DbConnectionString:DbPassword"] ?? "Passw0rd";

            var connectionString = $"Server={server},{port};Initial Catalog={database};User ID={user};Password={password}";

            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler(appBuilder => 
                {
                    appBuilder.Run(async context => {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened");
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
