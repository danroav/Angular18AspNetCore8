namespace Angular18AspNetCore8.App.Common;

public class TodoItemModel
{
  public int Id { get; set; } = 0;
  public string Description { get; set; } = "";
  public DateTimeOffset? DueDate { get; set; } = null;
  public string Status { get; set; } = "";
}
