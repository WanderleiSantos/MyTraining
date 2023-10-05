using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class TrainingSheetMapping : IEntityTypeConfiguration<TrainingSheet>
{
    public void Configure(EntityTypeBuilder<TrainingSheet> builder)
    {
        builder
            .ToTable("training_sheet")
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id");

        builder
            .Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(x => x.TimeExchange)
            .HasColumnName("time_exchange");
        
        builder
            .Property(x => x.Active)
            .HasColumnName("active")
            .HasDefaultValue(true)
            .IsRequired();
        
        builder
            .Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder
            .Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();        

        builder.HasOne(x => x.User)
            .WithMany(x => x.TrainingSheets)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}