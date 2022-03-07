using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RallyDakar.Data;
using RallyDakar.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RallyDakar
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

			Log.Logger = new LoggerConfiguration()
			.WriteTo.File(
			path: path, 
			outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.ff zzz} [{Level:u3}] {Message:lj} {NewLine} {Exception}",
			rollingInterval: RollingInterval.Day,
			restrictedToMinimumLevel: LogEventLevel.Information
			).CreateLogger();

			try
			{
				Log.Information("Application is starting...");
				//needed for easier testing - adding sample data
				var host = CreateHostBuilder(args).Build();
				using (var scope = host.Services.CreateScope())
				{
					var services = scope.ServiceProvider;
					var context = services.GetRequiredService<RallyDakarDbContext>();
					DataGenerator.Initialize(services);	//creating sample data
				}
				host.Run();
			}
			catch (Exception ex)
			{

				Log.Fatal(ex, "Application failed to start");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
