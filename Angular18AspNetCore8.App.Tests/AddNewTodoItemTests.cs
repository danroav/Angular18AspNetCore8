using Angular18AspNetCore8.App.Commands.AddNewTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class AddNewTodoItemTests
  {
    readonly Mock<ITodoItemsRepository> mockTodoItemsRepository;
    readonly AddNewTodoItemHandler testHandler;
    readonly AddNewTodoItemValidator validator = new();
    readonly TodoItemMapper mapper = new();
    public AddNewTodoItemTests()
    {
      mockTodoItemsRepository = new Mock<ITodoItemsRepository>();
      testHandler = new AddNewTodoItemHandler(mockTodoItemsRepository.Object, validator, mapper);
    }
    [Fact]
    public async Task AddNewTodoItemSuccess()
    {
      //Arrange
      var givenStatus = TodoItemStatus.ToDo;
      var givenCommand = new AddNewTodoItem
      {
        Description = "Some description",
        DueDate = null,
        Status = TodoItemStatusNames.Format[givenStatus],
      };
      var newTodoITem = new TodoItem { Id = 1000, Description = givenCommand.Description, DueDate = givenCommand.DueDate, LastUserStatus = givenStatus };
      mockTodoItemsRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoItemStatus>())).ReturnsAsync(newTodoITem);
      mockTodoItemsRepository.Setup(x => x.SaveChanges());
      var expectedResult = new AddNewTodoItemResult
      {
        Message = "Todo item created successfully",
        Item = mapper.Map(newTodoITem),
      };
      //Act
      var actualResult = await testHandler.Execute(givenCommand);
      //Assert
      mockTodoItemsRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task AddNewTodoItemWithError()
    {
      //Arrange
      var givenCommand = new AddNewTodoItem()
      {
        Description = "Some description",
        DueDate = DateTimeOffset.Now.AddDays(1),
        Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo]
      };
      var expectedError = "Some error description";
      mockTodoItemsRepository.Setup(x => x.AddNew(It.IsAny<string>(), It.IsAny<DateTimeOffset?>(), It.IsAny<TodoItemStatus>())).ThrowsAsync(new Exception(expectedError));

      //Act
      var actualResult = () => testHandler.Execute(givenCommand);

      //Assert
      await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
    }
    [Theory]
    [InlineData("", "Description is required", -1, "Due Date should be in the future", "", "Status should be valid")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123456", "Description should not exceed 255 chars", -1, "Due Date should be in the future", "", "Status should be valid")]
    public async Task AddNewTodoItemWithValidationErrors(string givenDescription, string givenDescriptionError, int givenDueDateDaysAdd, string givenDueDateError, string givenStatus, string givenStatusError)
    {
      //Arrange
      var givenDueDate = DateTimeOffset.Now.AddDays(givenDueDateDaysAdd);
      var givenCommand = new AddNewTodoItem()
      {
        Description = givenDescription,
        DueDate = givenDueDate,
        Status = givenStatus
      };
      var expectedResult = new AddNewTodoItemResult
      {
        Message = "Todo item should be valid",
        Item = new TodoItemModel
        {
          Description = givenDescription,
          DueDate = givenDueDate,
          Id = 0,
          Status = givenStatus
        },
        ValidationErrors = new Dictionary<string, string[]>
        {
          {"Description",[givenDescriptionError] },
          {"DueDate",[givenDueDateError] },
          {"Status",[givenStatusError] }
        }
      };

      //Act
      var actualResult = await testHandler.Execute(givenCommand);

      //Assert
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
  }
}
