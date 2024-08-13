using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyBook.Data;
using MyBook.Data.Services;
using MyBook.Exceptions;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration)); //*************************** Use Serilog()

var connectionString = builder.Configuration.GetConnectionString("DefualtConnectionString");


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// configure the services

builder.Services.AddTransient<BooksService>();
builder.Services.AddTransient<AuthorsService>();
builder.Services.AddTransient<PublishersService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyBookProject", Version = "v1" });
});

var app = builder.Build();


//***************************************************************************************
// Log configurations in appsetting
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config).CreateLogger();

//***************************************************************************************

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json" , "MyBookProject v1"));
}

app.UseHttpsRedirection();
// app.ConfigureBuilderInExceptionHandler();
app.ConfigureCustomExceptionHandler();

app.UseAuthorization();

app.MapControllers();

//AppDbInitializer.Seed(app);
Log.Logger.Information("Test");
app.Run();
