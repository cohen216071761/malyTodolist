using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// יש לוודא שה-Namespaces של ה-Models וה-DbContext נכללים
// אם הם מוגדרים ב-namespace אחר, יש להוסיף אותו כאן.
// for example: using YourProjectName.Models;

var builder = WebApplication.CreateBuilder(args);

// --- הגדרות פורטים עבור Render ---
// קריטי לשרתי Render המשתמשים ב-Docker
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// --- הוספת שירותים ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- הגדרת CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientAccess", // שינוי שם המדיניות
        policy =>
        {
            policy.WithOrigins(
                        // הכתובת של הלקוח שלך בפרויקט הקודם
                        "https://malytodolist.onrender.com",
                        "http://localhost:3000" // לצרכי פיתוח מקומי
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
});

// --- חיבור למסד הנתונים ---
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    var connStr = connectionString ?? "Server=localhost;Database=test;User=root;Password=root;";
    // שימוש ב-MySqlServerVersion(new Version(8, 0, 44)) כפי שהיה בקוד הקודם שלך
    options.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 44)));
});

var app = builder.Build();

// --- קריטי: CORS חייב להיות לפני כל השאר! ---
app.UseCors("AllowClientAccess"); // שימוש בשם המדיניות המעודכן

// הגדרת Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
    // אין צורך להגדיר את RoutePrefix אם משתמשים ב-app.MapGet("/", ...)
});

// --- הגדרת נתיבים (Routes) ---
// שימוש ב-/tasks כפי שהיה בקוד הקודם שלך
var apiRoutes = app.MapGroup("/tasks");

// Get כל המשימות
apiRoutes.MapGet("/", async (ToDoDbContext db) =>
{
    // שימוש ב-Tasks במקום Items
    return Results.Ok(await db.Tasks.ToListAsync());
});

// Post משימה חדשה
apiRoutes.MapPost("/", async (ToDoTask task, ToDoDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

// Put עדכון משימה
apiRoutes.MapPut("/{id}", async (int id, ToDoTask inputTask, ToDoDbContext db) =>
{
    var itemToUpdate = await db.Tasks.FindAsync(id);
    if (itemToUpdate == null) return Results.NotFound();

    itemToUpdate.Name = inputTask.Name;
    itemToUpdate.IsComplete = inputTask.IsComplete;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete מחיקת משימה
apiRoutes.MapDelete("/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Tasks.FindAsync(id);
    if (item == null) return Results.NotFound();

    db.Tasks.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// הפניה מהשורש ל-Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

// -------------------- MODELS --------------------
// אם המודלים לא הוגדרו בקובץ נפרד (כמו ToDoTask.cs), יש להוסיף אותם כאן.
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