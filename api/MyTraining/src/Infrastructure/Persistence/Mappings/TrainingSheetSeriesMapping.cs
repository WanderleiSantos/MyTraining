using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class TrainingSheetSeriesMapping : IEntityTypeConfiguration<TrainingSheetSeries>
{
    public void Configure(EntityTypeBuilder<TrainingSheetSeries> builder)
    {
        builder
            .ToTable("training_sheet_series")
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasColumnName("id");

        builder
            .Property(x => x.TrainingSheetId)
            .HasColumnName("training_sheet_id")
            .IsRequired();
        
        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();
        
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

        builder
            .HasOne(x => x.TrainingSheet)
            .WithMany(x => x.TrainingSheetSeries)
            .HasForeignKey(x => x.TrainingSheetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}