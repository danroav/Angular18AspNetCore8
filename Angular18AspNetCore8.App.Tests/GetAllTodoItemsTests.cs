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
    readonly Mock<ITodoItemsRepository> mockTodoTaskRepository;
    readonly Handler testQueryGetAllTasks;
    public GetAllTodoItemsTests()
    {
      mockTodoTaskRepository = new Mock<ITodoItemsRepository>();
      testQueryGetAllTasks = new Handler(mockTodoTaskRepository.Object);
    }
    [Fact]
    public async Task GetsAllTasksNoError()
    {
      //Arrange
      List<TodoTask> givenTaskEntities = [new TodoTask
      { Id=1, Description="A", Duedate=DateTime.Now.ToDateTimeOffset(), Status=TodoTaskStatus.ToDo },
        new TodoTask
      { Id=2, Description="B", Duedate=DateTime.Now.ToDateTimeOffset(), Status=TodoTaskStatus.InProgress }, ];
      mockTodoTaskRepository.Setup(x => x.GetAll()).ReturnsAsync(givenTaskEntities);
      var expectedResult = new Response
      {
        Count = 2,
        Items = givenTaskEntities.Select(e => new TodoItemModel
        {
          Description = e.Description,
          DueDate = e.Duedate,
          Id = e.Id,
          Status = (e.Duedate.HasValue && e.Duedate.Value < DateTimeOffset.Now) ? TodoTaskStatusNames.Format[TodoTaskStatus.Overdue] : TodoTaskStatusNames.Format[e.Status]
        }),
        Message = "2 tasks retrieved"
      };
      //Act
      var actualResult = await testQueryGetAllTasks.Execute(new Query());
      //Assert
      mockTodoTaskRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task GetsAllTasksWithError()
    {
      //Arrange
      var expectedError = "Some error description";
      mockTodoTaskRepository.Setup(x => x.GetAll()).ThrowsAsync(new Exception(expectedError));

      //Act
      var actualResult = () => testQueryGetAllTasks.Execute(new Query());

      //Assert
      await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
    }
  }
}