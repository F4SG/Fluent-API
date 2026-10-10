using HeroesWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesWeb.Data.Configurations;

public class HeroesConfiguration : IEntityTypeConfiguration<Heroes>
{
    public void Configure(EntityTypeBuilder<Heroes> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__Heroes__3214EC076F156510");
    }
}
