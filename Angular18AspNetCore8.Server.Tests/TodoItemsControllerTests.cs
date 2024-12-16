using Angular18AspNetCore8.App.Commands.AddNewTodoItem;
using Angular18AspNetCore8.App.Commands.DeleteTodoItem;
using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
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
        readonly Mock<ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult>> mockGetAllTodoITems;
        readonly Mock<ITodoItemsHandler<CreateTodoItem, CreateTodoItemResult>> mockAddNewTodoItemHandler;
        readonly Mock<ITodoItemsHandler<UpdateTodoItem, UpdateTodoItemResult>> mockUpdateTodoItemHandler;
        readonly Mock<ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>> mockDeleteTodoItemHandler;
        public TodoItemsControllerTests()
        {
            mockGetAllTodoITems = new Mock<ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult>>();
            mockAddNewTodoItemHandler = new Mock<ITodoItemsHandler<CreateTodoItem, CreateTodoItemResult>>();
            mockUpdateTodoItemHandler = new Mock<ITodoItemsHandler<UpdateTodoItem, UpdateTodoItemResult>>();
            mockDeleteTodoItemHandler = new Mock<ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>>();
            todoItemsController = new TodoItemsController(mockGetAllTodoITems.Object, mockAddNewTodoItemHandler.Object, mockUpdateTodoItemHandler.Object, mockDeleteTodoItemHandler.Object);
        }

        [Fact]
        public async Task GetAllTodoItemsSuccess()
        {
            //Arrange
            var expectedResult = new GetAllTodoItemsResult
            {
                Message = "",
                Items = [new TodoItemModel(), new TodoItemModel()]
            };

            mockGetAllTodoITems.Setup(x => x.Execute(It.IsAny<GetAllTodoITems>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.GetAllTodoItems();

            //Assert
            actualResult.Should().BeOfType<ActionResult<GetAllTodoItemsResult>>();
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
            mockGetAllTodoITems.Setup(x => x.Execute(It.IsAny<GetAllTodoITems>())).ThrowsAsync(new Exception(expectedExceptionMessage));
            var expectedProblem = new ProblemDetails
            {
                Detail = expectedExceptionMessage,
                Status = expectedStatus
            };

            //Act
            var actualResult = await todoItemsController.GetAllTodoItems();

            //Assert
            actualResult.Should().BeOfType<ActionResult<GetAllTodoItemsResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<ObjectResult>();
            var problem = ((ObjectResult)actualResult.Result!);
            problem.StatusCode.Should().Be(expectedStatus);
            problem.Value.Should().BeEquivalentTo(expectedProblem);
        }
        [Fact]
        public async Task CreateTodoItemSuccess()
        {
            //Arrange
            var givenCommand = new CreateTodoItem
            {
                Description = "some description",
                DueDate = null,
                Status = "",
            };
            var expectedResult = new CreateTodoItemResult
            {
                Item = new TodoItemModel
                {
                    Id = 0,
                    Description = givenCommand.Description,
                    DueDate = givenCommand.DueDate,
                    Status = givenCommand.Status,
                }
            };


            mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<CreateTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.CreateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<CreateTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task CreateTodoItemUnexpectedError()
        {
            //Arrange
            var givenCommand = new CreateTodoItem
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

            mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<CreateTodoItem>())).ThrowsAsync(new Exception(expectedErrorMessage));

            //Act
            var actualResult = await todoItemsController.CreateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<CreateTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<ObjectResult>();
            var problem = ((ObjectResult)actualResult.Result!);
            problem.StatusCode.Should().Be(expectedStatus);
            problem.Value.Should().BeEquivalentTo(expectedProblem);
        }
        [Fact]
        public async Task CreateTodoItemRequestError()
        {
            //Arrange
            var givenCommand = new CreateTodoItem
            {
                Description = "some description",
                DueDate = null,
                Status = "",
            };
            var expectedResult = new CreateTodoItemResult
            {
                Item = new TodoItemModel
                {
                    Description = givenCommand.Description,
                    DueDate = givenCommand.DueDate,
                    Id = 0,
                    Status = givenCommand.Status,
                },
                Message = "Todo item should be valid",
                ValidationErrors = new Dictionary<string, string[]>() { { "someProperty", ["someValidationError"] } }
            };


            mockAddNewTodoItemHandler.Setup(x => x.Execute(It.IsAny<CreateTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.CreateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<CreateTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task UpdateTodoItemSuccess()
        {
            //Arrange
            var expectedResult = new UpdateTodoItemResult
            {
                Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] },
                Message = "Update successful",
                ValidationErrors = new Dictionary<string, string[]>()
            };
            var givenCommand = new UpdateTodoItem
            {
                Item = new TodoItemModel { Description = "Some Description", DueDate = null, Id = 123, Status = TodoItemStatusNames.Format[TodoItemStatus.ToDo] }
            };

            mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<UpdateTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<UpdateTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task UpdateTodoItemUnexpectedError()
        {
            //Arrange
            var givenCommand = new UpdateTodoItem
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

            mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<UpdateTodoItem>())).ThrowsAsync(new Exception(expectedErrorMessage));

            //Act
            var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<UpdateTodoItemResult>>();
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
            var expectedResult = new UpdateTodoItemResult
            {
                Item = givenItem,
                Message = "Validation failed",
                ValidationErrors = new Dictionary<string, string[]>{
          { "someProperty",["Some validation Error"] }
        }
            };
            var givenCommand = new UpdateTodoItem
            {
                Item = givenItem
            };

            mockUpdateTodoItemHandler.Setup(x => x.Execute(It.IsAny<UpdateTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.UpdateTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<UpdateTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task DeleteTodoItemSuccess()
        {
            //Arrange
            var expectedResult = new DeleteTodoItemResult
            {
                Message = "Delete successful",
                ValidationErrors = new Dictionary<string, string[]>()
            };
            var givenCommand = new DeleteTodoItem
            {
                TodoItemId = 123
            };

            mockDeleteTodoItemHandler.Setup(x => x.Execute(It.IsAny<DeleteTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.DeleteTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<DeleteTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
        [Fact]
        public async Task DeleteTodoItemUnexpectedError()
        {
            //Arrange
            var givenCommand = new DeleteTodoItem
            {
                TodoItemId = 123
            };
            var expectedErrorMessage = "Unexpected error";
            var expectedStatus = (int)HttpStatusCode.InternalServerError;
            var expectedProblem = new ProblemDetails
            {
                Detail = expectedErrorMessage,
                Status = expectedStatus
            };

            mockDeleteTodoItemHandler.Setup(x => x.Execute(It.IsAny<DeleteTodoItem>())).ThrowsAsync(new Exception(expectedErrorMessage));

            //Act
            var actualResult = await todoItemsController.DeleteTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<DeleteTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<ObjectResult>();
            var problem = ((ObjectResult)actualResult.Result!);
            problem.StatusCode.Should().Be(expectedStatus);
            problem.Value.Should().BeEquivalentTo(expectedProblem);
        }
        [Fact]
        public async Task DeleteTodoItemRequestError()
        {
            //Arrange
            var expectedResult = new DeleteTodoItemResult
            {
                Message = "Validation failed",
                ValidationErrors = new Dictionary<string, string[]>{
                    { "id",["Some validation Error"] }
                }
            };
            var givenCommand = new DeleteTodoItem
            {
                TodoItemId = 234
            };

            mockDeleteTodoItemHandler.Setup(x => x.Execute(It.IsAny<DeleteTodoItem>())).ReturnsAsync(expectedResult);

            //Act
            var actualResult = await todoItemsController.DeleteTodoItem(givenCommand);

            //Assert
            actualResult.Should().BeOfType<ActionResult<DeleteTodoItemResult>>();
            actualResult.Result.Should().NotBeNull();
            actualResult.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)actualResult.Result!).Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}