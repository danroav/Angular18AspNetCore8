using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class UpdateTodoItemHandler(ITodoItemsRepository todoItemsRepository, IValidator<UpdateTodoItem> validator, TodoItemMapper mapper) : ITodoItemsHandler<UpdateTodoItem, UpdateTodoItemResult>
{
  public async Task<UpdateTodoItemResult> Execute(UpdateTodoItem command)
  {
    var validationResult = validator.Validate(command);

    if (!validationResult.IsValid)
    {
      return new UpdateTodoItemResult
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
    existingTodoItem.LastUserStatus = TodoItemStatusNames.Parse[infoToUpdate.Status];

    await todoItemsRepository.SaveChanges();

    return new UpdateTodoItemResult
    {
      HasValidationErrors = false,
      Item = mapper.Map(existingTodoItem),
    };
  }
}
