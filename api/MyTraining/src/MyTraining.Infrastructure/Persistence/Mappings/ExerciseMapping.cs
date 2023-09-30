using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTraining.Core.Entities;

namespace MyTraining.Infrastructure.Persistence.Mappings;

public class ExerciseMapping : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder
            .HasKey(exercise => exercise.Id);

        builder
            .HasOne(exercise => exercise.User)
            .WithMany(user => user.Exercises)
            .HasForeignKey(exercise => exercise.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}