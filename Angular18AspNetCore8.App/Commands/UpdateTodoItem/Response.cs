using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Response : ITodoTasksHandlerOutput
{
  public required ItemResultModel Item { get; set; }
  public bool HasValidationErrors { get; set; } = false;
  public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
}
