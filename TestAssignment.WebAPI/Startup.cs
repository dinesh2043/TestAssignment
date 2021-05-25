using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

using TestAssignment.ProfanityCheck.ProfanityService;
using TestAssignment.ProfanityCheck.ProfanityService.Interfaces;
using TestAssignment.WebAPI.GlobalExceptionHandler;
using Microsoft.AspNetCore.Mvc;
using TestAssignment.DAL.Repositories.Interfaces;
using TestAssignment.DAL.Repositories;

namespace TestAssignment.WebAPI
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
            services.AddControllers();

            services.AddMvc().AddMetrics();
            services.AddMetricsTrackingMiddleware();

            services.AddHealthChecks();

            //This LazyCache.AspNetCore is used to solve those issues.
            services.AddLazyCache();

            services.AddTransient<IProfanityServices, ProfanityServices>();
            services.AddTransient<IProfanityListRepository, ProfanityListRepository>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Test Assignment API EndPoints",
                    Version = "v1",
                    Description = "Test Assignment API development",
                    Contact = new OpenApiContact
                    {
                        Name = "Dinesh Sapkota",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/dinesh2043"),
                    }
                });
                //Set the comments path for the Swagger JSON and UI
                //Next, add code to include XML comments in ConfigureService method. Like:
                options.IncludeXmlComments(XmlCommentsFilePath());
            });
            //Starting from ASP.NET Core 3.0 server option AllowSynchronousIO is false by default.
            //So that we need to change it to true if the endpoint calls synchronous methods.
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Global exception handeling middleware
            app.UseGlobalExceptionMiddleware();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //set the Swagger endpoint to a relative path using the ./ prefix. For example, ./swagger/v1/swagger.json
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Task API V1");
                //To serve the Swagger UI at the app's root (http://localhost:<port>/), set the RoutePrefix property to an empty string:
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }

        //This code to get the XML path will work in your local environment as well as in the production environment.
        //But it needs further implementation to show the error message if this file is missing.
        private static string XmlCommentsFilePath()
        {
            try
            {
                var basePath = AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "TestAssignment.WebAPI.xml");
                return xmlPath;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
        }
    }
}
