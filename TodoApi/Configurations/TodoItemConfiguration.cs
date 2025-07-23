using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Models;

namespace TodoApi.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasMaxLength(100);
        builder.Property(t => t.IsComplete)
            .IsRequired()
            .HasColumnName("IsComplete");
        builder.Property(t => t.Secret)
            .HasMaxLength(200)
            .HasColumnName("Secret");
    }
}