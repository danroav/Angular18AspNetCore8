using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Commands.DeleteTodoItem;
public class DeleteTodoItemHandler(ITodoItemsRepository todoItemsRepository, TodoItemMapper mapper) : ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>
{
  public async Task<DeleteTodoItemResult> Execute(DeleteTodoItem command)
  {
    var validationErrors = new Dictionary<string, string[]>() { { "id", ["No item found to delete"] } };
    var message = "Validation failed";
    TodoItem? todoItem = null;
    var existingTodoItems = await todoItemsRepository.GetByIds([command.TodoItemId]);
    if (existingTodoItems.Count == 1)
    {
      todoItem = existingTodoItems.Single();
      todoItemsRepository.Delete(todoItem);
      await todoItemsRepository.SaveChanges();
      message = "Delete successful";
      validationErrors.Clear();
    }
    if (existingTodoItems.Count != 1)
    {
      message = $"{existingTodoItems.Count} found";
      validationErrors["id"] = ["There should be exactly one todo item to delete"];
    }

    return new DeleteTodoItemResult
    {
      Item = mapper.Map(todoItem),
      Message = message,
      ValidationErrors = validationErrors
    };
  }
}
