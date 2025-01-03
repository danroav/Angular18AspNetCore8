﻿using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class UpdateTodoItemValidator : AbstractValidator<UpdateTodoItem>
{
  public UpdateTodoItemValidator()
  {
    RuleFor(x => x.Item.Description).NotEmpty().WithMessage("Description is required").WithName("description");
    RuleFor(x => x.Item.Description).MaximumLength(255).WithMessage("Description should not exceed 255 chars").WithName("description");
    When(x => x.Item.DueDate.HasValue, () =>
    {
      var now = DateTimeOffset.Now;
      RuleFor(x => x.Item.DueDate).GreaterThan(now).WithMessage("Due Date should be in the future").WithName("dueDate");
    });
    RuleFor(x => x.Item.Status).Must(status => TodoItemStatusNames.Parse.ContainsKey(status) && TodoItemStatusNames.Parse[status] != TodoItemStatus.Overdue).WithMessage("Status should be valid").WithName("status");

  }
}
