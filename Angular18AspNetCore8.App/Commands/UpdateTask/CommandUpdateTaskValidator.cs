﻿using Angular18AspNetCore8.Core.Entities;
using FluentValidation;
using System.Globalization;

namespace Angular18AspNetCore8.App.Commands.UpdateTask
{
  public class CommandUpdateTaskValidator : AbstractValidator<CommandUpdateTask>
  {
    public CommandUpdateTaskValidator()
    {
      RuleFor(x => x.Item.Description).NotEmpty().WithMessage("Description is required");
      RuleFor(x => x.Item.Description).MaximumLength(255).WithMessage("Description should not exceed 255 chars");
      When(x => !string.IsNullOrEmpty(x.Item.DueDate), () =>
      {
        var now = DateTimeOffset.Now;
        RuleFor(x => DateTimeOffset.ParseExact(x.Item.DueDate, "O", CultureInfo.InvariantCulture)).GreaterThan(now).WithMessage("Due Date should be in the future").WithName("Item.DueDate");
      });
      RuleFor(x => x.Item.Status).Must(status => TodoTaskStatusNames.Parse.ContainsKey(status) && TodoTaskStatusNames.Parse[status] != TodoTaskStatus.Overdue).WithMessage("Status should be valid");

    }
  }
}