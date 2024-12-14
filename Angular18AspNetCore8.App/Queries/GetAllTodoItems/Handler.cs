
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class Handler(ITodoItemsRepository todoItemsRepository) : ITodoItemsHandler<Query, Response>
{
  public async Task<Response> Execute(Query query)
  {
    var entities = await todoItemsRepository.GetAll();

    var items = entities.Select(e => new TodoItemModel
    {
      Description = e.Description,
      DueDate = e.DueDate,
      Id = e.Id,
      Status = (e.DueDate.HasValue && e.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : TodoItemStatusNames.Format[e.Status]
    }).ToList();

    return new Response
    {
      Count = items.Count,
      Items = items,
      Message = $"{items.Count} tasks retrieved"
    };
  }
}
