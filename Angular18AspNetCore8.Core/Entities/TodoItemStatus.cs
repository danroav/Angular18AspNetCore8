namespace Angular18AspNetCore8.Core.Entities;
public enum TodoItemStatus
{
  ToDo = 0,
  InProgress = 1,
  Done = 2,
  Overdue = 3
}

public class TodoItemStatusNames
{
  private static readonly Dictionary<TodoItemStatus, string> format = new() {
    { TodoItemStatus.Done, "Done" },
    { TodoItemStatus.ToDo, "To-do" },
    { TodoItemStatus.Overdue, "Overdue" },
    { TodoItemStatus.InProgress, "In progress" }
  };
  private static readonly Dictionary<string, TodoItemStatus> parse = new() {
    { "Done",TodoItemStatus.Done },
    { "To-do",TodoItemStatus.ToDo },
    { "Overdue",TodoItemStatus.Overdue },
    { "In progress",TodoItemStatus.InProgress }
  };

  public static Dictionary<TodoItemStatus, string> Format => format;
  public static Dictionary<string, TodoItemStatus> Parse => parse;
}