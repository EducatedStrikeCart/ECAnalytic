using ECAnalytic.Server.Data.Repositories;
using ECAnalytic.Server.Infrastructure.Contracts;
using ECAnalytic.Server.Infrastructure.Entities;
using ECAnalytic.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy("DefaultPolicy",
		policy =>
		{
			policy.WithOrigins("https://localhost:55555")
					.AllowAnyMethod()
					.AllowAnyHeader();
		});
});
builder.Services.AddProblemDetails();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IBaseEntityFrameworkRepository<Order, OrderDto, Guid>,
								BaseEntityFrameworkRepository<Order, OrderDto, Guid>>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
