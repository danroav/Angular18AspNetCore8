using Angular18AspNetCore8.Core.Entities;

namespace Angular18AspNetCore8.App.Common
{
  public class TodoItemMapper
  {
    public TodoItemModel? Map(TodoItem? todoItem)
    {
      if (todoItem == null)
      {
        return null;
      }
      return new TodoItemModel
      {
        Description = todoItem.Description,
        DueDate = todoItem.DueDate,
        Id = todoItem.Id,
        Status = TodoItemStatusNames.Format[(todoItem.DueDate.HasValue && todoItem.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatus.Overdue : todoItem.LastUserStatus]
      };
    }
  }
}
