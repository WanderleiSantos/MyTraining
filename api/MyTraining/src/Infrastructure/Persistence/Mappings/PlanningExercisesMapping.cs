using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Mappings;

public class PlanningExercisesMapping : IEntityTypeConfiguration<PlanningExercises>
{
    public void Configure(EntityTypeBuilder<PlanningExercises> builder)
    {
        builder
            .ToTable("planning_exercises")
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id");
    }
}