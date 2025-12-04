using Microsoft.EntityFrameworkCore;

namespace ToDoApi;

public class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                "name=ToDoDB",
                new MySqlServerVersion(new Version(8, 0, 44))
            );
        }
    }
}
