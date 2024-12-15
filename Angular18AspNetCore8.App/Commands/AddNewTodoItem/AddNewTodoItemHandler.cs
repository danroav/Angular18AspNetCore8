using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem
{
  public class AddNewTodoItemHandler(ITodoItemsRepository todoItemsRepository, IValidator<AddNewTodoItem> validator, TodoItemMapper mapper) : ITodoItemsHandler<AddNewTodoItem, AddNewTodoItemResult>
  {
    public async Task<AddNewTodoItemResult> Execute(AddNewTodoItem command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new AddNewTodoItemResult
        {
          Message = "Todo item should be valid",
          Item = new TodoItemModel
          {
            Description = command.Description,
            DueDate = command.DueDate,
            Status = command.Status
          },
          ValidationErrors = result.ToDictionary()
        };
      }
      var newTodoItem = await todoItemsRepository.AddNew(command.Description, command.DueDate, TodoItemStatusNames.Parse[command.Status]);

      await todoItemsRepository.SaveChanges();

      return new AddNewTodoItemResult
      {
        Message = "Todo item created successfully",
        Item = mapper.Map(newTodoItem),
      };
    }
  }
}
