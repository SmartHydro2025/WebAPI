
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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // FIX: Register MqttService as a Singleton
            builder.Services.AddSingleton<MqttService>();

            builder.Services.AddHostedService<MqttService>(provider => provider.GetRequiredService<MqttService>());
            builder.Services.AddScoped<ISensorReadingStore, SensorReadingSqlStore>();
            builder.Services.AddDbContext<SmartHydroDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSQL")));
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            builder.Services.AddSingleton<LiveSensorCache>();
            builder.Services.AddSingleton<LiveHardwareStatusCache>();
            builder.Services.AddSingleton<AIEventCache>();
            builder.Services.AddSingleton<LiveTentInformationCache>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers(); //New Line



            app.Run();
        }
    }
}
