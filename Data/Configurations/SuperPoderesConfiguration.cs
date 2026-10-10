using HeroesWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesWeb.Data.Configurations;

public class SuperPoderesConfiguration : IEntityTypeConfiguration<SuperPoderes>
{
    public void Configure(EntityTypeBuilder<SuperPoderes> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__SuperPod__3214EC07AB65ED12");
        builder.HasOne(d => d.Heroe).WithMany(p => p.SuperPoderes).HasConstraintName("FK_SuperPoderes_Heroes");
    }
}
