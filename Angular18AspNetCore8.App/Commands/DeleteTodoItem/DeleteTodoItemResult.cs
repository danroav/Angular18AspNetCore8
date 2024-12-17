using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.DeleteTodoItem;
public class DeleteTodoItemResult : ITodoItemsHandlerOutput
{
  public required TodoItemModel? Item { get; set; } = null;
  public string Message { get; set; } = "";
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
}
