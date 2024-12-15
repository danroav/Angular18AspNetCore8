using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Core.Entities;
using Angular18AspNetCore8.Server.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Angular18AspNetCore8.Server.Tests
{
  public class TodoItemsControllerTests
  {
    readonly TodoItemsController todoItemsController;
    readonly Mock<ITodoItemsHandler<App.Queries.GetAllTodoItems.GetAllTodoITems, App.Queries.GetAllTodoItems.GetAllTodoItemsResult>> mockGetAllTodoITems;
    readonly Mock<ITodoItemsHandler<App.Commands.AddNewTodoItem.AddNewTodoItem, App.Commands.AddNewTodoItem.AddNewTodoItemResult>> mockAddNewTodoItemHandler;
    readonly Mock<ITodoItemsHandler<App.Commands.UpdateTodoItem.UpdateTodoItem, App.Commands.UpdateTodoItem.UpdateTodoItemResult>> mockUpdateTodoItemHandler;
    public TodoItemsControllerTests()
    {
      mockGetAllTodoITems = new Mock<ITodoItemsHandler<App.Queries.GetAllTodoItems.GetAllTodoITems, App.Queries.GetAllTodoItems.GetAllTodoItemsResult>>();
      mockAddNewTodoItemHandler = new Mock<ITodoItemsHandler<App.Commands.AddNewTodoItem.AddNewTodoItem, App.Commands.AddNewTodoItem.AddNewTodoItemResult>>();
      mockUpdateTodoItemHandler = new Mock<ITodoItemsHandler<App.Commands.UpdateTodoItem.UpdateTodoItem, App.Commands.UpdateTodoItem.UpdateTodoItemResult>>();
      todoItemsController = new TodoItemsController(mockGetAllTodoITems.Object, mockAddNewTodoItemHandler.Object, mockUpdateTodoItemHandler.Object);
    }

    [Fact]
    public async Task GetAllTodoItemsSuccess()
    {
      //Arrange
      var expectedResult = new App.Queries.GetAllTodoItems.GetAllTodoItemsResult
      {
        Count = 2,
        Message = "",
        Items = [new TodoItemModel(), new TodoItemModel()]
      };

      mockGetAllTodoITems.Setup(x => x.Execute(It.IsAny<App.Queries.GetAllTodoItems.GetAllTodoITems>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.GetAllTodoItems();

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Queries.GetAllTodoItems.GetAllTodoItemsResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task GetAllTodoItemsUnexpectedError()
    {
      //Arrange
      var expectedExceptionMessage = "Some error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      mockGetAllTodoITems.Setup(x => x.Execute(It.IsAny<App.Queries.GetAllTodoItems.GetAllTodoITems>())).ThrowsAsync(new Exception(expectedExceptionMessage));
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedExceptionMessage,
        Status = expectedStatus
      };

      //Act
      var actualResult = await todoItemsController.GetAllTodoItems();

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Queries.GetAllTodoItems.GetAllTodoItemsResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task AddNewTodoItemSuccess()
    {
      //Arrange
      var givenCommand = new App.Commands.AddNewTodoItem.AddNewTodoItem
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedResult = new App.Commands.AddNewTodoItem.AddNewTodoItemResult
      {
        Item = new TodoItemModel
        {
          Id = 0,
          Description = givenCommand.Description,
          DueDate = givenCommand.DueDate,
          Status = givenCommand.Status,
        }
      };


      mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.AddNewTodoItem>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.AddNewTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.AddNewTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task AddNewTodoItemUnexpectedError()
    {
      //Arrange
      var givenCommand = new App.Commands.AddNewTodoItem.AddNewTodoItem
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedErrorMessage = "Unexpected error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedErrorMessage,
        Status = expectedStatus
      };

      mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.AddNewTodoItem>())).ThrowsAsync(new Exception(expectedErrorMessage));

      //Act
      var actualResult = await todoItemsController.AddNewTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.AddNewTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task AddNewTodoItemRequestError()
    {
      //Arrange
      var givenCommand = new App.Commands.AddNewTodoItem.AddNewTodoItem
      {
        Description = "some description",
        DueDate = null,
        Status = "",
      };
      var expectedResult = new App.Commands.AddNewTodoItem.AddNewTodoItemResult
      {
        Item = new TodoItemModel
        {
          Description = givenCommand.Description,
          DueDate = givenCommand.DueDate,
          Id = 0,
          Status = givenCommand.Status,
        },
        HasValidationErrors = true,
        ValidationErrors = new Dictionary<string, string[]>() { { "someProperty", ["someValidationError"] } }
      };


      mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.AddNewTodoItem.AddNewTodoItem>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.AddNewTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.AddNewTodoItem.AddNewTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
      ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task UpdateTodoItemSuccess()
    {
      //Arrange
      var expectedResult = new App.Commands.UpdateTodoItem.UpdateTodoItemResult
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] },
        HasValidationErrors = false,
        ValidationErrors = new Dictionary<string, string[]>()
      };
      var givenCommand = new App.Commands.UpdateTodoItem.UpdateTodoItem
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] }
      };

      mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.UpdateTodoItem>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.UpdateTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<OkObjectResult>();
      ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
    [Fact]
    public async Task UpdateTodoItemUnexpectedError()
    {
      //Arrange
      var givenCommand = new App.Commands.UpdateTodoItem.UpdateTodoItem
      {
        Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] }
      };
      var expectedErrorMessage = "Unexpected error";
      var expectedStatus = (int)HttpStatusCode.InternalServerError;
      var expectedProblem = new ProblemDetails
      {
        Detail = expectedErrorMessage,
        Status = expectedStatus
      };

      mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.UpdateTodoItem>())).ThrowsAsync(new Exception(expectedErrorMessage));

      //Act
      var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.UpdateTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<ObjectResult>();
      var problem = ((ObjectResult)actualResult.Result!);
      problem.StatusCode.Should().Be(expectedStatus);
      problem.Value.Should().BeEquivalentTo(expectedProblem);
    }
    [Fact]
    public async Task UpdateTodoItemRequestError()
    {
      //Arrange
      var givenItem = new TodoItemModel
      {
        Description = "Some Description",
        DueDate = null,
        Id = 123,
        Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo]
      };
      var expectedResult = new App.Commands.UpdateTodoItem.UpdateTodoItemResult
      {
        Item = givenItem,
        HasValidationErrors = true,
        ValidationErrors = new Dictionary<string, string[]>{
          { "someProperty",["Some validation Error"] }
        }
      };
      var givenCommand = new App.Commands.UpdateTodoItem.UpdateTodoItem
      {
        Item = givenItem
      };

      mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<App.Commands.UpdateTodoItem.UpdateTodoItem>())).ReturnsAsync(expectedResult);

      //Act
      var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

      //Assert
      actualResult.Should().BeOfType<ActionResult<App.Commands.UpdateTodoItem.UpdateTodoItemResult>>();
      actualResult.Result.Should().NotBeNull();
      actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
      ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
    }
  }
}