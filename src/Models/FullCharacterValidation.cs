using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HitPointsTracker.Models
{
    public partial class FullCharacter : IValidatableObject
    {
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (Level <= 0)
            {
                yield return new ValidationResult(
                    "Character level must be at least one",
                    new string[] { nameof(Level) }
                    );
            }
            if (Classes is List<CharClass> classes)
            {
                int classLevels = classes
                    .Select(c => c.ClassLevel)
                    .Sum();
                if (classLevels != Level)
                {
                    yield return new ValidationResult(
                        $"Character level ({Level}) does not match total class levels ({classLevels})",
                        new string[] { nameof(Level), nameof(Classes) }
                        );
                }
            }
        }

        public partial class CharClass : IValidatableObject
        {
            IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
            {
                if (ClassLevel <= 0)
                {
                    yield return new ValidationResult(
                        "Class level must be at least one",
                        new string[] { nameof(ClassLevel) }
                        );
                }
                switch (HitDiceValue)
                {
                    case 6:
                    case 8:
                    case 10:
                    case 12:
                        break;
                    default:
                        yield return new ValidationResult(
                            "Invalid hit dice value",
                            new string[] { nameof(HitDiceValue) }
                            );
                        break;
                }
            }
        }
    }
}
