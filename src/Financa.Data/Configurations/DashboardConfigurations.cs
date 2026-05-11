using Financa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Financa.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories", schema: "finance");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(c => c.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Icon).HasColumnName("icon").HasMaxLength(10).IsRequired();
        builder.Property(c => c.Color).HasColumnName("color").HasMaxLength(7).IsRequired();
        builder.Property(c => c.Type).HasColumnName("type").HasMaxLength(10).IsRequired();
        builder.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at")
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        builder.HasIndex(c => c.UserId).HasDatabaseName("ix_categories_user_id");
    }
}

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions", schema: "finance");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(t => t.Amount).HasColumnName("amount").HasPrecision(12, 2).IsRequired();
        builder.Property(t => t.Type).HasColumnName("type").HasMaxLength(10).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description").HasMaxLength(255);
        builder.Property(t => t.Date).HasColumnName("date").IsRequired();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at")
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at")
            .HasConversion(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .HasConstraintName("fk_transactions_category")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.UserId).HasDatabaseName("ix_transactions_user_id");
        builder.HasIndex(t => new { t.UserId, t.Date }).HasDatabaseName("ix_transactions_user_date");
    }
}

public class MonthlyGoalConfiguration : IEntityTypeConfiguration<MonthlyGoal>
{
    public void Configure(EntityTypeBuilder<MonthlyGoal> builder)
    {
        builder.ToTable("monthly_goals", schema: "finance");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(g => g.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(g => g.TargetAmount).HasColumnName("target_amount").HasPrecision(12, 2).IsRequired();
        builder.Property(g => g.Month).HasColumnName("month").IsRequired();
        builder.Property(g => g.Year).HasColumnName("year").IsRequired();
        builder.Property(g => g.CreatedAt).HasColumnName("created_at")
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        builder.Property(g => g.UpdatedAt).HasColumnName("updated_at")
            .HasConversion(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

        builder.HasIndex(g => new { g.UserId, g.Month, g.Year })
            .IsUnique()
            .HasDatabaseName("ix_monthly_goals_user_month_year");
    }
}
