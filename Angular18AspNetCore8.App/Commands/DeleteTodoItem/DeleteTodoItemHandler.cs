using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Commands.DeleteTodoItem;
public class DeleteTodoItemHandler(ITodoItemsRepository todoItemsRepository, TodoItemMapper mapper) : ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>
{
  public async Task<DeleteTodoItemResult> Execute(DeleteTodoItem command)
  {
    var validationErrors = new Dictionary<string, string[]>();
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
      message = $"{existingTodoItems.Count} todo item(s) found";
      validationErrors["Item.Id"] = ["No todo items or more than one todo item correspondence to delete"];
    }

    return new DeleteTodoItemResult
    {
      Item = mapper.Map(todoItem),
      Message = message,
      ValidationErrors = validationErrors
    };
  }
}
