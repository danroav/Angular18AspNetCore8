using Angular18AspNetCore8.App.Commands.DeleteTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests;

public class DeleteTodoItemTests
{
    readonly Mock<ITodoItemsRepository> mockTodoItemsRepository;
    readonly DeleteTodoItemHandler testHandler;
    public DeleteTodoItemTests()
    {
        mockTodoItemsRepository = new Mock<ITodoItemsRepository>();
        testHandler = new DeleteTodoItemHandler(mockTodoItemsRepository.Object);
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
    [Fact]
    public async Task DeleteTodoItemWithValidationErrors()
    {
        //Arrange
        var givenCommand = new DeleteTodoItem
        {
            TodoItemId = 1112
        };
        var expectedResult = new DeleteTodoItemResult
        {
            Message = "Validation failed",
            ValidationErrors = new Dictionary<string, string[]>
            {
                {"id",["No item found to delete"] }
            }
        };
        mockTodoItemsRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([]);
        //Act
        var actualResult = await testHandler.Execute(givenCommand);

        //Assert
        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}
