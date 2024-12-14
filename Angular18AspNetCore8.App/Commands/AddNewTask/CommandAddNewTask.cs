using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public class CommandAddNewTask : ITodoTasksHandlerInput
  {
    public string Description { get; set; } = "";
    public DateTimeOffset? DueDate { get; set; } = null;
    public string Status { get; set; } = "";
  }
  public class CommandAddNewTaskHandler(ITodoTasksRepository todoTaskRepository, IValidator<CommandAddNewTask> validator) : ITodoTasksHandler<CommandAddNewTask, CommandAddNewTaskResult>
  {
    public async Task<CommandAddNewTaskResult> Execute(CommandAddNewTask command)
    {
      var result = validator.Validate(command);
      if (!result.IsValid)
      {
        return new CommandAddNewTaskResult
        {
          HasValidationErrors = true,
          Item = new ItemResultModel
          {
            Description = command.Description,
            DueDate = command.DueDate,
            Status = command.Status
          },
          ValidationErrors = result.ToDictionary()
        };
      }
      var repositoryResult = await todoTaskRepository.AddNew(command.Description, command.DueDate, TodoTaskStatusNames.Parse[command.Status]);

      await todoTaskRepository.SaveChanges();

      return new CommandAddNewTaskResult
      {
        HasValidationErrors = false,
        Item = new ItemResultModel
        {
          Description = repositoryResult.Description,
          DueDate = repositoryResult.Duedate,
          Status = (repositoryResult.Duedate.HasValue && repositoryResult.Duedate.Value > DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[repositoryResult.Status],
          Id = repositoryResult.Id
        },
      };
    }
  }
}
