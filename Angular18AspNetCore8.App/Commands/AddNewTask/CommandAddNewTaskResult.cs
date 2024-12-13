
namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public class CommandAddNewTaskResult
  {
    public int TaskId { get; set; }
    public IDictionary<string, string[]> ValidationErrors { get; set; } = new Dictionary<string, string[]>();
    public bool HasValidationErrors { get; set; }
  }
}
