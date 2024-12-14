
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Queries.GetAllTasks
{
  public class QueryGetAllTasks : ITodoItemsHandlerInput { }
  public class QueryGetAllTasksHandler(ITodoItemsRepository todoTaskRepository) : ITodoItemsHandler<QueryGetAllTasks, QueryGetAllTasksResult>
  {
    public async Task<QueryGetAllTasksResult> Execute(QueryGetAllTasks query)
    {
      var entities = await todoTaskRepository.GetAll();

      var items = entities.Select(e => new ItemResultModel
      {
        Description = e.Description,
        DueDate = e.Duedate,
        Id = e.Id,
        Status = (e.Duedate.HasValue && e.Duedate.Value < DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[e.Status]
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
