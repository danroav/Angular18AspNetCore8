using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

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
    public async Task GetAllTasksSuccess()
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
    [Fact]
    public async Task GetAllTasksUnexpectedError()
    {
      //Arrange
      var expectedExceptionMessage = "Some error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      mockGetAllTasks.Setup(x => x.Execute()).ThrowsAsync(new Exception(expectedExceptionMessage));
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedExceptionMessage,
        Status = expectedStatus
      };

      //Act
      var actualResult = await todoListController.GetAllTasks();

      //Assert
      actualResult.Should().BeOfType<ActionResult<QueryGetAllTasksResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
  }
}