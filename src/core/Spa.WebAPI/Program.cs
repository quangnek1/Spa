using Microsoft.EntityFrameworkCore;
using Serilog;
using Spa.Infrastructure.Common;
using Spa.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information(messageTemplate: $"Starting {builder.Environment.EnvironmentName} up");

try
{
	// Add services to the container.
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

	builder.Services.AddControllers();
	// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
	builder.Services.AddOpenApi();

	var app = builder.Build();
 

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.MapOpenApi();

	}

	app.UseHttpsRedirection();

	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception ex)
{
	string type = ex.GetType().Name;
	if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

	Log.Fatal(ex, messageTemplate: "Unhandled exception");
}
finally
{
	Log.Information(messageTemplate: $"Shut down  {builder.Environment.EnvironmentName} complete");
	Log.CloseAndFlush();
}
