using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebAPI.Database.Models;

namespace WebAPI.Database.Configurations
{
    public class ResultConfiguration: IEntityTypeConfiguration<ResultEntity>
    {
        public void Configure(EntityTypeBuilder<ResultEntity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasIndex(r => r.FileName).IsUnique();

            builder
                .HasMany(r => r.Values)
                .WithOne(v => v.Result)
                .HasForeignKey(v => v.ResultId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
