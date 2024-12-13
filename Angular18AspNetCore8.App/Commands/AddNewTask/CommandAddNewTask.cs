using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public class CommandAddNewTask
  {
    public string Description { get; set; } = "";
    public DateTimeOffset? DueDate { get; set; } = null;
    public string Status { get; set; } = "";
  }
  public class CommandAddNewTaskHandler(ITodoTaskRepository todoTaskRepository, IValidator<CommandAddNewTask> validator) : ICommandHandler<CommandAddNewTask, CommandAddNewTaskResult>
  {
    public async Task<CommandAddNewTaskResult> Execute(CommandAddNewTask command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new CommandAddNewTaskResult
        {
          HasValidationErrors = true,
          TaskId = 0,
          ValidationErrors = result.ToDictionary()
        };
      }
      var repositoryResult = await todoTaskRepository.AddNew(command.Description, command.DueDate, TodoTaskStatusNames.Parse[command.Status]);
      return new CommandAddNewTaskResult
      {
        HasValidationErrors = false,
        TaskId = repositoryResult.Id
      };
    }
  }
}
