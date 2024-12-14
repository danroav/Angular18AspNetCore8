using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Command : ITodoItemsHandlerInput
{
  public required TodoItemModel Item { get; set; }
}
