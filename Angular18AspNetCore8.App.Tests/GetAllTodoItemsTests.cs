using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class GetAllTodoItemsTests
  {
    readonly Mock<ITodoItemsRepository> mockTodoItemsRepository;
    readonly Handler testHandler;
    readonly TodoItemMapper mapper = new();
    public GetAllTodoItemsTests()
    {
      mockTodoItemsRepository = new Mock<ITodoItemsRepository>();
      testHandler = new Handler(mockTodoItemsRepository.Object, mapper);
    }
    [Fact]
    public async Task GetsAllTodoItemsNoError()
    {
      //Arrange
      List<TodoItem> givenTaskEntities = [new TodoItem
      { Id=1, Description="A", DueDate=DateTime.Now.ToDateTimeOffset(), LastUserStatus= TodoItemStatus.ToDo },
        new TodoItem
      { Id=2, Description="B", DueDate=DateTime.Now.ToDateTimeOffset(), LastUserStatus= TodoItemStatus.InProgress }, ];
      mockTodoItemsRepository.Setup(x => x.GetAll()).ReturnsAsync(givenTaskEntities);
      var expectedResult = new Response
      {
        Count = 2,
        Items = givenTaskEntities.Select(e => new TodoItemModel
        {
          Description = e.Description,
          DueDate = e.DueDate,
          Id = e.Id,
          Status = (e.DueDate.HasValue && e.DueDate.Value < DateTimeOffset.Now) ? TodoItemStatusNames.Format[TodoItemStatus.Overdue] : TodoItemStatusNames.Format[e.LastUserStatus]
        }),
        Message = "2 tasks retrieved"
      };
      //Act
      var actualResult = await testHandler.Execute(new Query());
      //Assert
      mockTodoItemsRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task GetsAllTasksWithError()
    {
      //Arrange
      var expectedError = "Some error description";
      mockTodoItemsRepository.Setup(x => x.GetAll()).ThrowsAsync(new Exception(expectedError));

      //Act
      var actualResult = () => testHandler.Execute(new Query());

      //Assert
      await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
    }
  }
}