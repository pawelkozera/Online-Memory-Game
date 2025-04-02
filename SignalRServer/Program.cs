using SignalRServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GameHub>("/gameHub");
});

app.Run();
