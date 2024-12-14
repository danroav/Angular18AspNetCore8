using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Command : ITodoItemsHandlerInput
{
  public required ItemResultModel Item { get; set; }
}
