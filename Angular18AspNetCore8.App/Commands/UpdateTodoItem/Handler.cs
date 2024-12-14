using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Handler(ITodoItemsRepository todoItemsRepository, IValidator<Command> validator) : ITodoItemsHandler<Command, Response>
{
  public async Task<Response> Execute(Command command)
  {
    var validationResult = validator.Validate(command);

    if (!validationResult.IsValid)
    {
      return new Response
      {
        HasValidationErrors = true,
        Item = command.Item,
        ValidationErrors = validationResult.ToDictionary()
      };
    }

    var infoToUpdate = command.Item;

    var existingTodoItems = await todoItemsRepository.GetByIds([command.Item.Id]);
    var existingTodoItem = existingTodoItems.Single();

    existingTodoItem.Description = infoToUpdate.Description;
    existingTodoItem.DueDate = infoToUpdate.DueDate;
    existingTodoItem.Status = TodoItemStatusNames.Parse[infoToUpdate.Status];

    await todoItemsRepository.SaveChanges();

    return new Response
    {
      HasValidationErrors = false,
      Item = new TodoItemModel
      {
        Description = existingTodoItem.Description,
        DueDate = existingTodoItem.DueDate,
        Id = command.Item.Id,
        Status = (existingTodoItem.DueDate.HasValue && existingTodoItem.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : TodoItemStatusNames.Format[existingTodoItem.Status]
      }
    };
  }
}
