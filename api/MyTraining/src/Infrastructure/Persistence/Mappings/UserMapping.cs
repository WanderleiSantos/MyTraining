using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

namespace Infrastructure.Persistence.Mappings;

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
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(x => x.LastName)
            .HasColumnName("lastname")
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(x => x.Password)
            .HasColumnName("password")
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
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}