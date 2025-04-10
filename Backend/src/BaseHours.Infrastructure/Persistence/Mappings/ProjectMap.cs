using BaseHours.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseHours.Infrastructure.Persistence.Mappings;

public class ProjectMap : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Define the table name
        // Define o nome da tabela
        builder.ToTable("projects");

        // Primary key
        // Chave primária
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(p => p.ClientId)
            .IsRequired()
            .HasColumnName("client_id");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(p => p.NormalizedName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("normalized_name");

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        // Unique index on normalized_name
        // Índice único sobre normalized_name
        builder.HasIndex(p => p.NormalizedName)
            .IsUnique()
            .HasDatabaseName("ix_projects_normalized_name");

        // Relationship: Project → Client (many-to-one)
        // Relacionamento: Project → Client (muitos para um)
        builder.HasOne(p => p.Client)
            .WithMany()
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}