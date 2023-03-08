using SecurePassword;
using SecurePassword.DAL;
using SecurePassword.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", false, true);

// Our singletons
builder.Services.AddSingleton<DatabaseManager>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

#if DEBUG
Initializer.Initialize(app);
#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
