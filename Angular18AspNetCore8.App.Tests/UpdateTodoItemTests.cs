using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests;

public class UpdateTodoItemTests
{
  readonly Mock<ITodoItemsRepository> mockTodoTaskRepository;
  readonly Commands.UpdateTodoItem.Handler testUpdateTaskHandler;
  readonly Validator validator = new();
  public UpdateTodoItemTests()
  {
    mockTodoTaskRepository = new Mock<ITodoItemsRepository>();
    testUpdateTaskHandler = new Handler(mockTodoTaskRepository.Object, validator);
  }
  [Theory]
  [InlineData(true)]
  public async Task UpdateTaskSuccess(bool givenUpdateDatetimeNull)
  {
    //Arrange
    var givenTodoTaskStatus = TodoItemStatus.ToDo;
    var givenUpdateDatetime = DateTimeOffset.Now;
    var givenExistingDatetime = DateTimeOffset.Now.AddDays(-1);
    var givenItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = givenUpdateDatetimeNull ? null : givenUpdateDatetime,
      Status = TodoItemStatusNames.Format[givenTodoTaskStatus],
    };
    var givenCommandUpdateTask = new Command
    {
      Item = givenItemToUpdate
    };
    var existingTodoTask = new TodoItem
    {
      Id = givenCommandUpdateTask.Item.Id,
      Description = "Existing description",
      DueDate = givenExistingDatetime,
      Status = TodoItemStatus.InProgress
    };

    mockTodoTaskRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([existingTodoTask]);
    mockTodoTaskRepository.Setup(x => x.SaveChanges());

    var expectedResult = new Response
    {
      HasValidationErrors = false,
      Item = new TodoItemModel
      {
        Id = givenItemToUpdate.Id,
        Description = givenItemToUpdate.Description,
        DueDate = givenItemToUpdate.DueDate,
        Status = (givenItemToUpdate.DueDate.HasValue && givenItemToUpdate.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : givenItemToUpdate.Status
      }
    };
    //Act
    var actualResult = await testUpdateTaskHandler.Execute(givenCommandUpdateTask);
    //Assert
    mockTodoTaskRepository.VerifyAll();
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
  [Fact]
  public async Task UpdateTaskWithError()
  {
    //Arrange
    var givenTodoTaskStatus = TodoItemStatus.ToDo;
    var givenItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = null,
      Status = TodoItemStatusNames.Format[givenTodoTaskStatus],
    };
    var givenCommand = new Command
    {
      Item = givenItemToUpdate
    };
    var expectedError = "Some error description";
    mockTodoTaskRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ThrowsAsync(new Exception(expectedError));

    //Act
    var actualResult = () => testUpdateTaskHandler.Execute(givenCommand);

    //Assert
    await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
  }
  [Theory]
  [InlineData("", "Description is required", -1, "Due Date should be in the future", "", "Status should be valid")]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123456", "Description should not exceed 255 chars", -1, "Due Date should be in the future", "", "Status should be valid")]
  public async Task UpdateTaskWithValidationErrors(string givenDescription, string givenDescriptionError, int givenDueDateDaysAdd, string givenDueDateError, string givenStatus, string givenStatusError)
  {
    //Arrange
    var givenItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = givenDescription,
      DueDate = DateTimeOffset.Now.AddDays(givenDueDateDaysAdd),
      Status = givenStatus,
    };
    var givenCommand = new Command
    {
      Item = givenItemToUpdate
    };
    var expectedResult = new Response
    {
      HasValidationErrors = true,
      Item = givenItemToUpdate,
      ValidationErrors = new Dictionary<string, string[]>
        {
          {"Item.Description",[givenDescriptionError] },
          {"Item.DueDate",[givenDueDateError] },
          {"Item.Status",[givenStatusError] }
        }
    };

    //Act
    var actualResult = await testUpdateTaskHandler.Execute(givenCommand);

    //Assert
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
}
