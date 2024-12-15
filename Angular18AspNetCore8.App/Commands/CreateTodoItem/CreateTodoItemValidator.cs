using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.AddNewTodoItem;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItem>
{
  public CreateTodoItemValidator()
  {
    RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
    RuleFor(x => x.Description).MaximumLength(255).WithMessage("Description should not exceed 255 chars");
    When(x => x.DueDate.HasValue, () =>
    {
      RuleFor(x => x.DueDate).GreaterThan(DateTimeOffset.Now).WithMessage("Due Date should be in the future");
    });
    RuleFor(x => x.Status).Must(status => TodoItemStatusNames.Parse.ContainsKey(status) && TodoItemStatusNames.Parse[status] != TodoItemStatus.Overdue).WithMessage("Status should be valid");
  }
}
