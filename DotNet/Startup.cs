﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNet.Services;
using Microsoft.Extensions.Configuration;
using DotNet.Models;

namespace DotNet
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")

                //allow configuration to be over ridden in different formats => project properties to add environment variables(debug)
                .AddEnvironmentVariables();

            //builder returns IConfigurationRoot - add IConfigurationRoot to AppController constructor
            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            if (_env.IsEnvironment("Development"))
            {
                //AddTransient supplies an interface (IMailService) that can be fulfilled by a certain class (DebugMailService)
                services.AddScoped<IMailService, DebugMailService>(); ;
                //AddTransient creates an instance of DebugMailService as soon as it needs it and can keep it cached
                //AddScoped creates an instance of DebugMailService for each set of requests, can be reused but only within the scope of a single request
                //AddSingleton creates one instance on first request and same instance is used throughout
            }
            else
            {
                //Implement real Mail Service
            }

            services.AddDbContext<WorldContext>();

            services.AddScoped<IWorldRepository, WorldRepository>();

            services.AddTransient<WorldContextSeedData>();

            services.AddLogging();

            //aspdotnetcore requires the use of dependecy injection
            //this ConfigureServices is responsible for the setup of the service container
            //the container holds all the different services to the different parts that the application requires
            //mvc requires a number of services, interfaces, instances of classes in order to do its job, mvc services are registered here
            services.AddMvc();
            //app.UseMvc(); needs to be used in Configure
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
            }
            

            //the order of these are important for middleware execution/requests
            app.UseStaticFiles();

            //enables MVC to listen for operation
            //tie up routes with their controllers by using lambda to configure mvc
            //create a route by using the config object => MapRoute method
            app.UseMvc(config =>
            {
                //MapRoute creates different routes that will take a pattern of a url of different options, the specified url in the request is mapped to specific controllers
                config.MapRoute(
                    //fallback when no specific route is used
                    name: "Default",

                    //pattern matching | maps to specific controller | the first section of url is the name of controller | second is name of action | third part is otional id
                    template: "{controller}/{action}/{id?}",

                    //if no parameters are supplied then set these as default, the root path will map directly to the index method 
                    defaults: new { controller = "App", action = "Index" }
                    );
            });
            //Configure is synchronous, Wait can be used to make seeder async
            seeder.EnsureSeedData().Wait();
        }
    }
}
