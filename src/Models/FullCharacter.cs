using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HitPointsTracker.Models
{
    public partial class FullCharacter
    {
        [Required]
        public string? Name { get; set; }

        [Range(1, 20)]
        public int Level { get; set; }

        [Required]
        public CharStats? Stats { get; set; }

        public List<Item>? Items { get; set; }

        [Required]
        public List<CharClass>? Classes { get; set; }

        public List<CharDefense>? Defenses { get; set; }

        public class CharStats
        {
            [Range(1, 20)]
            public int Strength { get; set; }
            [Range(1, 20)]
            public int Dexterity { get; set; }
            [Range(1, 20)]
            public int Constitution { get; set; }
            [Range(1, 20)]
            public int Intelligence { get; set; }
            [Range(1, 20)]
            public int Wisdom { get; set; }
            [Range(1, 20)]
            public int Charisma { get; set; }
        }

        public partial class Item
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

        public class CharClass
        {
            [Required]
            public string? Name { get; set; }
            [IntValuesValidation(6, 8, 10, 12)]
            public int HitDiceValue { get; set; }
            [IntMinimumValidation(1)]
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
