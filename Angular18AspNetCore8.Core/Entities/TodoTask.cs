﻿namespace Angular18AspNetCore8.Core.Entities;
public class TodoTask : EntityBase
{
  public string Description { get; set; } = "";
  public DateTimeOffset Duedate { get; set; }
  public TodoTaskStatus Status { get; set; }

}

