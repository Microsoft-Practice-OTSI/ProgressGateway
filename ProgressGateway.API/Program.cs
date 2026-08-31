using ProgressGateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

// Register Progress Service
builder.Services.AddScoped<IProgressService, ProgressService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "ProgressGatewayCors",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials();
        });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must come before controllers and hubs
app.UseCors("ProgressGatewayCors");

app.UseAuthorization();

// API Controllers
app.MapControllers();

// SignalR Hub
app.MapHub<ProgressGateway.Api.Hubs.ProgressHub>(
    "/progressHub");

app.Run();