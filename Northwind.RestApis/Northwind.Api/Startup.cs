﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Data;
using Northwind.Data.Models;
using Northwind.DI;
using Northwind.Services.Configuration;
using Northwind.Services.Contracts;
using Swashbuckle.AspNetCore.Swagger;

namespace Northwind.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //This configuration is needed if 'IOptions<AppData> appDataAccessor' is injected into class constructor
            //services.Configure<AppData>(Configuration.GetSection("AppData"));

            AppData appData = Configuration.GetSection("AppData").Get<AppData>();

            services.AddDbContext<MasterContext>(options => options.UseSqlServer(appData.MasterDbConnectionString));
            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(appData.NorthwindDbConnectionString));

            services.AddDependencyInjection(DiContainers.AspNetCoreDependencyInjector);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Northwind REST API",
                    Description = "A simple API for working with Northwind database",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Ivaylo Botusharov",
                        Email = string.Empty,
                        Url = "https://twitter.com/ivo.botusharov"
                    },
                    License = new License
                    {
                        Name = "MIT License",
                        Url = "https://example.com/license"
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Northwind REST API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind REST API");
                c.RoutePrefix = string.Empty;
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var masterContext = serviceScope.ServiceProvider.GetService<MasterContext>();
                var northwindContext = serviceScope.ServiceProvider.GetService<NorthwindContext>();

                var databaseConfigurationService = serviceScope.ServiceProvider.GetService<IDatabaseConfigurationService>();
                databaseConfigurationService.SeedData(northwindContext, masterContext);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
