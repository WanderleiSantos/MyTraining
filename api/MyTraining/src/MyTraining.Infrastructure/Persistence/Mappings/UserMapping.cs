using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTraining.Core.Entities;

namespace MyTraining.Infrastructure.Persistence.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("user")
            .HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id");

        builder
            .Property(x => x.FirstName)
            .HasColumnName("firstname")
            .IsRequired();
        
        builder
            .Property(x => x.LastName)
            .HasColumnName("lastname")
            .IsRequired();
        
        builder
            .Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired();
        
        builder
            .Property(x => x.Password)
            .HasColumnName("password")
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
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}