using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NewsCollector.Core;
using NewsCollector.Core.Services;
using NewsCollector.Data;
using NewsCollector.Helpers;
using NewsCollector.Services;
using Npgsql.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using static NewsCollector.Helpers.JwtMiddleware;

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
            services.AddCors();
            services.AddControllers();
            services.AddHttpClient();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddTransient<IUserService, UserService>();
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

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id= "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            // services.AddAuthentication(option =>
            // {
            //     option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //     option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            // }).AddJwtBearer(options =>
            // {
            //     options.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateIssuer = true,
            //         ValidateAudience = true,
            //         ValidateLifetime = false,
            //         ValidateIssuerSigningKey = true,
            //         ValidIssuer = Configuration["JwtToken:Issuer"],
            //         ValidAudience = Configuration["JwtToken:Issuer"],
            //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtToken:SecretKey"])) //Configuration["JwtToken:SecretKey"]
            //     };
            // });
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

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/newscollector/swagger.json", "News Collector");
            });

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                await context.Response.WriteAsJsonAsync(new { error = exception.Message });
            }));
        }
    }
}
