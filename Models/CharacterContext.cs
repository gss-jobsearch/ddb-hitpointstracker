using Microsoft.EntityFrameworkCore;

namespace HitPointsTracker.Models
{
    public class CharacterContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<Defense> Defenses { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public CharacterContext(DbContextOptions<CharacterContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new Character.Conf())
                .ApplyConfiguration(new Defense.Conf());
        }

    }
}
