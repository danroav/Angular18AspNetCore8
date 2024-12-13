﻿namespace Angular18AspNetCore8.App.Queries.GetAllTasks;

public class ItemResultModel
{
  public int Id { get; set; }
  public string Description { get; set; } = "";
  public DateTimeOffset? DueDate { get; set; } = null;
  public string Status { get; set; } = "";
}
