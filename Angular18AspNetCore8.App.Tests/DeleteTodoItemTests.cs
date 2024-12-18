using Angular18AspNetCore8.App.Commands.DeleteTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests;

public class DeleteTodoItemTests
{
  readonly Mock<ITodoItemsRepository> mockTodoItemsRepository;
  readonly TodoItemMapper mapper;
  readonly DeleteTodoItemHandler testHandler;
  public DeleteTodoItemTests()
  {
    mapper = new TodoItemMapper();
    mockTodoItemsRepository = new Mock<ITodoItemsRepository>();
    testHandler = new DeleteTodoItemHandler(mockTodoItemsRepository.Object, mapper);
  }
  [Fact]
  public async Task DeleteTodoItemSuccess()
  {
    //Arrange

    var givenCommand = new DeleteTodoItem
    {
      TodoItemId = 234
    };
    var existingTodoItem = new TodoItem
    {
      Id = givenCommand.TodoItemId,
      Description = "Existing description",
      DueDate = DateTimeOffset.Now.AddDays(-1),
      LastUserStatus = TodoItemStatus.InProgress
    };

    mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([existingTodoItem]);
    mockTodoItemsRepository.Setup(x => x.Delete(It.IsAny<TodoItem>()));
    mockTodoItemsRepository.Setup(x => x.SaveChanges());

    var expectedResult = new DeleteTodoItemResult
    {
      Item = mapper.Map(existingTodoItem),
      Message = "Delete successful",
    };
    //Act
    var actualResult = await testHandler.Execute(givenCommand);
    //Assert
    mockTodoItemsRepository.VerifyAll();
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
  [Fact]
  public async Task DeleteTodoItemWithError()
  {
    //Arrange
    var givenCommand = new DeleteTodoItem
    {
      TodoItemId = 1111
    };
    var expectedError = "Some error description";
    mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ThrowsAsync(new Exception(expectedError));

    //Act
    var actualResult = () => testHandler.Execute(givenCommand);

    //Assert
    await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
  }
  [Theory]
  [InlineData(0)]
  [InlineData(2)]
  [InlineData(3)]
  public async Task DeleteTodoItemWhenNotExactlyOneRecordWithValidationErrors(int existingTodoItemCount)
  {
    //Arrange
    var givenTodoItemId = 1112;
    var givenCommand = new DeleteTodoItem
    {
      TodoItemId = givenTodoItemId
    };
    var givenExistingTodoItem = new TodoItem
    {
      Description = "existing description",
      DueDate = null,
      Id = givenTodoItemId
    };
    List<TodoItem> givenExistingTodoItems = [];
    for (int i = 0; i < existingTodoItemCount; i++)
    {
      givenExistingTodoItems.Add(givenExistingTodoItem);
    }

    var expectedResult = new DeleteTodoItemResult
    {
      Item = null,
      Message = $"{existingTodoItemCount} todo item(s) found",
      ValidationErrors = new Dictionary<string, string[]>
      {
        {"Item.Id",["No todo items or more than one todo item correspondence to delete"] }
      }
    };
    mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([.. givenExistingTodoItems]);

    //Act
    var actualResult = await testHandler.Execute(givenCommand);

    //Assert
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
}
