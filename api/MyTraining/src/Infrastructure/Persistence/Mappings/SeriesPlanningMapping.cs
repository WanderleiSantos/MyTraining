using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class SeriesPlanningMapping : IEntityTypeConfiguration<SeriesPlanning>
{
    public void Configure(EntityTypeBuilder<SeriesPlanning> builder)
    {
        builder
            .ToTable("series_planning")
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id");
        
        builder
            .Property(x => x.TrainingSheetSeriesId)
            .HasColumnName("training_sheet_series_id")
            .IsRequired();
        
        builder
            .Property(x => x.Machine)
            .HasColumnName("machine")
            .HasMaxLength(150);
        
        builder
            .Property(x => x.SeriesNumber)
            .HasColumnName("series_number");
        
        builder
            .Property(x => x.Repetitions)
            .HasColumnName("repetitions")
            .HasMaxLength(150);
        
        builder
            .Property(x => x.Charge)
            .HasColumnName("charge")
            .HasMaxLength(150);
        
        builder
            .Property(x => x.Interval)
            .HasColumnName("interval")
            .HasMaxLength(150);
        
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
            .HasOne(x => x.TrainingSheetSeries)
            .WithMany(x => x.SeriesPlannings)
            .HasForeignKey(x => x.TrainingSheetSeriesId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasMany(e => e.Exercises)
            .WithMany(e => e.SeriesPlannings)
            .UsingEntity("planning_exercises");
        
    }
}