using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northwind.WebApi.Interfaces;
using Northwind.WebApi.Models;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi
{
    public class Startup
    {
        readonly string LocalPolicy = "_localPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: LocalPolicy,
                    builder =>
                    {
                        //builder.WithOrigins()//("https://localhost:44398", "https://localhost:44398/api", "http://localhost:12904", "http://localhost:12904/api")
                        //    .AllowAnyOrigin()
                        //    .AllowAnyHeader()
                        //    .AllowAnyMethod();

                        builder.WithOrigins("https://localhost:44398")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Response caching
            services.AddResponseCaching();

            // Memory caching
            services.AddMemoryCache();

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<NorthwindContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CustomerContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Response caching
            app.UseResponseCaching();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(LocalPolicy);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                         .RequireCors(LocalPolicy);
            });
        }
    }
}
