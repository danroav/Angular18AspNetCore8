namespace Angular18AspNetCore8.App.Common
{
  public interface IHandler<in TCommand, TResult>
  {
    Task<TResult> Execute(TCommand command);
  }
}
