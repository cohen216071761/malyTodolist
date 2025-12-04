using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

// -------------------- BUILD --------------------
var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------

// DbContext עם MySQL
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        new MySqlServerVersion(new Version(8, 0, 44))
    )
);

// CORS - מאפשר רק ל‑React frontend שלך גישה
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("https://todoapi-client-igk3.onrender.com") // כתובת ה־frontend שלך
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------- MIDDLEWARE --------------------

// CORS צריך להיות **לפני כל ה‑endpoints**
app.UseCors("AllowReactApp");

app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();

// -------------------- ROUTES --------------------

// Get כל המשימות
app.MapGet("/tasks", async (ToDoDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});

// Post משימה חדשה
app.MapPost("/tasks", async (ToDoTask task, ToDoDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

// Put עדכון משימה
app.MapPut("/tasks/{id}", async (int id, ToDoTask updatedTask, ToDoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();

    task.Name = updatedTask.Name;
    task.IsComplete = updatedTask.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(task);
});

// Delete מחיקת משימה
app.MapDelete("/tasks/{id}", async (int id, ToDoDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task == null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Fallback ל‑React (אחרי כל ה‑endpoints)
app.MapFallbackToFile("index.html");

app.Run();

// -------------------- MODELS --------------------
public class ToDoTask
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

// -------------------- DbContext --------------------
public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options) { }

    public DbSet<ToDoTask> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDoTask>().ToTable("Items");
    }
}
