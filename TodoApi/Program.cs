using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הגדרת פורטים עבור Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// הוספת שירותים
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientAccess",
        policy =>
        {
            policy.WithOrigins(
                        "https://malytodolist.onrender.com",
                        "http://localhost:3000"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});

// חיבור למסד הנתונים
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
Console.WriteLine($"Connection String: {connectionString}");

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    var connStr = connectionString ?? "Server=localhost;Database=test;User=root;Password=root;";
    options.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 44)));
});

// הוספת logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// CORS חייב להיות ראשון
app.UseCors("AllowClientAccess");

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
});

// Middleware לטיפול בשגיאות
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

var apiRoutes = app.MapGroup("/tasks");

// Get כל המשימות
apiRoutes.MapGet("/", async (ToDoDbContext db, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Fetching all tasks");
        var tasks = await db.Tasks.ToListAsync();
        logger.LogInformation($"Found {tasks.Count} tasks");
        return Results.Ok(tasks);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error fetching tasks");
        return Results.Problem(ex.Message);
    }
});

// Post משימה חדשה
apiRoutes.MapPost("/", async (TodoItem task, ToDoDbContext db, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation($"Adding new task: {task.Name}");

        if (string.IsNullOrWhiteSpace(task.Name))
        {
            return Results.BadRequest("Task name is required");
        }

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        logger.LogInformation($"Task added with ID: {task.Id}");
        return Results.Created($"/tasks/{task.Id}", task);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error adding task");
        return Results.Problem(ex.Message);
    }
});

// Put עדכון משימה
apiRoutes.MapPut("/{id}", async (int id, TodoItem inputTask, ToDoDbContext db, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation($"Updating task {id}");

        var itemToUpdate = await db.Tasks.FindAsync(id);
        if (itemToUpdate == null)
        {
            logger.LogWarning($"Task {id} not found");
            return Results.NotFound();
        }

        itemToUpdate.Name = inputTask.Name;
        itemToUpdate.IsComplete = inputTask.IsComplete;

        await db.SaveChangesAsync();
        logger.LogInformation($"Task {id} updated successfully");
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error updating task {id}");
        return Results.Problem(ex.Message);
    }
});

// Delete מחיקת משימה
apiRoutes.MapDelete("/{id}", async (int id, ToDoDbContext db, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation($"Deleting task {id}");

        var item = await db.Tasks.FindAsync(id);
        if (item == null)
        {
            logger.LogWarning($"Task {id} not found");
            return Results.NotFound();
        }

        db.Tasks.Remove(item);
        await db.SaveChangesAsync();

        logger.LogInformation($"Task {id} deleted successfully");
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error deleting task {id}");
        return Results.Problem(ex.Message);
    }
});

// הפניה מהשורש ל-Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// בדיקת חיבור למסד נתונים
app.MapGet("/health", async (ToDoDbContext db) =>
{
    try
    {
        await db.Database.CanConnectAsync();
        return Results.Ok(new { status = "healthy", database = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();