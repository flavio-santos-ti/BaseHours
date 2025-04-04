using BaseHours.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseHours.Infrastructure.Persistence.Mappings;

public class ClientMap : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(c => c.NormalizedName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("normalized_name");

        builder.HasIndex(c => c.NormalizedName)
            .IsUnique()
            .HasDatabaseName("ix_clients_normalized_name");

        builder.HasIndex(c => c.Name) 
            .IsUnique();

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");
    }
}
