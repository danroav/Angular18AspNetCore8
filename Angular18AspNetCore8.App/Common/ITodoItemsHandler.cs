namespace Angular18AspNetCore8.App.Common
{
  public interface ITodoItemsHandlerInput { }
  public interface ITodoItemsHandlerOutput { }
  public interface ITodoItemsHandler<TInput, TOutput> where TInput : ITodoItemsHandlerInput where TOutput : ITodoItemsHandlerOutput
  {
    Task<TOutput> Execute(TInput command);
  }
}
