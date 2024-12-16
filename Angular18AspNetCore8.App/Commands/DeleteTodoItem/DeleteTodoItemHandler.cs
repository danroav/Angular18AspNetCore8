using Angular18AspNetCore8.App.Common;

namespace Angular18AspNetCore8.App.Commands.DeleteTodoItem;
public class DeleteTodoItemHandler(ITodoItemsRepository todoItemsRepository) : ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>
{
    public async Task<DeleteTodoItemResult> Execute(DeleteTodoItem command)
    {
        var validationErrors = new Dictionary<string, string[]>(){ {"id", ["No item found to delete"]} };
        var message = "Validation failed";
        var existingTodoItems = await todoItemsRepository.GetByIds([command.TodoItemId]);
        if (existingTodoItems.Count == 1)
        {
            todoItemsRepository.Delete(existingTodoItems.Single()!);
            await todoItemsRepository.SaveChanges();
            message = "Delete successful";
            validationErrors.Clear();
        }

        return new DeleteTodoItemResult
        {
            Message = message,
            ValidationErrors = validationErrors
        };
    }
}
