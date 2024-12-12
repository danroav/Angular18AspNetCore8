using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Angular18AspNetCore8.Server.Tests
{
  public class TodoListTests
  {
    readonly TodoListController todoListController;
    readonly Mock<IQueryGetAllTasks> mockGetAllTasks;
    public TodoListTests()
    {
      mockGetAllTasks = new Mock<IQueryGetAllTasks>();
      todoListController = new TodoListController(mockGetAllTasks.Object);
    }

    [Fact]
    public async Task GetAllTasks()
    {
      //Arrange
      var expectedResult = new QueryGetAllTasksResult
      {
        Count = 2,
        Message = "",
        Items = [new ItemResultModel(), new ItemResultModel()]
      };

      mockGetAllTasks.Setup(x => x.Execute()).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoListController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<QueryGetAllTasksResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
  }
}