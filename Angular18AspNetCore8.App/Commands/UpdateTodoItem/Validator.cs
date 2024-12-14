using Angular18AspNetCore8.Core.Entities;
using FluentValidation;

namespace Angular18AspNetCore8.App.Commands.UpdateTodoItem;
public class Validator : AbstractValidator<Command>
{
  public Validator()
  {
    RuleFor(x => x.Item.Description).NotEmpty().WithMessage("Description is required");
    RuleFor(x => x.Item.Description).MaximumLength(255).WithMessage("Description should not exceed 255 chars");
    When(x => x.Item.DueDate.HasValue, () =>
    {
      var now = DateTimeOffset.Now;
      RuleFor(x => x.Item.DueDate).GreaterThan(now).WithMessage("Due Date should be in the future").WithName("Item.DueDate");
    });
    RuleFor(x => x.Item.Status).Must(status => TodoTaskStatusNames.Parse.ContainsKey(status) && TodoTaskStatusNames.Parse[status] != TodoTaskStatus.Overdue).WithMessage("Status should be valid");

  }
}
