using CompanionAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .ProjectStartup();

var app = builder.Build();

app.ConfigureProject();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
