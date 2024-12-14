
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class Handler(ITodoItemsRepository todoTaskRepository) : ITodoItemsHandler<Query, Response>
{
  public async Task<Response> Execute(Query query)
  {
    var entities = await todoTaskRepository.GetAll();

    var items = entities.Select(e => new TodoItemModel
    {
      Description = e.Description,
      DueDate = e.Duedate,
      Id = e.Id,
      Status = (e.Duedate.HasValue && e.Duedate.Value < DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[e.Status]
    }).ToList();

    return new Response
    {
      Count = items.Count,
      Items = items,
      Message = $"{items.Count} tasks retrieved"
    };
  }
}
