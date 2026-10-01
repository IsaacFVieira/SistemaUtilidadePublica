using SistemaUtilidadePublicaAPI.Data;
using SistemaUtilidadePublicaAPI.Data.Repositories;
using SistemaUtilidadePublicaAPI.Middleware;
using SistemaUtilidadePublicaAPI.Services.AI;
using SistemaUtilidadePublicaAPI.Services.Authentication;
using SistemaUtilidadePublicaAPI.Services.Content;
using SistemaUtilidadePublicaAPI.Services.EmergencyContact;

var builder = WebApplication.CreateBuilder(args);


// ==========================================
// SERVICES
// ==========================================

builder.Services.AddControllers();

builder.Services.AddOpenApi();


// Data
builder.Services.AddScoped<SqlConnectionFactory>();


// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<EmergencyContactRepository>();
builder.Services.AddScoped<LocationRepository>();
builder.Services.AddScoped<ContentService>();
builder.Services.AddScoped<ContentRepository>();
builder.Services.AddHttpClient<AIService>();


// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmergencyContactService>();



// ==========================================
// BUILD
// ==========================================

var app = builder.Build();


// ==========================================
// HTTP REQUEST PIPELINE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Middleware global de exceções
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();