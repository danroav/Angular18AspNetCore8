using Angular18AspNetCore8.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Angular18AspNetCore8.Infra.Persistence.Configurations;

internal class TodoTasksConfiguration : IEntityTypeConfiguration<TodoTask>
{
  public void Configure(EntityTypeBuilder<TodoTask> builder)
  {
    builder.HasKey(x => x.Id);

    builder.Property(x => x.Description).HasMaxLength(255).IsRequired();
    builder.Property(x => x.Duedate);
    builder.Property(x => x.Status).HasConversion(c => TodoTaskStatusNames.Format[c], f => TodoTaskStatusNames.Parse[f]).IsRequired();
  }
}
