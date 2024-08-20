using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyBook.Data;
using MyBook.Data.Models;
using MyBook.Data.Services;
using MyBook.Exceptions;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

//***************************************************************************************
// Log configurations in appsetting
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

Serilog.Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config).CreateLogger();

//***************************************************************************************
//Add  Token validation parameters:
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWT:Secret"])),

    ValidateIssuer = true,
    ValidIssuer = config["JWT:Issuer"],

    ValidateAudience = true,
    ValidAudience = config["JWT:Audience"],

    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton<TokenValidationParameters>();

//Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Add Authentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
//Add JWT Bearer
.AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = tokenValidationParameters;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyBookProject", Version = "v1" });
});

var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json" , "MyBookProject v1"));
}

app.UseHttpsRedirection();
app.ConfigureBuilderInExceptionHandler();
//app.ConfigureCustomExceptionHandler();

//Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//AppDbInitializer.Seed(app);
Serilog.Log.Logger.Information("Test");
app.Run();
