using Assessment.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Assessment.Data.Configuration;

public class UserConfiguration : BaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(x => x.Username)
            .HasMaxLength(255)
            .IsRequired();

        builder
            .HasMany<Call>(x => x.Calls)
            .WithOne(x => x.CallingUser);
        
        base.Configure(builder);
    }
}