using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.UpdateTask
{
  public class CommandUpdateTask : ITodoTasksHandlerInput
  {
    public required ItemResultModel Item { get; set; }
  }

}
