namespace Angular18AspNetCore8.Core.Entities;
public enum TodoTaskStatus
{
  ToDo = 0,
  InProgress = 1,
  Done = 2,
  Overdue = 3
}

public class TodoTaskStatusNames
{
  private static readonly Dictionary<TodoTaskStatus, string> format = new() {
    { TodoTaskStatus.Done, "Done" },
    { TodoTaskStatus.ToDo, "To-do" },
    { TodoTaskStatus.Overdue, "Overdue" },
    { TodoTaskStatus.InProgress, "In progress" }
  };
  private static readonly Dictionary<string, TodoTaskStatus> parse = new() {
    { "Done",TodoTaskStatus.Done },
    { "To-do",TodoTaskStatus.ToDo },
    { "Overdue",TodoTaskStatus.Overdue },
    { "In progress",TodoTaskStatus.InProgress }
  };

  public static Dictionary<TodoTaskStatus, string> Format => format;
  public static Dictionary<string, TodoTaskStatus> Parse => parse;
}