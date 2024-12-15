
using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem;
public class CreateTodoItemResult : ITodoItemsHandlerOutput
{
  public required TodoItemModel Item { get; set; }
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
    public string Message { get; set; } = "";
}
