namespace Angular18AspNetCore8.App.Queries.GetAllTasks;

public class QueryGetAllTasksResult
{
  public int Count { get; set; }
  public string Message { get; set; } = "";
  public IEnumerable<ItemResultModel> Items { get; set; } = [];
}

