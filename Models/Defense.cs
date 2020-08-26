using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HitPointsTracker.Models
{
    public class Defense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long CharacterId { get; set; }

        public string DamageType { get; set; }

        public bool IsImmune { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public Character? Character { get; set; }

        public Defense()
        {
            DamageType = "";
        }

        public class Conf : IEntityTypeConfiguration<Defense>
        {
            public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Defense> builder)
            {
                builder.ToTable("Defense");

                builder
                    .HasOne(e => e.Character)
                    .WithMany(t => t!.Defenses);
            }
        }
    }
}
