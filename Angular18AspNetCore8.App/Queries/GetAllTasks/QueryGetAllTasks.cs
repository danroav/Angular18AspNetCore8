﻿
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Queries.GetAllTasks
{
  public class QueryGetAllTasks(ITodoTaskRepository todoTaskRepository) : IQueryGetAllTasks
  {
    public async Task<QueryGetAllTasksResult> Execute()
    {
      var entities = await todoTaskRepository.GetAll();

      var items = entities.Select(e => new ItemResultModel
      {
        Description = e.Description,
        DueDate = $"{e.Duedate:D}",
        Id = e.Id,
        Status = Enum.GetName<TodoTaskStatus>(e.Status) ?? ""
      }).ToList();

      return new QueryGetAllTasksResult
      {
        Count = items.Count,
        Items = items,
        Message = $"{items.Count} tasks retrieved"
      };
    }
  }
}
