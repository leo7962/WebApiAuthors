using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAuthors.Context;
using WebApiAuthors.Services;

namespace WebApiAuthors;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

        //AddTransient nos da una instancia del servicio, sirve para situaciones temporales o transitorias en las que no es necesario almacenar información
        //services.AddTransient<IService, ServicioA>();
        //Se puede realizar inyección de dependencias directamente a una clase especifica
        //services.AddTransient<ServicioA>();

        //el tiempo de servicio será la misma instancia de la clase durante la misma petición HTTP,
        //services.AddScoped<IService, ServicioA>();

        //Siempre tendrá la misma instancia la clase del servicio, sirve por si tenemos información en cache o en memoria y necesitamos información de manera rápida entre todos
        services.AddTransient<IService, ServicioA>();

        services.AddTransient<ServiceTransient>();
        services.AddScoped<ServiceScoped>();
        services.AddSingleton<ServiceSingleton>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
