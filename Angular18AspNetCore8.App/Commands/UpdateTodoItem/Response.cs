using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Response : ITodoItemsHandlerOutput
{
  public required TodoItemModel Item { get; set; }
  public bool HasValidationErrors { get; set; } = false;
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
}
