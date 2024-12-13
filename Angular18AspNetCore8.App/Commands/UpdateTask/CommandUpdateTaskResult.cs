using Angular18AspNetCore8.App.Queries.GetAllTasks;

namespace Angular18AspNetCore8.App.Commands.UpdateTask
{
  public class CommandUpdateTaskResult
  {
    public ItemResultModel Item { get; set; }
    public bool HasValidationErrors { get; set; }
    public IDictionary<string, string[]> ValidationErrors { get; set; }
  }
}
