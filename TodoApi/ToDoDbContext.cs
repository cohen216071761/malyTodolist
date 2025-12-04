using Microsoft.EntityFrameworkCore;
// וודאי שה-namespace של המודל נכון
// using YourProjectName.Models; 

namespace TodoApi;

public class ToDoDbContext : DbContext
{
    // הקונסטרקטור מקבל את ההגדרות מה-DI ב-Program.cs
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    // וודאי שהמודל TodoItem קיים ומוגדר נכון
    public DbSet<TodoItem> Tasks { get; set; } = null!;

    // הגדרת שם הטבלה במסד הנתונים (כפי שהיה בקוד הקודם)
   
}