
using Microsoft.EntityFrameworkCore;
using SmartHydro_API.Database;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;

namespace SmartHydro_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<MqttService>(); //New Line added
            builder.Services.AddScoped<ISensorReadingStore, SensorReadingSqlStore>(); //New Line added
            builder.Services.AddDbContext<SmartHydroDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSQL")));
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });  // New Line
            builder.Services.AddSingleton<LiveSensorCache>(); // New Line


            //builder.Services.AddScoped<ISensorReadingStore, SensorReadingSqlStore>(); // optional if switching storage


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers(); //New Line



            app.Run();
        }
    }
}
