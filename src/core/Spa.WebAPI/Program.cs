using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Spa.Application.Interfaces;
using Spa.Application.Services;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Repositories;
using Spa.Infrastructure.Common;
using Spa.Infrastructure.Data;
using Spa.Infrastructure.Repositories;
using Stripe;

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
	builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
	builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
	builder.Services.AddScoped<ISpaManagerService, SpaManagerService>();
	builder.Services.AddScoped<IBookingService, BookingService>();
	builder.Services.AddScoped<IPaymentService, PaymentService>();
	builder.Services.AddScoped<IAuthService, AuthService>();

	builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
		.AddJwtBearer(options =>
		{
			options.SaveToken = true;
			options.RequireHttpsMetadata = false;
			options.TokenValidationParameters = new TokenValidationParameters()
			{
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
			};
		});
	StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


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
		app.MapScalarApiReference();
	}

	//app.UseHttpsRedirection();

	app.UseAuthentication();
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
