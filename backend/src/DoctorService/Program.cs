using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DoctorService.Data;
using DoctorService.Services;
using DoctorService.Kafka; 
using Steeltoe.Discovery.Client;
using Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<DoctorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DoctorDb")));

// Register Kafka Producer
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

// Service Discovery
builder.Services.AddDiscoveryClient(builder.Configuration);

// Services
builder.Services.AddScoped<IDoctorService, DoctorServiceImpl>();

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations automatically with retry logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<DoctorDbContext>();

    int retries = 5;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Attempting to apply database migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
            break; // Success, exit loop
        }
        catch (Exception ex)
        {
            retries--;
            if (retries == 0)
            {
                logger.LogError(ex, "Failed to apply migrations after multiple attempts. Terminating.");
                throw;
            }
            
            logger.LogWarning($"Database not ready. Retrying in 2 seconds... ({retries} attempts left). Error: {ex.Message}");
            System.Threading.Thread.Sleep(2000);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();