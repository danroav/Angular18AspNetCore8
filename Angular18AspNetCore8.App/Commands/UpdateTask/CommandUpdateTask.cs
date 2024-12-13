using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTask;

public class CommandUpdateTask : ITodoTasksHandlerInput
{
  public required ItemResultModel Item { get; set; }
}
public class CommandUpdateTaskHandler(ITodoTasksRepository todoTasksRepository, IValidator<CommandUpdateTask> validator) : ITodoTasksHandler<CommandUpdateTask, CommandUpdateTaskResult>
{
  public async Task<CommandUpdateTaskResult> Execute(CommandUpdateTask command)
  {
    var validationResult = validator.Validate(command);

    if (!validationResult.IsValid)
    {
      return new CommandUpdateTaskResult
      {
        HasValidationErrors = true,
        Item = command.Item,
        ValidationErrors = validationResult.ToDictionary()
      };
    }

    var infoToUpdate = command.Item;

    var existingTasks = await todoTasksRepository.GetByIds([command.Item.Id]);
    var existingTask = existingTasks.Single();

    existingTask.Description = infoToUpdate.Description;
    existingTask.Duedate = infoToUpdate.DueDate;
    existingTask.Status = TodoTaskStatusNames.Parse[infoToUpdate.Status];

    await todoTasksRepository.SaveChanges();

    return new CommandUpdateTaskResult
    {
      HasValidationErrors = false,
      Item = new ItemResultModel
      {
        Description = existingTask.Description,
        DueDate = existingTask.Duedate,
        Id = command.Item.Id,
        Status = (existingTask.Duedate.HasValue && existingTask.Duedate.Value < DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[existingTask.Status]
      }
    };
  }
}
