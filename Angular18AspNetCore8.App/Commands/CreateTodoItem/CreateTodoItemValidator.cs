using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.CreateTodoItem;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItem>
{
  public CreateTodoItemValidator()
  {
    RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required").WithName("description");
    RuleFor(x => x.Description).MaximumLength(255).WithMessage("Description should not exceed 255 chars").WithName("description");
    When(x => x.DueDate.HasValue, () =>
    {
      RuleFor(x => x.DueDate).GreaterThan(DateTimeOffset.Now).WithMessage("Due Date should be in the future").WithName("dueDate");
    });
    RuleFor(x => x.Status).Must(status => TodoItemStatusNames.Parse.ContainsKey(status) && TodoItemStatusNames.Parse[status] != TodoItemStatus.Overdue).WithMessage("Status should be valid").WithName("status");
  }
}
