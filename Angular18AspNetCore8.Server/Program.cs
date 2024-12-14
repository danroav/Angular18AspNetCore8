using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Infra.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("TodoItemsDb"));

builder.Services.AddScoped<ITodoItemsRepository, TodoItemsRepository>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Queries.GetAllTodoItems.Query, Angular18AspNetCore8.App.Queries.GetAllTodoItems.Response>, Angular18AspNetCore8.App.Queries.GetAllTodoItems.Handler>();

builder.Services.AddTransient<IValidator<Angular18AspNetCore8.App.Commands.AddNewTodoItem.Command>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.Validator>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Commands.AddNewTodoItem.Command, Angular18AspNetCore8.App.Commands.AddNewTodoItem.Response>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.Handler>();

builder.Services.AddTransient<IValidator<Angular18AspNetCore8.App.Commands.UpdateTodoItem.Command>, Angular18AspNetCore8.App.Commands.UpdateTodoItem.Validator>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Commands.UpdateTodoItem.Command, Angular18AspNetCore8.App.Commands.UpdateTodoItem.Response>, Angular18AspNetCore8.App.Commands.UpdateTodoItem.Handler>();

builder.Services.AddTransient<TodoItemMapper>();

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
