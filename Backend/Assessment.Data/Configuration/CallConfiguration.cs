using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Assessment.Data.Entities;

namespace Assessment.Data.Configuration;

public class CallConfiguration : BaseConfiguration<Call>
{
    public override void Configure(EntityTypeBuilder<Call> builder)
    {
        builder
            .Property(x => x.DateCallStarted)
            .IsRequired();

        builder
            .HasOne<User>(x => x.CallingUser)
            .WithMany(x => x.Calls)
            .HasForeignKey(x => x.CallingUserId)
            .IsRequired();
        
        base.Configure(builder);
    }
}