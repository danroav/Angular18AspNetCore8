using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class UpdateTodoItemResult : ITodoItemsHandlerOutput
{
  public required TodoItemModel Item { get; set; }
  public string Message { get; set; } = "";
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
}
