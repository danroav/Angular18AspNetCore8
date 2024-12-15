
using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Queries.GetAllTodoItems;

public class GetAllTodoITemsHandler(ITodoItemsRepository todoItemsRepository, TodoItemMapper mapper) : ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult>
{
  public async Task<GetAllTodoItemsResult> Execute(GetAllTodoITems query)
  {
    var entities = await todoItemsRepository.GetAll();

    var items = entities.Select(e => mapper.Map(e)).ToList();

    return new GetAllTodoItemsResult
    {
      Items = items,
      Message = $"{items.Count} tasks retrieved"
    };
  }
}
