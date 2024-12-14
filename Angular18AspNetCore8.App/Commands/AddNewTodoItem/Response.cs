
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem;
public class Response : ITodoItemsHandlerOutput
{
  public required ItemResultModel Item { get; set; }
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
  public bool HasValidationErrors { get; set; }
}
