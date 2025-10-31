
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

            //CORS POLICY
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
            });




            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Register MqttService as a Singleton
            builder.Services.AddSingleton<MqttService>();

            builder.Services.AddHostedService<MqttService>(provider => provider.GetRequiredService<MqttService>());
            builder.Services.AddScoped<ISensorReadingStore, SensorReadingSqlStore>();
            //builder.Services.AddDbContext<SmartHydroDbContext>(options =>
            //     options.UseMySql(
            //        builder.Configuration.GetConnectionString("DefaultConnection"),
            //        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
            
            // Database connection
            builder.Services.AddDbContext<SmartHydroDbContext>(options =>
            {
                options.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING"));
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
            builder.Services.AddSingleton<LiveSensorCache>();
            builder.Services.AddSingleton<LiveHardwareStatusCache>();
            builder.Services.AddSingleton<AIEventCache>();
            builder.Services.AddSingleton<LiveTentInformationCache>();
            builder.Services.AddHostedService<MqttService>();

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<SmartHydroDbContext>();
                context.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins); //new Line
            app.UseAuthorization();
            app.MapControllers(); //New Line



            app.Run();
        }
    }
}
