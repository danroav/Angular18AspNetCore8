using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class QueryGetAllTasksTests
  {
    readonly Mock<ITodoTaskRepository> mockTodoTaskRepository;
    readonly QueryGetAllTasks testQueryGetAllTasks;
    public QueryGetAllTasksTests()
    {
      mockTodoTaskRepository = new Mock<ITodoTaskRepository>();
      testQueryGetAllTasks = new QueryGetAllTasks(mockTodoTaskRepository.Object);
    }
    [Fact]
    public async Task GetsAllTasksNoError()
    {
      //Arrange
      IList<TodoTask> givenTaskEntities = [new TodoTask
      { Id=1, Description="A", Duedate=DateTime.Now.ToDateTimeOffset(), Status=TodoTaskStatus.ToDo },
        new TodoTask
      { Id=2, Description="B", Duedate=DateTime.Now.ToDateTimeOffset(), Status=TodoTaskStatus.InProgress }, ];
      mockTodoTaskRepository.Setup(x => x.GetAll()).ReturnsAsync(givenTaskEntities);
      var expectedResult = new QueryGetAllTasksResult
      {
        Count = 2,
        Items = givenTaskEntities.Select(e => new ItemResultModel
        {
          Description = e.Description,
          DueDate = $"{e.Duedate:D}",
          Id = e.Id,
          Status = Enum.GetName<TodoTaskStatus>(e.Status) ?? ""
        }),
        Message = "2 tasks retrieved"
      };
      //Act
      var actualResult = await testQueryGetAllTasks.Execute();
      //Assert
      mockTodoTaskRepository.VerifyAll();
      actualResult.Should().BeEquivalentTo(expectedResult);
    }
  }
}