using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HitPointsTracker.Models
{
    public partial class FullCharacter : IValidatableObject
    {
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (Stats != null)
            {
                foreach (var result in ValidateObj(Stats))
                {
                    yield return result;
                }
            }
            if (Items != null)
            {
                foreach (var result in ValidateMultiple(Items))
                {
                    yield return result;
                }
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

                foreach (var result in ValidateMultiple(classes))
                {
                    yield return result;
                }
            }
            if (Defenses != null)
            {
                foreach (var result in ValidateMultiple(Defenses))
                {
                    yield return result;
                }
            }
        }

        private static readonly IEnumerable<ValidationResult> Empty =
            Enumerable.Empty<ValidationResult>();

        private static IEnumerable<ValidationResult> ValidateObj<T>(T obj)
        {
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                obj, new ValidationContext(obj), results, true) ?
                    Empty : results;
        }

        private static IEnumerable<ValidationResult> ValidateMultiple<T>(
            IEnumerable<T> obj) => obj.SelectMany(ValidateObj);

        public partial class Item : IValidatableObject
        {
            IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) =>
                (Modifier == null) ? Empty : ValidateObj(Modifier);
        }
    }
}
