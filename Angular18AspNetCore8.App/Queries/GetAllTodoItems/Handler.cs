
using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class Handler(ITodoItemsRepository todoItemsRepository, TodoItemMapper mapper) : ITodoItemsHandler<Query, Response>
{
  public async Task<Response> Execute(Query query)
  {
    var entities = await todoItemsRepository.GetAll();

    var items = entities.Select(e => mapper.Map(e)).ToList();

    return new Response
    {
      Count = items.Count,
      Items = items,
      Message = $"{items.Count} tasks retrieved"
    };
  }
}
