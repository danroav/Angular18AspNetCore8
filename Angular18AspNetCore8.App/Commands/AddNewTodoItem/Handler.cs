using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem
{
  public class Handler(ITodoItemsRepository todoItemsRepository, IValidator<Command> validator, TodoItemMapper mapper) : ITodoItemsHandler<Command, Response>
  {
    public async Task<Response> Execute(Command command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new Response
        {
          HasValidationErrors = true,
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

      return new Response
      {
        HasValidationErrors = false,
        Item = mapper.Map(newTodoItem),
      };
    }
  }
}
