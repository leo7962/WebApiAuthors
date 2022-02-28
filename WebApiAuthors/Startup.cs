using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApiAuthors.Context;
using WebApiAuthors.Filters;
using WebApiAuthors.Helpers;
using WebApiAuthors.Middlewares;
using WebApiAuthors.Services;

namespace WebApiAuthors;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
                options.Conventions.Add(new SwaggerGroupByVersion());
            })
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            .AddNewtonsoftJson();
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["keyjwt"])),
                ClockSkew = TimeSpan.Zero
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApiAutores", Version = "v1"});
            x.SwaggerDoc("v2", new OpenApiInfo {Title = "WebApiAutores", Version = "v2"});
            x.OperationFilter<AddParameterHateoas>();
            x.OperationFilter<AddParameterXVersion>();
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddAutoMapper(typeof(MappingProfiles));
        services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAdmin", policy => policy.RequireClaim("isAdmin"));
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader();
            });
        });

        services.AddDataProtection();
        services.AddTransient<HashService>();
        services.AddTransient<GenerateLink>();
        services.AddTransient<HateoasAuthorFilterAttribute>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        // Configure the HTTP request pipeline.

        app.UseResponseLogging();


        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "webapiAutores V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "webapiAutores V2");
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
