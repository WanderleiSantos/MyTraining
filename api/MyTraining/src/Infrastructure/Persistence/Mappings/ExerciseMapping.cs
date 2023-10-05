using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

namespace Infrastructure.Persistence.Mappings;

public class ExerciseMapping : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder
            .ToTable("exercise")
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id");

        builder
            .Property(x => x.IdUser)
            .HasColumnName("user_id")
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired();

        builder
            .Property(x => x.Link)
            .HasColumnName("link");
        
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
            .HasOne(exercise => exercise.User)
            .WithMany(user => user.Exercises)
            .HasForeignKey(exercise => exercise.IdUser)
            .OnDelete(DeleteBehavior.Restrict);
    }
}