using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebAPI.Database.Models;

namespace WebAPI.Database.Configurations
{
    public class ValueConfiguration : IEntityTypeConfiguration<ValueEntity>
    {

        public void Configure(EntityTypeBuilder<ValueEntity> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .HasOne(v => v.Result)
                .WithMany(r => r.Values)
                .HasForeignKey(v => v.ResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
