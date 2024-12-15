using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class GetAllTodoItemsResult : ITodoItemsHandlerOutput
{
  public string Message { get; set; } = "";
  public IEnumerable<TodoItemModel> Items { get; set; } = [];
}

