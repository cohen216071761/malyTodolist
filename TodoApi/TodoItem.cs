namespace ToDoApi;

public class TodoItem
{
    public int Id { get; set; }
    // *** הוחלף Title ב-Name כדי להתאים ל-DB ול-Program.cs ***
    public string Name { get; set; } = "";
    public bool IsCompleted { get; set; }
}