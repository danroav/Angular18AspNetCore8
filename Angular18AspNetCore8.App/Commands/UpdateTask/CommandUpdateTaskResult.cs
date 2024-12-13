using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.UpdateTask
{
  public class CommandUpdateTaskResult : ITodoTasksHandlerOutput
  {
    public required ItemResultModel Item { get; set; }
    public bool HasValidationErrors { get; set; }
    public required IDictionary<string, string[]> ValidationErrors { get; set; }
  }
}
