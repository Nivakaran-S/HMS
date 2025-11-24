var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "This service is replaced by the Steeltoe Eureka Docker image in production.");

app.Run();