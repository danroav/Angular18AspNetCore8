using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Angular18AspNetCore8.Infra.Persistence.Configurations;

internal class TodoItemsConfiguration : IEntityTypeConfiguration<TodoItem>
{
  public void Configure(EntityTypeBuilder<TodoItem> builder)
  {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Description).HasMaxLength(255).IsRequired();
    builder.Property(x => x.DueDate);
    builder.Property(x => x.LastUserStatus).HasConversion(c => TodoItemStatusNames.Format[c], f => TodoItemStatusNames.Parse[f]).IsRequired();
  }
}
