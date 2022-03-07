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
using RallyDakar.Configurations;
using RallyDakar.Data;
using RallyDakar.IRepository;
using RallyDakar.Repository;
using RallyDakar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RallyDakar
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
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "RallyDakar", Version = "v1" });
				c.EnableAnnotations();
			});

			services.AddDbContext<RallyDakarDbContext>(options =>
			  options.UseInMemoryDatabase(databaseName: "RallyDakar"));

			services.AddCors(o =>
			{
				o.AddPolicy("AllowAll", builder =>
				builder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());
			});

			services.AddAutoMapper(typeof(MapperInitializer));

			services.AddTransient<IUnitOfWork, UnitOfWork>();  //singleton, transient, scoped
			services.AddScoped<IRaceService, RaceService>();
			services.AddScoped<IVehicleService, VehicleService>();

			services.AddControllers().AddNewtonsoftJson(
				option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); ;
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RallyDakar v1"));

			app.UseExceptionHandler("/error");

			app.UseHttpsRedirection();

			app.UseCors("AllowAll");

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseStatusCodePages();
		}
	}
}
