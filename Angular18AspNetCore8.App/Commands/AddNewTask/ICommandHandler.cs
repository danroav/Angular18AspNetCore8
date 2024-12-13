namespace Angular18AspNetCore8.App.Commands.AddNewTask
{
  public interface ICommandHandler<in TCommand, TResult>
  {
    Task<TResult> Execute(TCommand command);
  }
}
