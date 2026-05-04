using LinguaSwap.Api.Middleware;
using LinguaSwap.Application.Practice.Common;
using LinguaSwap.Application.Progress.GetProgressHistory;
using LinguaSwap.Application.Progress.GetProgressSummary;
using LinguaSwap.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",  // Vite
                "https://localhost:5173"  // por si usas https
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// GENERATE OpenAPI docs
builder.Services.AddOpenApi();

// Your handlers/services
builder.Services.AddScoped<LinguaSwap.Application.Practice.StartPracticeSession.StartPracticeSessionHandler>();
builder.Services.AddSingleton<InMemoryPracticeSessionStore>();
builder.Services.AddSingleton<LinguaSwap.Application.Practice.Common.InMemoryPracticeSessionStore>();
builder.Services.AddScoped<LinguaSwap.Application.Practice.GetNextPracticeWord.GetNextPracticeWordHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Practice.SubmitAttempt.SubmitAttemptHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Practice.GetSessionStats.GetSessionStatsHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.DeleteTerm.DeleteTermHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.AddTerm.AddTermHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.UpdateTerm.UpdateTermHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.DeleteVocabItem.DeleteVocabItemHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.DeleteLibrary.DeleteLibraryHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.GetLibraries.GetLibrariesHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.GetVocabItems.GetVocabItemsHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.GetVocabItemDetail.GetVocabItemDetailHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.UpdateLibrary.UpdateLibraryHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Progress.GetProgressSummary.GetProgressSummaryHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Progress.GetProgressHistory.GetProgressHistoryHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Progress.GetTopMistakes.GetTopMistakesHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Progress.GetProgressByPair.GetProgressByPairHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Progress.GetProgressByLanguage.GetProgressByLanguageHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.GetLibraryItems.GetLibraryItemsHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.GetPublicLibraries.GetPublicLibrariesHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Auth.Register.RegisterHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Auth.Login.LoginHandler>();

// EF Core
builder.Services.AddDbContext<LinguaSwapDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    options.UseNpgsql(cs);
});

// More handlers
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.CreateLibrary.CreateLibraryHandler>();
builder.Services.AddScoped<LinguaSwap.Application.Vocabulary.CreateVocabItem.CreateVocabItemHandler>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Only in development: expose docs
if (app.Environment.IsDevelopment())
{
    // JSON OpenAPI endpoint
    app.MapOpenApi();

    // 🎨 Scalar UI
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LinguaSwapDbContext>();

    // Asegura que la DB está al día con migraciones antes de seed
    db.Database.Migrate();

    SeedData.EnsureSeeded(db);
}

app.UseCors("FrontendDev");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();