using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.Infra.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("TodoItemsDb"));

builder.Services.AddScoped<ITodoItemsRepository, TodoItemsRepository>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Queries.GetAllTodoItems.GetAllTodoITems, Angular18AspNetCore8.App.Queries.GetAllTodoItems.GetAllTodoItemsResult>, Angular18AspNetCore8.App.Queries.GetAllTodoItems.GetAllTodoITemsHandler>();

builder.Services.AddTransient<IValidator<Angular18AspNetCore8.App.Commands.AddNewTodoItem.AddNewTodoItem>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.AddNewTodoItemValidator>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Commands.AddNewTodoItem.AddNewTodoItem, Angular18AspNetCore8.App.Commands.AddNewTodoItem.AddNewTodoItemResult>, Angular18AspNetCore8.App.Commands.AddNewTodoItem.AddNewTodoItemHandler>();

builder.Services.AddTransient<IValidator<Angular18AspNetCore8.App.Commands.UpdateTodoItem.UpdateTodoItem>, Angular18AspNetCore8.App.Commands.UpdateTodoItem.UpdateTodoItemValidator>();
builder.Services.AddScoped<ITodoItemsHandler<Angular18AspNetCore8.App.Commands.UpdateTodoItem.UpdateTodoItem, Angular18AspNetCore8.App.Commands.UpdateTodoItem.UpdateTodoItemResult>, Angular18AspNetCore8.App.Commands.UpdateTodoItem.UpdateTodoItemHandler>();

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
