using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;

namespace Angular18AspNetCore8.App.Tests
{
  public class QueryGetAllTasksTests
  {
    readonly Mock<ITodoTasksRepository> mockTodoTaskRepository;
    readonly QueryGetAllTasksHandler testQueryGetAllTasks;
    public QueryGetAllTasksTests()
    {
      mockTodoTaskRepository = new Mock<ITodoTasksRepository>();
      testQueryGetAllTasks = new QueryGetAllTasksHandler(mockTodoTaskRepository.Object);
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
      var expectedResult = new QueryGetAllTasksResult
      {
        Count = 2,
        Items = givenTaskEntities.Select(e => new ItemResultModel
        {
          Description = e.Description,
          DueDate = $"{e.Duedate:O}",
          Id = e.Id,
          Status = Enum.GetName<TodoTaskStatus>(e.Status) ?? ""
        }),
        Message = "2 tasks retrieved"
      };
      //Act
      var actualResult = await testQueryGetAllTasks.Execute(new QueryGetAllTasks());
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
      var actualResult = () => testQueryGetAllTasks.Execute(new QueryGetAllTasks());

      //Assert
      await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
    }
  }
}