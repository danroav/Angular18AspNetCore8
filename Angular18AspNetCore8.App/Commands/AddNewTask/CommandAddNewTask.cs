namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public class CommandAddNewTask
  {
    public string Description { get; set; } = "";
    public DateTimeOffset? DueDate { get; set; } = null;
    public string Status { get; set; } = "";
  }
}
