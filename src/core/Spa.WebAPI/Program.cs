using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spa.Domain.Entities.Identity;
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

	// 2. THÊM ĐOẠN NÀY VÀO: Đăng ký Identity
	builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
	{
		// Bạn có thể cấu hình rule mật khẩu ở đây nếu muốn
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequireUppercase = true;
		options.Password.RequiredLength = 6;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

	// 3. Đăng ký class Seeder vào DI Container
	builder.Services.AddScoped<SpaDbInitializer>();

	builder.Services.AddControllers();
	// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
	builder.Services.AddOpenApi();

	var app = builder.Build();

	// 2. Chạy Seeder lúc khởi động ứng dụng
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		try
		{
			var seeder = services.GetRequiredService<SpaDbInitializer>();
			await seeder.SeedAsync();
		}
		catch (Exception ex)
		{
			var logger = services.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo dữ liệu mẫu.");
		}
	}


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
