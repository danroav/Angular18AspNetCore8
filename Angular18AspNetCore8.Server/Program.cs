using Angular18AspNetCore8.App.Commands.CreateTodoItem;
using Angular18AspNetCore8.App.Commands.DeleteTodoItem;
using Angular18AspNetCore8.App.Commands.UpdateTodoItem;
using Angular18AspNetCore8.App.Common;
using Angular18AspNetCore8.App.Queries.GetAllTodoItems;
using Angular18AspNetCore8.Infra.Persistence;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("TodoItemsDb"));

builder.Services.AddScoped<ITodoItemsRepository, TodoItemsRepository>();
builder.Services.AddScoped<ITodoItemsHandler<GetAllTodoITems, GetAllTodoItemsResult>, GetAllTodoITemsHandler>();

builder.Services.AddTransient<IValidator<CreateTodoItem>, CreateTodoItemValidator>();
builder.Services.AddScoped<ITodoItemsHandler<CreateTodoItem, CreateTodoItemResult>, CreateTodoItemHandler>();

builder.Services.AddTransient<IValidator<UpdateTodoItem>, UpdateTodoItemValidator>();
builder.Services.AddScoped<ITodoItemsHandler<UpdateTodoItem, UpdateTodoItemResult>, UpdateTodoItemHandler>();

builder.Services.AddScoped<ITodoItemsHandler<DeleteTodoItem, DeleteTodoItemResult>, DeleteTodoItemHandler>();

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
