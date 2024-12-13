using Angular18AspNetCore8.App.Commands.UpdateTask;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Core.Entities;
using FluentAssertions;
using Moq;

namespace Angular18AspNetCore8.App.Tests;

public class CommandUpdateTasksTests
{
  readonly Mock<ITodoTasksRepository> mockTodoTaskRepository;
  readonly CommandUpdateTaskHandler testUpdateTaskHandler;
  readonly CommandUpdateTaskValidator validator = new();
  public CommandUpdateTasksTests()
  {
    mockTodoTaskRepository = new Mock<ITodoTasksRepository>();
    testUpdateTaskHandler = new CommandUpdateTaskHandler(mockTodoTaskRepository.Object, validator);
  }
  [Theory]
  [InlineData(true)]
  public async Task UpdateTaskSuccess(bool givenUpdateDatetimeNull)
  {
    //Arrange
    var givenTodoTaskStatus = TodoTaskStatus.ToDo;
    var givenUpdateDatetime = DateTimeOffset.Now;
    var givenExistingDatetime = DateTimeOffset.Now.AddDays(-1);
    var givenItemToUpdate = new ItemResultModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = givenUpdateDatetimeNull ? "" : $"{givenUpdateDatetime:O}",
      Status = TodoTaskStatusNames.Format[givenTodoTaskStatus],
    };
    var givenCommandUpdateTask = new CommandUpdateTask
    {
      Item = givenItemToUpdate
    };
    var existingTodoTask = new TodoTask
    {
      Id = givenCommandUpdateTask.Item.Id,
      Description = "Existing description",
      Duedate = givenExistingDatetime,
      Status = TodoTaskStatus.InProgress
    };

    mockTodoTaskRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ReturnsAsync([existingTodoTask]);
    mockTodoTaskRepository.Setup(x => x.SaveChanges());

    var expectedResult = new CommandUpdateTaskResult
    {
      HasValidationErrors = false,
      Item = new ItemResultModel
      {
        Id = givenItemToUpdate.Id,
        Description = givenItemToUpdate.Description,
        DueDate = string.IsNullOrEmpty(givenItemToUpdate.DueDate) ? "" : givenItemToUpdate.DueDate,
        Status = givenItemToUpdate.Status
      }
    };
    //Act
    var actualResult = await testUpdateTaskHandler.Execute(givenCommandUpdateTask);
    //Assert
    mockTodoTaskRepository.VerifyAll();
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
  [Fact]
  public async Task UpdateTaskWithError()
  {
    //Arrange
    var givenTodoTaskStatus = TodoTaskStatus.ToDo;
    var givenItemToUpdate = new ItemResultModel
    {
      Id = 1,
      Description = "Some description",
      DueDate = "",
      Status = TodoTaskStatusNames.Format[givenTodoTaskStatus],
    };
    var givenCommand = new CommandUpdateTask
    {
      Item = givenItemToUpdate
    };
    var expectedError = "Some error description";
    mockTodoTaskRepository.Setup(x => x.GetByIds(It.IsAny<IList<int>>())).ThrowsAsync(new Exception(expectedError));

    //Act
    var actualResult = () => testUpdateTaskHandler.Execute(givenCommand);

    //Assert
    await actualResult.Should().ThrowAsync<Exception>().WithMessage(expectedError);
  }
  [Theory]
  [InlineData("", "Description is required", -1, "Due Date should be in the future", "", "Status should be valid")]
  [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa123456", "Description should not exceed 255 chars", -1, "Due Date should be in the future", "", "Status should be valid")]
  public async Task UpdateTaskWithValidationErrors(string givenDescription, string givenDescriptionError, int givenDueDateDaysAdd, string givenDueDateError, string givenStatus, string givenStatusError)
  {
    //Arrange
    var givenItemToUpdate = new ItemResultModel
    {
      Id = 1,
      Description = givenDescription,
      DueDate = $"{DateTimeOffset.Now.AddDays(givenDueDateDaysAdd):O}",
      Status = givenStatus,
    };
    var givenCommand = new CommandUpdateTask
    {
      Item = givenItemToUpdate
    };
    var expectedResult = new CommandUpdateTaskResult
    {
      HasValidationErrors = true,
      Item = givenItemToUpdate,
      ValidationErrors = new Dictionary<string, string[]>
        {
          {"Item.Description",[givenDescriptionError] },
          {"Item.DueDate",[givenDueDateError] },
          {"Item.Status",[givenStatusError] }
        }
    };

    //Act
    var actualResult = await testUpdateTaskHandler.Execute(givenCommand);

    //Assert
    actualResult.Should().BeEquivalentTo(expectedResult);
  }
}
