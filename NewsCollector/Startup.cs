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
using Microsoft.OpenApi.Models;
using NewsCollector.Core;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.Services;
using Npgsql.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;

namespace NewsCollector
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
            services.AddHttpClient();
            services.AddTransient<ISourceService, SourceService>();
            services.AddTransient<IKeywordService, KeywordService>();
            services.AddTransient<INewsKeywordService, NewsKeywordService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<NewsCollectorDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), 
                x => x.MigrationsAssembly("NewsCollector.Data")));
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("newscollector", new OpenApiInfo { Title = "News Collector", Version = "1.1" });
            });
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(); 
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/newscollector/swagger.json", "News Collector");
            });
        }
    }
}
