namespace Angular18AspNetCore8.Core.Entities;
public class TodoItem : EntityBase
{
  public string Description { get; set; } = "";
  public DateTimeOffset? DueDate { get; set; } = null;
  public TodoItemStatus Status { get; set; } = TodoItemStatus.ToDo;

}

