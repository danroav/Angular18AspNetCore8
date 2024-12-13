
namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public class CommandAddNewTaskResult
  {
    public int TaskId { get; set; }
    public Dictionary<string, string> ValidationErrors { get; set; }
    public bool HasValidationErrors { get; set; }
  }
}
