using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HitPointsTracker.Models
{
    public partial class FullCharacter
    {
        [Required]
        public string? Name { get; set; }

        public int Level { get; set; }

        [Required]
        public CharStats? Stats { get; set; }

        public List<Item>? Items { get; set; }

        [Required]
        public List<CharClass>? Classes { get; set; }

        public List<CharDefense>? Defenses { get; set; }

        public class CharStats
        {
            public int Strength { get; set; }
            public int Dexterity { get; set; }
            public int Constitution { get; set; }
            public int Intelligence { get; set; }
            public int Wisdom { get; set; }
            public int Charisma { get; set; }
        }

        public class Item
        {
            [Required]
            public string? Name { get; set; }
            public ItemModifier? Modifier { get; set; }
        }

        public class ItemModifier
        {
            [Required]
            public string? AffectedObject { get; set; }
            [Required]
            public string? AffectedValue { get; set; }
            public int Value { get; set; }
        }

        public partial class CharClass
        {
            [Required]
            public string? Name { get; set; }
            public int HitDiceValue { get; set; }
            public int ClassLevel { get; set; }
        }

        public class CharDefense
        {
            [Required]
            public string? Type { get; set; }
            [Required]
            [RegularExpression("^(immunity|resistance)$")]
            public string? Defense { get; set; }
        }
    }
}
