using Angular18AspNetCore8.App.Commands.UpdateTask;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTasks;
using Angular18AspNetCore8.Infra.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("TodoListDb"));

builder.Services.AddScoped<ITodoTasksRepository, TodoTasksRepository>();
builder.Services.AddScoped<ITodoTasksHandler<QueryGetAllTasks, QueryGetAllTasksResult>, QueryGetAllTasksHandler>();

builder.Services.AddTransient<IValidator<Angular18AspNetCore8.App.Commands.AddNewTodoItem.Command>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.CommandValidator>();
builder.Services.AddScoped<ITodoTasksHandler<Angular18AspNetCore8.App.Commands.AddNewTodoItem.Command, Angular18AspNetCore8.App.Commands.AddNewTodoItem.Response>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.CommandHandler>();

builder.Services.AddTransient<IValidator<CommandUpdateTask>, CommandUpdateTaskValidator>();
builder.Services.AddScoped<ITodoTasksHandler<CommandUpdateTask, CommandUpdateTaskResult>, CommandUpdateTaskHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
