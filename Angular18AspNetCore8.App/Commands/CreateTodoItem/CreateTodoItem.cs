using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem;

public class CreateTodoItem : ITodoItemsHandlerInput
{
  public string Description { get; set; } = "";
  public DateTimeOffset? DueDate { get; set; } = null;
  public string Status { get; set; } = "";
}
