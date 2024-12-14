using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class Response : ITodoItemsHandlerOutput
{
  public int Count { get; set; }
  public string Message { get; set; } = "";
  public IEnumerable<TodoItemModel> Items { get; set; } = [];
}

