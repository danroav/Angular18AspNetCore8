namespace Angular18AspNetCore8.App.Common
{
  public interface ITodoTasksHandlerInput { }
  public interface ITodoTasksHandlerOutput { }
  public interface ITodoTasksHandler<TInput, TOutput> where TInput : ITodoTasksHandlerInput where TOutput : ITodoTasksHandlerOutput
  {
    Task<TOutput> Execute(TInput command);
  }
}
