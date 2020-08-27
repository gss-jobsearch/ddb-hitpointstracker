using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HitPointsTracker.Models
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }

        public int MaxHitPoints { get; set; }

        public int CurHitPoints { get; set; }

        public int TempHitPoints { get; set; }

        public ICollection<Defense> Defenses { get; } =
            new List<Defense>();

        public Character()
        {
            Name = "";
        }

        public class Conf : IEntityTypeConfiguration<Character>
        {
            public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Character> builder)
            {
                builder.ToTable("Character");
            }
        }
    }
}