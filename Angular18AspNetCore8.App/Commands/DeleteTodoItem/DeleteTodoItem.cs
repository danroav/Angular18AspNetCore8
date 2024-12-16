using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.DeleteTodoItem;
public class DeleteTodoItem : ITodoItemsHandlerInput
{
  public required int TodoItemId { get; set; }
}
