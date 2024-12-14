using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests;

public class UpdateTodoItemTests
{
  readonly Mock<ITodoItemsRepository> mockTodoItemsRepository;
  readonly Handler testHandler;
  readonly Validator validator = new();
  readonly TodoItemMapper mapper = new();
  public UpdateTodoItemTests()
  {
    mockTodoItemsRepository = new Mock<ITodoItemsRepository>();
    testHandler = new Handler(mockTodoItemsRepository.Object, validator, mapper);
  }
  [Theory]
  [InlineData(true)]
  public async Task UpdateTodoItemSuccess(bool givenUpdateDatetimeNull)
  {
    //Arrange
    var givenStatus = TodoItemStatus.ToDo;
    var givenUpdateDatetime = DateTimeOffset.Now;
    var givenExistingDatetime = DateTimeOffset.Now.AddDays(-1);
    var givenTodoItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = givenUpdateDatetimeNull ? null : givenUpdateDatetime,
      Status = TodoItemStatusNames.Format[givenStatus],
    };
    var givenCommand = new Command
    {
      Item = givenTodoItemToUpdate
    };
    var existingTodoTask = new TodoItem
    {
      Id = givenCommand.Item.Id,
      Description = "Existing description",
      DueDate = givenExistingDatetime,
      LastUserStatus = TodoItemStatus.InProgress
    };

    mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([existingTodoTask]);
    mockTodoItemsRepository.Setup(x => x.SaveChanges());

    var expectedResult = new Response
    {
      HasValidationErrors = false,
      Item = new TodoItemModel
      {
        Id = givenTodoItemToUpdate.Id,
        Description = givenTodoItemToUpdate.Description,
        DueDate = givenTodoItemToUpdate.DueDate,
        Status = (givenTodoItemToUpdate.DueDate.HasValue && givenTodoItemToUpdate.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : givenTodoItemToUpdate.Status
      }
    };
    //Act
    var actualResult = await testHandler.Execute(givenCommand);
    //Assert
    mockTodoItemsRepository.VerifyAll();
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
  [Fact]
  public async Task UpdateTodoItemWithError()
  {
    //Arrange
    var givenStatus = TodoItemStatus.ToDo;
    var givenTodoItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = null,
      Status = TodoItemStatusNames.Format[givenStatus],
    };
    var givenCommand = new Command
    {
      Item = givenTodoItemToUpdate
    };
    var expectedError = "Some error description";
    mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ThrowsAsync(new Exception(expectedError));

    //Act
    var actualResult = () => testHandler.Execute(givenCommand);

    //Assert
    await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
  }
  [Theory]
  [InlineData("", "Description is required", -1, "Due Date should be in the future", "", "Status should be valid")]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123456", "Description should not exceed 255 chars", -1, "Due Date should be in the future", "", "Status should be valid")]
  public async Task UpdateTodoItemWithValidationErrors(string givenDescription, string givenDescriptionError, int givenDueDateDaysAdd, string givenDueDateError, string givenStatus, string givenStatusError)
  {
    //Arrange
    var givenTodoItemToUpdate = new TodoItemModel
    {
      Id = 1,
      Description = givenDescription,
      DueDate = DateTimeOffset.Now.AddDays(givenDueDateDaysAdd),
      Status = givenStatus,
    };
    var givenCommand = new Command
    {
      Item = givenTodoItemToUpdate
    };
    var expectedResult = new Response
    {
      HasValidationErrors = true,
      Item = givenTodoItemToUpdate,
      ValidationErrors = new Dictionary<string, string[]>
        {
          {"Item.Description",[givenDescriptionError] },
          {"Item.DueDate",[givenDueDateError] },
          {"Item.Status",[givenStatusError] }
        }
    };

    //Act
    var actualResult = await testHandler.Execute(givenCommand);

    //Assert
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
}
