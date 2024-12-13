namespace Angular18AspNetCore8.App.Queries.GetAllTasks;

public class ItemResultModel
{
  public int Id { get; set; }
  public string Description { get; set; } = "";
  public string DueDate { get; set; } = "";
  public string Status { get; set; } = "";
}
